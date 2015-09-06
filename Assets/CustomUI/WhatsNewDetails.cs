using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CustomUI
{
    public class WhatsNewDetails : MonoBehaviour
    {
        public RawImage MainImage;
        public RawImage DetailsImage;
        public Text Title;
        public Text Description;
        public Button VideoButton;
        public Button ReadMoreButton;
        public Button ScanButton;
        public Button ShareButton;
        public Button EmailButton;

        private CanvasGroup canvasGroup;

        private Texture originalMainTexture, originalDetailTexture;

        private string title, description, videoLink, shareLink, readMoreLink;

        public void Start()
        {
            if (MainImage != null) originalMainTexture = MainImage.texture;
            if (DetailsImage != null) originalDetailTexture = DetailsImage.texture;

			VideoButton.onClick.AddListener(delegate {
				if (!string.IsNullOrEmpty(videoLink)) OpenURL(videoLink);
			});

			ReadMoreButton.onClick.AddListener (delegate {
                if (!string.IsNullOrEmpty(readMoreLink)) OpenURL(readMoreLink);
			});

			ShareButton.onClick.AddListener (delegate {
				AppSocial.ShareToFacebook();
			});

            EmailButton.onClick.AddListener(() => PushEmail());
		}

        public void Enable()
        {
            this.gameObject.SetActive(true);
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            StartCoroutine(FadeIn());
        }

        public void Disable()
        {
            StartCoroutine(FadeOut());
        }

        float duration = 0.6f;
        private IEnumerator FadeIn() 
        {
            for (var t = 0.0f; t < duration; t += Time.deltaTime)
            {
                canvasGroup.alpha = t / duration;
                yield return null;
            }
            canvasGroup.alpha = 1;
        }

        private IEnumerator FadeOut()
        {
            for (var t = duration; t >= 0.0f; t -= Time.deltaTime)
            {
                canvasGroup.alpha = t / duration;
                yield return null;
            }
            canvasGroup.alpha = 0;
            this.gameObject.transform.parent.gameObject.SetActive(false);
            this.gameObject.transform.SetParent(null);
            Destroy(this.gameObject);
        }

        private void PushEmail()
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
                AppSocial.SendEmail(title, description);
        }

        public void Reset()
        {
            title = string.Empty;
            description = string.Empty;
            videoLink = string.Empty;
            shareLink = string.Empty;
            readMoreLink = string.Empty;

            if (originalMainTexture != null) MainImage.texture = originalMainTexture;
            if (originalDetailTexture != null) DetailsImage.texture = originalDetailTexture;

            if (Title != null) Title.text = string.Empty;
            if (Description != null) Description.text = string.Empty;
        }

		void OpenURL(string link)
		{
			Application.OpenURL(link);
		}

        public void LoadContent(WhatsNewListSource source)
        {
            title = source.title;
            description = source.description;
            videoLink = source.videoURL;
            readMoreLink = source.linkURL;



            if (!string.IsNullOrEmpty(source.backgroundImageURL))
            {
                StartCoroutine(LoadImage(true, source.backgroundImageURL));
            }
            if (!string.IsNullOrEmpty(title)) Title.text = title;
            if (!string.IsNullOrEmpty(description)) Description.text = description;
            
            if (!string.IsNullOrEmpty(source.imageURL))
            {
                DetailsImage.gameObject.SetActive(true);
                StartCoroutine(LoadImage(false, source.imageURL));
            }
            else
            {

            }

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