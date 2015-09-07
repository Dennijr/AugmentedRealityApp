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

        private string title, description, videoLink, shareLink, readMoreLink;

        public void Start()
        {
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

            // set Main image at the top
            if (!string.IsNullOrEmpty(source.backgroundImageURL))
            {
                StartCoroutine(LoadImage(true, source.backgroundImageURL));
            }
            // Set title
            if (!string.IsNullOrEmpty(title)) Title.text = title;
            // Set description
            if (!string.IsNullOrEmpty(description)) Description.text = description;
            // If video url doesn't exists, delete the game object
            if (string.IsNullOrEmpty(videoLink))
            {
                try
                {
                    Destroy(VideoButton.gameObject);
                }
                catch { }
            }
            // If details image doesn't exitst delete the game object
            if (!string.IsNullOrEmpty(source.imageURL))
            {
                StartCoroutine(LoadImage(false, source.imageURL));
            }
            else
            {
                try
                {
                    Destroy(DetailsImage.gameObject);
                }
                catch { }
            }
            // If read more link not present remove the object
            if (string.IsNullOrEmpty(readMoreLink))
            {
                Destroy(ReadMoreButton.gameObject);
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
                var width = imageTexture.width;
                var height = imageTexture.height;
                if (width > 0 && height > 0)
                {
                    var ratio = (CanvasConstants.ReferenceWidth - 20) / width;
                    var layoutElement = DetailsImage.gameObject.GetComponent<LayoutElement>();
                    layoutElement.preferredWidth = width * ratio;
                    layoutElement.preferredHeight = height * ratio;
                    this.DetailsImage.texture = imageTexture;
                }
            }
            www.Dispose();
            www = null;
        }
    }
}