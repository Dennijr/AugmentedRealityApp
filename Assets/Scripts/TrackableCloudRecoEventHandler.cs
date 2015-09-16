//Trackable event handler for video
//For 3d object look at TrackableEventHandler.cs
using System;
using UnityEngine;
using Vuforia;
using System.Collections;

namespace AssemblyCSharp
{
    // A custom handler that implements the ITrackableEventHandler interface.
    public class TrackableCloudRecoEventHandler : MonoBehaviour,
    ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES

        private int currentMetadataId;

        private ImageTargetBehaviour mImageTargetBehaviour = null;

        private TrackableBehaviour mTrackableBehaviour;
        private VideoPlaybackBehaviour video;

        private GameObject videoObject;
        private GameObject threeDObject;

        private bool mHasBeenFound = false;
        private bool mLostTracking;
        private bool videoFinished;

        private float mSecondsSinceLost;
        private float distanceToCamera;

        private float mVideoCurrentPosition;
        private float mCurrentVolume;

        // Below variables are used to detect the state change
        // They are updated only in state change handling
        private bool isVideoPlaying = false;
        private bool isTrackingLost = true;
        private VideoPlayerHelper.MediaState videoCurrentState = VideoPlayerHelper.MediaState.NOT_READY;

        #endregion // PRIVATE_MEMBER_VARIABLES

        #region PUBLIC_MEMBER_VARIABLES

        [HideInInspector]
        public bool isAudioMuted = false;
        [HideInInspector]
        public GameObject overlayObject = null;

        public event EventHandler OnTrackingLostHandler;
        public event EventHandler OnTrackingFoundHandler;
        public event EventHandler OnVideoLoadHandler;
        public event EventHandler OnVideoPlayHandler;
        public event EventHandler OnVideoErrorHandler;
        public event EventHandler OnVideoUnloadHandler;
        public event EventHandler OnVideoFinishHandler;

        private IEnumerator StateChangeHandler()
        {
            yield return null;
            bool nowVideoPlaying = (videoCurrentState == VideoPlayerHelper.MediaState.PLAYING || videoCurrentState == VideoPlayerHelper.MediaState.PLAYING_FULLSCREEN);
            if ((isVideoPlaying != nowVideoPlaying) && CloudRecoEventHandler.metadata.type == "video")
            {
                if (nowVideoPlaying)
                {
                    if (OnVideoPlayHandler != null) OnVideoPlayHandler(this, new EventArgs());
                }
                else if (videoCurrentState == VideoPlayerHelper.MediaState.REACHED_END)
                {
                    if (OnVideoFinishHandler != null) OnVideoFinishHandler(this, new EventArgs());
                }
                else
                {
                    if (OnVideoUnloadHandler != null) OnVideoUnloadHandler(this, new EventArgs());
                }
                if (!nowVideoPlaying && !mLostTracking && !videoFinished)
                {
                    if (OnVideoLoadHandler != null) OnVideoLoadHandler(this, new EventArgs());
                }
            }
            if (isTrackingLost != mLostTracking)
            {
                if (mLostTracking)
                {
                    if (OnTrackingLostHandler != null) OnTrackingLostHandler(this, new EventArgs());
                }
                else
                {
                    if (OnTrackingFoundHandler != null) OnTrackingFoundHandler(this, new EventArgs());
                }
                if (!nowVideoPlaying && !mLostTracking && !videoFinished && CloudRecoEventHandler.metadata.type == "video")
                {
                    if (OnVideoLoadHandler != null) OnVideoLoadHandler(this, new EventArgs());
                }
            }
            else if (videoCurrentState == VideoPlayerHelper.MediaState.ERROR)
            {
                if (OnVideoErrorHandler != null) OnVideoErrorHandler(this, new EventArgs());
            }
            isVideoPlaying = nowVideoPlaying;
            isTrackingLost = mLostTracking;
        }

        #endregion //PUBLIC_MEMBER_VARIABLES

        #region UNITY_MONOBEHAVIOUR_METHODS

        void Start()
        {
            threeDObject = GameObject.Find("GameObject");
            videoObject = GameObject.Find("Video");

            mImageTargetBehaviour = GetComponent<ImageTargetBehaviour>();

            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }

            video = GetComponentInChildren<VideoPlaybackBehaviour>();

