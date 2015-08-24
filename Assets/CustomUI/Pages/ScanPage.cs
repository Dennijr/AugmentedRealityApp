using UnityEngine;
using System.Collections;
using MaterialUI;
using Vuforia;
using AssemblyCSharp;
using System;

namespace CustomUI
{
    public class ScanPage : BasePage
    {
        public GameObject arCamera;
        public GameObject background;
        public GameObject cloudRecognition;
        public GameObject imageTarget;
        public GameObject loadingIndicator;
		public GameObject streamingIndicator;

        public GameObject sharePopup;
        public GameObject flashButton;
        public GameObject volumeButton;
        public GameObject shareButton;
        public GameObject linkButton;
        public GameObject captureButton;
        public GameObject moreInfoButton;

        private TrackableCloudRecoEventHandler trackableCloudRecoEventHandler;
        private bool flashEnabled = false;
        private bool isPageActive = false;
        private bool isAudioEnabled = true;

        public override void Start()
        {
            base.Start();
            try
            {
                arCamera.SetActive(true);
                Invoke("DisableArCamera", 0.5f);
                trackableCloudRecoEventHandler = imageTarget.GetComponent<TrackableCloudRecoEventHandler>();
                trackableCloudRecoEventHandler.OnTrackingLostHandler += trackableCloudRecoEventHandler_OnTrackingLostHandler;
                trackableCloudRecoEventHandler.OnTrackingFoundHandler += trackableCloudRecoEventHandler_OnTrackingFoundHandler;
                trackableCloudRecoEventHandler.OnVideoPlayHandler += trackableCloudRecoEventHandler_OnVideoPlayHandler;
				trackableCloudRecoEventHandler.OnVideoLoadHandler += trackableCloudRecoEventHandler_OnVideoLoadHandler;
                trackableCloudRecoEventHandler.OnVideoUnloadHandler += trackableCloudRecoEventHandler_OnVideoUnloadHandler;
                trackableCloudRecoEventHandler.OnVideoFinishHandler += trackableCloudRecoEventHandler_OnVideoFinishHandler;
            }
            catch { }
        }

        private void trackableCloudRecoEventHandler_OnVideoFinishHandler(object sender, EventArgs e)
        {
            moreInfoButton.SetActive(true);
            trackableCloudRecoEventHandler.overlayObject = moreInfoButton;
        }

        private void trackableCloudRecoEventHandler_OnTrackingFoundHandler(object sender, System.EventArgs e)
        {
            shareButton.SetActive(true);
            linkButton.SetActive(true);
        }

        private void trackableCloudRecoEventHandler_OnTrackingLostHandler(object sender, System.EventArgs e)
        {
            trackableCloudRecoEventHandler.overlayObject = null;
            moreInfoButton.SetActive(false);
			streamingIndicator.SetActive (false);
        }

        private void trackableCloudRecoEventHandler_OnVideoPlayHandler(object sender, System.EventArgs e)
        {
			streamingIndicator.SetActive(false);
            volumeButton.SetActive(true);
            captureButton.SetActive(true);
        }

		private void trackableCloudRecoEventHandler_OnVideoLoadHandler(object sender, System.EventArgs e)
		{
			streamingIndicator.SetActive(true);
		}

        private void trackableCloudRecoEventHandler_OnVideoUnloadHandler(object sender, System.EventArgs e)
        {
            volumeButton.SetActive(false);
            captureButton.SetActive(false);
        }

        public override void OnNavigatingTo(NavigationEventArgs e)
        {
			//loadingIndicator.SetActive (true);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            isPageActive = true;
            loadingIndicator.SetActive(true);
            //Delay camera load so that the page transition effect finishes
            Invoke("LoadCamera", 0.5f);
        }

        private void DisableArCamera()
        {
            arCamera.SetActive(false);
        }

        private void LoadCamera()
        {
            if (isPageActive)
            {
                arCamera.SetActive(true);
                cloudRecognition.SetActive(true);
                imageTarget.SetActive(true);
                //Delay disabling background or home page contents will be shown till the camera feedback appears
                Invoke("ShowCamera", 0.1f);
            }
        }

        private void ShowCamera()
        {
            background.SetActive(false);
            loadingIndicator.SetActive(false);
        }

        public override void OnNavigatingFrom(NavigationEventArgs e)
        {
            isPageActive = false;
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            trackableCloudRecoEventHandler.PauseAndUnloadVideo();
            trackableCloudRecoEventHandler.OnTrackingLost(true);
            trackableCloudRecoEventHandler.isAudioMuted = false;
            SetVolumeButton(true);
            background.SetActive(true);
            arCamera.SetActive(false);
			streamingIndicator.SetActive(false);
            loadingIndicator.SetActive(false);
            cloudRecognition.SetActive(false);
            imageTarget.SetActive(false);
            SetFlash(false);
            // hide the share popup on closing
            sharePopup.SetActive(false);
            volumeButton.SetActive(false);
            shareButton.SetActive(false);
            linkButton.SetActive(false);
            captureButton.SetActive(false);
        }

        public void ToggleVolume()
        {
            if (trackableCloudRecoEventHandler.SetVolume(!isAudioEnabled))
            {
                isAudioEnabled = !isAudioEnabled;
                volumeButton.transform.Find("Image").GetComponent<ImageToggle>().SetSprite(isAudioEnabled);
            }
        }

        private void SetVolumeButton(bool on)
        {
            isAudioEnabled = on;
            volumeButton.transform.Find("Image").GetComponent<ImageToggle>().SetSprite(isAudioEnabled);
        }

        public void ToggleFlash()
        {
            SetFlash(!flashEnabled);
        }

        private void SetFlash(bool on)
        {
            if (flashEnabled == on) return;
            if (!CameraDevice.Instance.SetFlashTorchMode(on)) on = false;
            flashEnabled = on;
            flashButton.transform.Find("Image").GetComponent<ImageToggle>().SetSprite(!on);
        }

        public void ShareClick()
        {
            //Application.OpenURL("http://www.facebook.com");
            sharePopup.SetActive(true);
        }

        public void HyperlinkClick()
        {
            Application.OpenURL(CloudRecoEventHandler.metadata["hyperlink"].str.Replace("\\", ""));
        }


        public void CaptureScreenShot()
        {
            Application.CaptureScreenshot(FileManager.GetScreenShotFileName());
            PagesManager.DisplayToast("Screenshot captured");
        }

        public void ShareToFacebook()
        {
            AppSocial.ShareToFacebook();
        }

        public void ShareToTwitter()
        {
            AppSocial.ShareToTwitter();
        }

        // Hide the share popup when hardware back button is pressed
        public override void OnBackKeyPress(CancellationEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (sharePopup.activeSelf)
            {
                sharePopup.SetActive(false);
                e.CancelEvent = true;
            }
        }
    }
}