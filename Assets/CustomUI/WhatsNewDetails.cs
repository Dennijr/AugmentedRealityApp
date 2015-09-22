using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Soomla.Profile;

namespace CustomUI
{
    public class WhatsNewDetails : MonoBehaviour
    {
        private WhatsNewListSource detailsSource;

        public RawImage MainImage;
        public RawImage DetailsImage;
        public Text Title;
        public Text Description;
        public Button VideoButton;
        public Button ReadMoreButton;
        public Button ScanButton;
        public Button ShareButton;
        public Button EmailButton;

		public Button LikeButton;
        public Text LikeCount;
		public Button CommentButton;
        public Text CommentCount;

		public GameObject CommentPopup;
		public GameObject SharePopup;
		public InputField CommentInput;

        private WhatsNewCommentController commentController;

		public void Start()
        {
			VideoButton.onClick.AddListener(delegate {
                if (!string.IsNullOrEmpty(detailsSource.videoURL)) OpenURL(detailsSource.videoURL);
			});

			ReadMoreButton.onClick.AddListener (delegate {
                if (!string.IsNullOrEmpty(detailsSource.linkURL)) OpenURL(detailsSource.linkURL);
			});

			ShareButton.onClick.AddListener (delegate {
				ShowSharePopup();
			});

            EmailButton.onClick.AddListener(() => PushEmail());

			LikeButton.onClick.AddListener (() => Like ());

			CommentButton.onClick.AddListener(() => ShowCommentPopup());

			ScanButton.onClick.AddListener(() => { CanvasConstants.Navigate("Scan"); });
		}

        public void SetSource(WhatsNewListSource source)
        {
            this.detailsSource = source;
        }

		private static IEnumerator PostUserToServer(UserProfile userProfile)
		{
			Debug.Log ("Posting data: ");
			string url = string.Format (CanvasConstants.UserSURL + "?request=adduser&provider={0}&provider_user_id={1}&name={2}&email={3}", userProfile.Provider, userProfile.ProfileId, userProfile.FirstName, userProfile.Email);
			Debug.Log("Posting user data: " + url );
			WWW www = new WWW (url);
			yield return www;
		}

		public void UpdateStory ()
		{
			Debug.Log ("Share button clicked. Login status: " + SoomlaProfile.IsLoggedIn (Provider.FACEBOOK));
			if (!SoomlaProfile.IsLoggedIn (Provider.FACEBOOK)) {
				SoomlaProfile.Login (Provider.FACEBOOK);
				ProfileEvents.OnLoginFinished += (UserProfile userProfile, bool autologin, string payload) =>  {
					//Your code to execute here
					Debug.Log ("onloginfinished Logged into " + userProfile.Provider + " username: " + userProfile.FirstName);
					StartCoroutine (PostUserToServer (userProfile));
				};
			}
            SoomlaProfile.UpdateStory(Provider.FACEBOOK, detailsSource.title, detailsSource.title, detailsSource.title, detailsSource.description, detailsSource.linkURL, detailsSource.imageURL, "", null);
			CloseSharePopup();
		}

		public void UpdateTwitterStory ()
		{
			Debug.Log ("Share button clicked. Login status: " + SoomlaProfile.IsLoggedIn (Provider.TWITTER));
			if (!SoomlaProfile.IsLoggedIn (Provider.TWITTER)) {
				SoomlaProfile.Login (Provider.TWITTER);
				ProfileEvents.OnLoginFinished += (UserProfile userProfile, bool autologin, string payload) =>  {
					//Your code to execute here
					Debug.Log ("onloginfinished Logged into " + userProfile.Provider + " username: " + userProfile.FirstName);
					StartCoroutine (PostUserToServer (userProfile));
				};
			}
            SoomlaProfile.UpdateStory(Provider.TWITTER, detailsSource.title, detailsSource.title, detailsSource.title, detailsSource.description, detailsSource.linkURL, detailsSource.imageURL, "", null);
			CloseSharePopup();
		}

		
		void OnGUI()
		{
			if (CommentInput.isFocused && CommentInput.text != "" && Input.GetKey(KeyCode.Return)) {
                var comment = CommentInput.text;
                CommentInput.text = "";
                PostComment(comment);
			}
		}


		public void ShowCommentPopup()
		{
			if (CommentPopup == null)
				return;
			commentController = CommentPopup.GetComponent<WhatsNewCommentController>();
			Enable (CommentPopup);
			if (commentController != null)
				commentController.LoadWhatsNewComments(detailsSource.id);
		}

		public void ShowSharePopup()
		{
			if (SharePopup == null)
				return;
			Enable (SharePopup);
		}

        public bool CanNavigateBack()
        {
            if (CommentPopup.activeSelf)
            {
                CloseCommentPopup();
                return false;
            }
            return true;
        }

        public void CloseCommentPopup()
        {
            CanvasConstants.ShowLoading(false);
            Disable(CommentPopup, false);
        }