            OnTrackingLost();
        }

        void Update()
        {
            if (overlayObject != null)
            {
                Vector3 pointOnTarget = new Vector3(0, 0, 0);
                // We convert the local point to world coordinates
                Vector3 targetPointInWorldRef = transform.TransformPoint(pointOnTarget);
                // We project the world coordinates to screen coords (pixels)
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetPointInWorldRef);

                overlayObject.transform.position = screenPoint;
            }

			if (CloudRecoEventHandler.metadata != null && CloudRecoEventHandler.metadata.type == "video")
            {
                if (video == null) return;

                var newState = video.VideoPlayer.GetStatus();
                if (videoCurrentState != newState)
                {
                    videoCurrentState = newState;
                    StartCoroutine(StateChangeHandler());
                }

                if (!mLostTracking && mHasBeenFound)
                {

                    if (video.CurrentState == VideoPlayerHelper.MediaState.READY ||
                        video.CurrentState == VideoPlayerHelper.MediaState.PAUSED ||
                        video.CurrentState == VideoPlayerHelper.MediaState.STOPPED)
                    {
                        ResumeVideo();
                    }
                    //if video is playing, get distance to camera.
                    else if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
                    {
                        distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.root.position);
                        mCurrentVolume = 1.0f - (Mathf.Clamp01(distanceToCamera * 0.0005f) * 0.5f);
                        SetVolume(mCurrentVolume);
                    }
                    else if (video.CurrentState == VideoPlayerHelper.MediaState.REACHED_END)
                    {
                        videoFinished = true;
                        //Loop automatically if marker is visible and video has reached the end
                        //comment this out if you want the play button to appear when the video has reached the end 
                        //Debug.Log("Video Has ended, not playing again");
                        //Don't repeat video
                        //PlayVideo(false, 0);
                    }
                }

                // Pause the video if tracking is lost for more than n seconds
                if (mHasBeenFound && mLostTracking && !videoFinished)
                {
                    if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
                    {
                        //fade out volume from current if marker is lost
                        SetVolume(Mathf.Clamp01(mCurrentVolume - mSecondsSinceLost));
                        //n.0f is number of seconds before playback stops when marker is lost
                        if (mSecondsSinceLost > 0.0f)
                        {
                            PauseAndUnloadVideo();
                        }
                    }
                    mSecondsSinceLost += Time.deltaTime;
                }
            }
        }

        #endregion // UNITY_MONOBEHAVIOUR_METHODS


        #region PUBLIC_METHODS

        // Implementation of the ITrackableEventHandler function called when the
        // tracking state changes.
        public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED)
            {
                if (currentMetadataId != CloudRecoEventHandler.metadata.id)
                {
                    Debug.Log("New meta data found with id : " + CloudRecoEventHandler.metadata.id);
                    ResetTrackable();
                    currentMetadataId = CloudRecoEventHandler.metadata.id;
                }
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
            // Dont block main thread
            StartCoroutine(StateChangeHandler());
        }

        //Set volume
        private bool SetVolume(float level)
        {
            try
            {
                if (video != null)
                {
                    if (isAudioMuted)
                        return video.VideoPlayer.SetVolume(0);
                    else
                        return video.VideoPlayer.SetVolume(level);
                }
            }
            catch (Exception any)
            {
                Debug.Log("Error with set volume : " + any.Message);
            }
            return false;
        }

        //Mute audio
        public bool SetVolume(bool on)
        {
            try
            {
                if (on)
                {
                    if (mCurrentVolume == 0) mCurrentVolume = 1.0f;
                    if (video != null && video.VideoPlayer.SetVolume(mCurrentVolume))
                    {
                        isAudioMuted = false;
                        return true;
                    }
                }
                else
                {
                    if (video != null && video.VideoPlayer.SetVolume(0))
                    {
                        isAudioMuted = true;
                        return true;
                    }
                }
            }
            catch (Exception any)
            {
                Debug.Log("Error with set volume : " + any.Message);
            }
            return false;
        }

        // Pause the video
        public bool PauseVideo()
        {
            try
            {
                if (video != null && (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING || video.CurrentState == VideoPlayerHelper.MediaState.PLAYING_FULLSCREEN))
                {
                    //get last position so it can resume after video is unloaded and reloaded.
                    mVideoCurrentPosition = video.VideoPlayer.GetCurrentPosition();
                    Debug.Log("Paused video at position " + mVideoCurrentPosition);
                    return video.VideoPlayer.Pause();
                }
            }
            catch (Exception any)
            {
                Debug.Log("Error with video pause : " + any.Message);
            }
            return false;
        }

        //Resume the video
        public bool ResumeVideo()
        {
            try
            {
                if (video != null && video.VideoPlayer.IsPlayableOnTexture())
                {
                    VideoPlayerHelper.MediaState state = video.VideoPlayer.GetStatus();
                    Debug.Log(state);
                    if (state == VideoPlayerHelper.MediaState.PAUSED ||
                        state == VideoPlayerHelper.MediaState.READY ||
                        state == VideoPlayerHelper.MediaState.STOPPED)
                    {
                        Debug.Log("Resuming video at left position : " + mVideoCurrentPosition);
                        return PlayVideo(false, mVideoCurrentPosition);
                    }
                    else if (state == VideoPlayerHelper.MediaState.REACHED_END)
                    {
                        Debug.Log("Resuming video from start.");
                        // Play this video from the beginning
                        // return PlayVideo(false, 0);
                    }
                }
            }
            catch (Exception any)
            {
                Debug.Log("Error with video resume : " + any.Message);
            }
            return false;
        }

        //Play video
        private bool PlayVideo(bool fullScreen, float seekPosition)
        {
            try
            {
                SetVolume(mCurrentVolume);
                Debug.Log("Playing the video seekposition : " + seekPosition);
                if (video != null && video.VideoPlayer.Play(fullScreen, seekPosition))
                {
                    return true;
                }
            }
            catch (Exception any)
            {
                Debug.Log("Error with video play : " + any.Message);
            }
            return false;
        }

        //Both pause and unload video
        public bool PauseAndUnloadVideo()
        {
            try
            {
                if (PauseVideo())
                {
                    if (video.VideoPlayer.Unload())
                    {
                        Debug.Log("UnLoaded Video: " + video.m_path);
                        return true;
                    }
                }
            }
            catch (Exception any)
            {
                Debug.Log("Error with pause and unload video : " + any.Message);
            }
            return false;
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS

        private void ResetTrackable()
        {
            video.VideoPlayer.Unload();
            video.SetNotReady();
            mVideoCurrentPosition = 0;
            mHasBeenFound = false;
            isTrackingLost = true;
            videoFinished = false;
        }

        private void OnTrackingFound()
        {
			if (CloudRecoEventHandler.metadata.type == "video")
            {
                // videoObject.SetActive(true);
                threeDObject.SetActive(false);
                Renderer[] rendererComponents = videoObject.GetComponentsInChildren<Renderer>();
                Collider[] colliderComponents = videoObject.GetComponentsInChildren<Collider>();


                // Enable rendering:
                foreach (Renderer component in rendererComponents)
                {
                    component.enabled = true;
                }

                // Enable colliders:
                foreach (Collider component in colliderComponents)
                {
                    component.enabled = true;
                }

                AudioSource[] audioComponents = GetComponentsInChildren<AudioSource>();

                //Play audio:
                foreach (AudioSource component in audioComponents)
                {
                    component.Play();
                }

                Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");

                // Optionally play the video automatically when the target is found
                // video.InitializeVideoPlayback ();
                if (video != null)
                {
                    video = GetComponentInChildren<VideoPlaybackBehaviour>();

                    video.m_path = CloudRecoEventHandler.metadata.materialurl;

					video.VideoPlayer.SetFilename(CloudRecoEventHandler.metadata.materialurl);

					if (video.VideoPlayer.Load(video.m_path, VideoPlayerHelper.MediaType.ON_TEXTURE, false, mVideoCurrentPosition))
                    {
                        Debug.Log("Loaded video from position : " + mVideoCurrentPosition);
                    }

                    ResumeVideo();
                }
            }
            else if (CloudRecoEventHandler.metadata.type == "3d")
            {
				PauseAndUnloadVideo();
                threeDObject.SetActive(true);
                objReaderCSharpV4 objReader = GetComponentInChildren<objReaderCSharpV4>();
                objReader.StartCoroutine("Init", "GameObject");

            }
            
            mHasBeenFound = true;
            mLostTracking = false;
        }


        public void OnTrackingLost(bool onunload = false)
        {
            // We call this function on leaving the page
            // Some game objects might have been inactive at this stage
            // To avoid errors using try catch 
            try
            {
//				videoObject.SetActive(false);
                threeDObject.SetActive(false);

                Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
                Collider[] colliderComponents = GetComponentsInChildren<Collider>();
                AudioSource[] audioComponents = GetComponentsInChildren<AudioSource>();

                // Disable rendering:
                foreach (Renderer component in rendererComponents)
                {
                    component.enabled = false;
                }

                // Disable colliders:
                foreach (Collider component in colliderComponents)
                {
                    component.enabled = false;
                }

                //Pause Audio:
                foreach (AudioSource component in audioComponents)
                {
                    component.Pause();
                }

                //			Destroy (GetComponentsInChildren<VideoPlayBackCloudRecoBehaviour>());
                Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");

                mLostTracking = true;
                mSecondsSinceLost = 0;
                videoFinished = false;
            }
            catch { }
        }


        #endregion // PRIVATE_METHODS
    }
}