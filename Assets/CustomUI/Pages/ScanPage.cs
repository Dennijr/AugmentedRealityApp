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

        public GameObject flashButton;
        public GameObject volumeButton;
        public GameObject shareButton;
        public GameObject linkButton;
        public GameObject captureButton;

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
            }
            catch { }
        }

        private void trackableCloudRecoEventHandler_OnTrackingFoundHandler(object sender, System.EventArgs e)
        {
            volumeButton.SetActive(true);
            shareButton.SetActive(true);
            linkButton.SetActive(true);
        }

        private void trackableCloudRecoEventHandler_OnTrackingLostHandler(object sender, System.EventArgs e)
        {
			loadingIndicator.SetActive (false);
        }

        private void trackableCloudRecoEventHandler_OnVideoPlayHandler(object sender, System.EventArgs e)
        {
			Debug.Log ("On Video Play Handler");
			loadingIndicator.SetActive(false);
            captureButton.SetActive(true);
        }

		private void trackableCloudRecoEventHandler_OnVideoLoadHandler(object sender, System.EventArgs e)
		{
			loadingIndicator.SetActive(true);
		}

        private void trackableCloudRecoEventHandler_OnVideoUnloadHandler(object sender, System.EventArgs e)
        {
            captureButton.SetActive(false);
        }

        public override void OnNavigatingTo(NavigationEventArgs e)
        {
			Debug.Log("Navigating to SCan");
			loadingIndicator.SetActive (true);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            isPageActive = true;
            Invoke("DelayedLoad", 0.5f);
        }

        private void DisableArCamera()
        {
            arCamera.SetActive(false);
        }

        private void DelayedLoad()
        {
            if (isPageActive)
            {
                arCamera.SetActive(true);
				loadingIndicator.SetActive (false);
                cloudRecognition.SetActive(true);
                imageTarget.SetActive(true);
                background.SetActive(false);
            }
        }

        public override void OnNavigatingFrom(NavigationEventArgs e)
        {
            isPageActive = false;
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            arCamera.SetActive(false);
            trackableCloudRecoEventHandler.PauseAndUnloadVideo();
            trackableCloudRecoEventHandler.OnTrackingLost();
            background.SetActive(true);
			loadingIndicator.SetActive(false);
            cloudRecognition.SetActive(false);
            imageTarget.SetActive(false);
            SetFlash(false);
            volumeButton.SetActive(false);
            shareButton.SetActive(false);
            linkButton.SetActive(false);
        }

        public void ToggleVolume()
        {
            if (trackableCloudRecoEventHandler.SetVolume(!isAudioEnabled))
            {
                isAudioEnabled = !isAudioEnabled;
                volumeButton.transform.Find("Image").GetComponent<ImageToggle>().SetSprite(isAudioEnabled);
            }
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
            Application.OpenURL("http://www.facebook.com");
        }

        public void HyperlinkClick()
        {
            Application.OpenURL("http://www.google.com");
        }

        public void CaptureScreenShot()
        {
            Application.CaptureScreenshot(FileManager.GetScreenShotFileName());
            PagesManager.DisplayToast("Screenshot captured");
        }
    }
}