		public void CloseSharePopup()
		{
			Disable(SharePopup, false);
		}

        public void Enable(GameObject gObject = null)
        {
			if (gObject == null)
				gObject = this.gameObject;
            gObject.SetActive(true);
            var canvasGroup = gObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            StartCoroutine(FadeIn(canvasGroup));
        }

        public void Disable(GameObject gObject = null, bool deleteGameObject = true)
        {
            if (gObject == null)
                gObject = this.gameObject;
            var canvasGroup = gObject.GetComponent<CanvasGroup>();
            StartCoroutine(FadeOut(canvasGroup, deleteGameObject));
        }

        float duration = 0.6f;
        private IEnumerator FadeIn(CanvasGroup canvasGroup)
        {
            for (var t = 0.0f; t < duration; t += Time.deltaTime)
            {
                canvasGroup.alpha = t / duration;
                yield return null;
            }
            canvasGroup.alpha = 1;
        }

        private IEnumerator FadeOut(CanvasGroup canvasGroup, bool deleteGameObject)
        {
            for (var t = duration; t >= 0.0f; t -= Time.deltaTime)
            {
                canvasGroup.alpha = t / duration;
                yield return null;
            }
            canvasGroup.alpha = 0;
            if (deleteGameObject)
            {
                canvasGroup.transform.parent.gameObject.SetActive(false);
                canvasGroup.transform.SetParent(null);
                Destroy(canvasGroup.gameObject);
            }
            else
            {
                canvasGroup.gameObject.SetActive(false);
            }
        }

        private void PushEmail()
        {
            if (!string.IsNullOrEmpty(detailsSource.title) && !string.IsNullOrEmpty(detailsSource.description))
                AppSocial.SendEmail(detailsSource.title, detailsSource.description);
        }

        public void PostComment(string comment)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                string url = CanvasConstants.WhatsNewSURL + "?request=comment&id=" + detailsSource.id + "&comment=" + WWW.EscapeURL(comment);
                StartCoroutine(Utils.PostRequest(url, true, true, HandleCommentResponse));
            }
        }

        private void HandleCommentResponse(object sender, System.EventArgs e)
        {
            if (sender != null)
            {
                var www = sender as WWW;
                var response = new JSONObject(www.text);
                if (response != null)
                {
                    CommentInput.text = "";
                    CommentInput.transform.Find("DisplayText").GetComponent<Text>().text = "";
                    detailsSource.commentscount = Utils.GetInt(response["count"]);
                    commentController.AddComment(response["comment"]);
                    SetCount(CommentCount, detailsSource.commentscount);
                }
            }
        }

        private void SetCount(Text text, int count)
        {
            if (count < 1000)
            {
                text.text = count.ToString();
            }
            else if (count < 10000)
            {
                count = count / 100;
                var fract = count % 10;
                count = count / 10;
                text.text = count + "." + fract + "k";
            }
        }

		private void Like ()
		{
            if (detailsSource.liked) return;
            var url = CanvasConstants.WhatsNewSURL + "?request=like&id=" + detailsSource.id;
			StartCoroutine(Utils.PostRequest(url, true, true, HandleLikeResponse));
		}

        private void HandleLikeResponse(object sender, System.EventArgs e)
        {
            if (sender != null)
            {
                var www = sender as WWW;
                var response = new JSONObject(www.text);
                if (response != null)
                {
                    detailsSource.likescount = Utils.GetInt(response);
                    detailsSource.liked = true;
                    LikeButton.transform.Find("Image").GetComponent<ImageToggle>().SetSprite(false);
                    SetCount(LikeCount, detailsSource.likescount);
                }
            }
        }
		
		void OpenURL(string link)
		{
			Application.OpenURL(link);
		}

        public void LoadContent()
        {
            SetCount(LikeCount, detailsSource.likescount);
            SetCount(CommentCount, detailsSource.commentscount);
            LikeButton.transform.Find("Image").GetComponent<ImageToggle>().SetSprite(!detailsSource.liked);

            // set Main image at the top
            if (!string.IsNullOrEmpty(detailsSource.backgroundImageURL))
            {
                StartCoroutine(LoadImage(true, detailsSource.backgroundImageURL));
            }
            // Set title
            if (!string.IsNullOrEmpty(detailsSource.title)) Title.text = detailsSource.title;
            // Set description
            if (!string.IsNullOrEmpty(detailsSource.description)) Description.text = detailsSource.description;
            // If video url doesn't exists, delete the game object
            if (string.IsNullOrEmpty(detailsSource.videoURL))
            {
                try
                {
                    Destroy(VideoButton.gameObject);
                }
                catch { }
            }
            // If details image doesn't exitst delete the game object
            if (!string.IsNullOrEmpty(detailsSource.imageURL))
            {
                StartCoroutine(LoadImage(false, detailsSource.imageURL));
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
            if (string.IsNullOrEmpty(detailsSource.linkURL))
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