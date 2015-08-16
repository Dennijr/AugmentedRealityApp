﻿// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;

using UnityEngine;
using Vuforia;


namespace AssemblyCSharp
{
    // A custom handler that implements the ITrackableEventHandler interface.
    public class TrackableCloudRecoEventHandler : MonoBehaviour,
    ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES

        private TrackableBehaviour mTrackableBehaviour;
        private VideoPlaybackBehaviour video;

		private GameObject videoObject;
		private GameObject gameObject;


        private bool mHasBeenFound = false;
        private bool mLostTracking;
        private bool videoFinished;
        private bool isAudioMuted = false;
        private float mSecondsSinceLost;
        private float distanceToCamera;

        private float mVideoCurrentPosition;
        private float mCurrentVolume;

        private Transform mMyModel;


        #endregion // PRIVATE_MEMBER_VARIABLES

        #region PUBLIC_MEMBER_VARIABLES

        public event EventHandler OnTrackingLostHandler;
        public event EventHandler OnTrackingFoundHandler;
        public event EventHandler OnVideoPlayHandler;
        public event EventHandler OnVideoUnloadHandler;

        private void CallOnTrackingLostHandler()
        {
            if (OnTrackingLostHandler != null) OnTrackingLostHandler(this, new EventArgs());
        }

        private void CallOnTrackingFoundHandler()
        {
            if (OnTrackingFoundHandler != null) OnTrackingFoundHandler(this, new EventArgs());
        }

        private void CallOnVideoPlayHandler()
        {
            if (OnVideoPlayHandler != null) OnVideoPlayHandler(this, new EventArgs());
        }

        private void CallOnVideoUnloadHandler()
        {
            if (OnVideoUnloadHandler != null) OnVideoUnloadHandler(this, new EventArgs());
        }

        #endregion //PUBLIC_MEMBER_VARIABLES

        #region UNITY_MONOBEHAVIOUR_METHODS

        void Start()
        {
            /*for custom animations on update
Transform[] allChildren = GetComponentsInChildren<Transform>();
foreach (Transform child in allChildren) {
     // do whatever with child transform here
if (child.name == "MyModel") mMyModel = child;
}
*/
			gameObject = GameObject.Find ("GameObject");
			videoObject = GameObject.Find ("Video");

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
			if (CloudRecoEventHandler.type == "video") {
				if (video == null)
					return;

				if (!mLostTracking && mHasBeenFound) {
					/*
//whatever custom animation is performed per update frame if tracker is found
if (mMyModel)
{
mMyModel.Rotate(0.0f, -0.2666f, 0.0f);
}
*/
					//if video is playing, get distance to camera.
					if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING) {
						distanceToCamera = Vector3.Distance (Camera.main.transform.position, transform.root.position);
						mCurrentVolume = 1.0f - (Mathf.Clamp01 (distanceToCamera * 0.0005f) * 0.5f);
						SetVolume (mCurrentVolume);
					} else if (video.CurrentState == VideoPlayerHelper.MediaState.REACHED_END) {
						//Loop automatically if marker is visible and video has reached the end
						//comment this out if you want the play button to appear when the video has reached the end 
						Debug.Log ("Video Has ended, playing again");
						PlayVideo (false, 0);
					}
				}

				// Pause the video if tracking is lost for more than n seconds
				if (mHasBeenFound && mLostTracking && !videoFinished) {
					if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING) {
						//fade out volume from current if marker is lost
						Debug.Log (mCurrentVolume - mSecondsSinceLost);
						SetVolume (Mathf.Clamp01 (mCurrentVolume - mSecondsSinceLost));
					}
					//n.0f is number of seconds before playback stops when marker is lost
					if (mSecondsSinceLost > 1.0f) {
						PauseAndUnloadVideo ();
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
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }

        //Set volume
        private bool SetVolume(float level)
        {
            try
            {
                if (video != null && !isAudioMuted) return video.VideoPlayer.SetVolume(level);
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
                    isAudioMuted = false;
                    if (video != null) return video.VideoPlayer.SetVolume(mCurrentVolume);
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
                if (video != null && video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
                {
                    //get last position so it can resume after video is unloaded and reloaded.
                    mVideoCurrentPosition = video.VideoPlayer.GetCurrentPosition();
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
                    if (state == VideoPlayerHelper.MediaState.PAUSED ||
                        state == VideoPlayerHelper.MediaState.READY ||
                        state == VideoPlayerHelper.MediaState.STOPPED)
                    {
                        Debug.Log("Video File: " + video.m_path);
                        return PlayVideo(false, video.VideoPlayer.GetCurrentPosition());
                    }
                    else if (state == VideoPlayerHelper.MediaState.REACHED_END)
                    {
                        // Play this video from the beginning
                        return PlayVideo(false, 0);
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
                if (video != null && video.VideoPlayer.Play(fullScreen, seekPosition))
                {
                    StartCoroutine("CallOnVideoPlayHandler");
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
                        StartCoroutine("CallOnVideoUnloadHandler");
                        videoFinished = true;
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


        private void OnTrackingFound()
        {
            if (OnTrackingFoundHandler != null) OnTrackingFoundHandler(this, new EventArgs());
           
			if (CloudRecoEventHandler.type == "video") {
				videoObject.SetActive(true);
				gameObject.SetActive(false);
				Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
				Collider[] colliderComponents = GetComponentsInChildren<Collider>();
				

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
				foreach (AudioSource component in audioComponents) {
					component.Play ();
				}

				Debug.Log ("Trackable " + mTrackableBehaviour.TrackableName + " found");

				// Optionally play the video automatically when the target is found
				//			video.InitializeVideoPlayback ();
				if (video != null) {
					video = GetComponentInChildren<VideoPlaybackBehaviour> ();

					video.m_path = CloudRecoEventHandler.mPath;

					video.VideoPlayer.SetFilename (CloudRecoEventHandler.mPath);

					if (video.VideoPlayer.Load (video.m_path, VideoPlayerHelper.MediaType.ON_TEXTURE, true, mVideoCurrentPosition)) {
//						Debug.Log ("Loaded Video: " + video.m_path + " Video Texture Id: " + video.mVideoTexture.GetNativeTextureID ());
					}

					ResumeVideo ();
				}
			} else {

				videoObject.SetActive (false);
				gameObject.SetActive (true);
				objReaderCSharpV4 objReader = GetComponentInChildren<objReaderCSharpV4> ();
				objReader.StartCoroutine ("Init", "GameObject");
			}


            mHasBeenFound = true;
            mLostTracking = false;
        }


        public void OnTrackingLost()
        {

			videoObject.SetActive (false);
			gameObject.SetActive (false);
			if (OnTrackingLostHandler != null) OnTrackingLostHandler(this, new EventArgs());
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


        #endregion // PRIVATE_METHODS
    }
}