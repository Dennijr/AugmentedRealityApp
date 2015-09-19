using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Soomla.Profile;

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

		public Button LikeButton;
        public Text LikeCount;
		public Button CommentButton;
        public Text CommentCount;

		public GameObject CommentPopup;
		public GameObject SharePopup;
		public InputField CommentInput;

        private WhatsNewCommentController commentController;

        private int id, likecount, commentcount;
        private string title, description, videoLink, shareLink, readMoreLink, imageURL;

		public void Start()
        {
			VideoButton.onClick.AddListener(delegate {
				if (!string.IsNullOrEmpty(videoLink)) OpenURL(videoLink);
			});

			ReadMoreButton.onClick.AddListener (delegate {
                if (!string.IsNullOrEmpty(readMoreLink)) OpenURL(readMoreLink);
			});

			ShareButton.onClick.AddListener (delegate {
				ShowSharePopup();
			});

            EmailButton.onClick.AddListener(() => PushEmail());

			LikeButton.onClick.AddListener (() => Like ());

			CommentButton.onClick.AddListener(() => ShowCommentPopup());

			ScanButton.onClick.AddListener(() => { CanvasConstants.Navigate("Scan"); });
		}

		private static IEnumerator PostUserToServer(UserProfile userProfile)
		{
			Debug.Log ("Posting data: ");
			string url = string.Format (CanvasConstants.userServerURL + "?request=adduser&provider={0}&provider_user_id={1}&name={2}&email={3}", userProfile.Provider, userProfile.ProfileId, userProfile.FirstName, userProfile.Email);
			Debug.Log("Posting user data: " + url );
			WWW www = new WWW (url);
			yield return www;
		}

		public void UpadateStory ()
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
			SoomlaProfile.UpdateStory (Provider.FACEBOOK, title, title, title, description, readMoreLink, imageURL, "", null);
			CloseSharePopup();
		}

		public void UpadateTwitterStory ()
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
			SoomlaProfile.UpdateStory (Provider.TWITTER, title, title, title, description, readMoreLink, imageURL, "", null);
			CloseSharePopup();
		}

		
		void OnGUI()
		{
			if(CommentInput.isFocused && CommentInput.text != "" && Input.GetKey(KeyCode.Return)) {
				PostComment();
			}
		}


		public void ShowCommentPopup()
		{
			if (CommentPopup == null)
				return;
			commentController = CommentPopup.GetComponent<WhatsNewCommentController>();
			Enable (CommentPopup);
			if (commentController != null)
				commentController.LoadWhatsNewComments (id);
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
			CanvasConstants.ShowLoading(false);
			Disable(SharePopup, false);
		}
		
		public void PostComment()
		{
			var comment = CommentInput.text;
			if (!string.IsNullOrEmpty (comment)) {
				StartCoroutine(PostCommentToServer(comment));
			}
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
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
                AppSocial.SendEmail(title, description);
        }

		private IEnumerator PostLike()
		{
			WWW www = new WWW (CanvasConstants.serverURL + "?request=like&id=" + id );
			yield return www;
            if (www != null)
            {
                LikeButton.transform.Find("Image").GetComponent<ImageToggle>().SetSprite(false);
                SetCount(LikeCount, ++likecount);
            }
		}

		private IEnumerator PostCommentToServer(string comment)
		{
			string url = CanvasConstants.serverURL + "?request=comment&id=" + id + "&comment=" + WWW.EscapeURL(comment);
			Debug.Log("Posting comment: " +url );
			WWW www = new WWW (url);
			yield return www;
            if (www != null)
            {
                CommentInput.text = "";
                commentController.AddComment(comment);
                SetCount(CommentCount, ++likecount);
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
			StartCoroutine (PostLike());
		}
		
		void OpenURL(string link)
		{
			Application.OpenURL(link);
		}

        public void LoadContent(WhatsNewListSource source)
        {
			id = source.id;
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
				imageURL= source.imageURL;
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