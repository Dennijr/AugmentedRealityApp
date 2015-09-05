using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CustomUI
{
    public class WhatsNewDetails : MonoBehaviour
    {
        public RawImage MainImage;
        public Text Title;
        public Text Description;
        public RawImage DetailsImage;
        public Button VideoButton;
        public Button ReadMoreButton;
        public Button ScanButton;
        public Button ShareButton;
        public Button EmailButton;

        private string videoLink;
        private string readMoreLink;

        public void Start()
        {
			VideoButton.onClick.AddListener(delegate {
				if (videoLink != null) OpenURL(videoLink);
			});

			ReadMoreButton.onClick.AddListener (delegate {
				if (readMoreLink != null) OpenURL(readMoreLink);
			});

			ShareButton.onClick.AddListener (delegate {
				AppSocial.ShareToFacebook();
			});

		}

		void OpenURL(string link)
		{

			Application.OpenURL(link);
		}

        public void LoadContent(WhatsNewListSource source)
        {
            if (MainImage != null)
            {
                StartCoroutine(LoadImage(true, source.backgroundImageURL));
            }
            if (Title != null)
            {
                Title.text = source.title;
            }
            if (Description != null)
            {
                Description.text = source.description;
            }
            if (DetailsImage != null && source.imageURL != null) {
				DetailsImage.gameObject.SetActive (true);
				StartCoroutine (LoadImage (false, source.imageURL));
			} else
				DetailsImage.gameObject.SetActive (false);

			if (source.videoURL != null)
				videoLink = source.videoURL;
			else
				videoLink = null;

			if (source.linkURL != null) 
				readMoreLink = source.linkURL;
			else 
				readMoreLink = null;


			if (ScanButton != null)
			{

			}
		}
		
		private IEnumerator LoadImage(bool isMain, string url)
        {
            WWW www = new WWW(url);

            yield return www;

            Texture2D imageTexture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(imageTexture);
            if (isMain)
            {
                this.MainImage.texture = imageTexture;
            }
            else
            {
                this.DetailsImage.texture = imageTexture;
            }
            www.Dispose();
            www = null;
        }
    }
}