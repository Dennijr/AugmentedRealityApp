using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace CustomUI
{
	public class CanvasConstants : MonoBehaviour
	{
	    public Canvas canvas;

	    public static float ReferenceWidth = 360;
	    public static float ReferenceHeight = 640;

	    public static event EventHandler resizeHandler;
	    public static string appName = "ActivateAR";
	    public static string screenShotPath;

	    public GameObject ARCamera;
	    public GameObject MainCamera;
		public PagesManager PagesManager;
        public GameObject LoadingIndicator;

	    private static GameObject _ARCamera;
	    private static GameObject _MainCamera;
		private static PagesManager _PagesManager;
        private static GameObject _LoadingIndicator;

	    [HideInInspector]
	    public static float canvasWidth;
	    [HideInInspector]
	    public static float canvasHeight;

	    private static RectTransform canvasRect;

        private static string SURL = @"http://192.168.24.1/activatear/services/";
        //private static string SURL = @"http://studio.activatear.com/services/";
		[HideInInspector]
        public static string WhatsNewSURL = SURL + "whatsnew.php";
		[HideInInspector]
		public static string UserSURL = SURL + "users.php";

	    // Use this for initialization
	    void Start()
	    {
	        if (canvas != null)
	        {
	            canvasRect = canvas.GetComponent<RectTransform>();
	        }
	        if (ARCamera != null) _ARCamera = ARCamera;
	        if (MainCamera != null) _MainCamera = MainCamera;
			if (PagesManager != null) _PagesManager = PagesManager;
            if (LoadingIndicator != null) _LoadingIndicator = LoadingIndicator;
	        screenShotPath = Application.persistentDataPath;
	        Debug.Log(screenShotPath);
	    }

	    void Update()
	    {
	        var newCanvasWidth = canvasRect != null ? canvasRect.rect.width : Screen.width;
	        var newCanvasHeight = canvasRect != null ? canvasRect.rect.height : Screen.height;
	        if (canvasWidth != newCanvasWidth || canvasHeight != newCanvasHeight)
	        {
	            canvasWidth = newCanvasWidth;
	            canvasHeight = newCanvasHeight;
	            if (resizeHandler != null) resizeHandler(this, new EventArgs());
	            Debug.Log("Change in canvas size detected : " + canvasWidth + " , " + canvasHeight);
	        }
	    }

        public static void Navigate(string page)
        {
            if (_PagesManager != null)
            {
                _PagesManager.Navigate(page);
            }
        }

	    public static void SetARCamera(bool on) 
	    {
	        if (_ARCamera == null || _MainCamera == null) return;
	        if (on)
	        {
	            _MainCamera.SetActive(false);
	            _ARCamera.SetActive(true);
	        }
	        else
	        {
	            _ARCamera.SetActive(false);
	            _MainCamera.SetActive(true);
	        }
	    }

        public static void ShowLoading(bool on)
        {
            if (_LoadingIndicator != null) _LoadingIndicator.SetActive(on);
        }

		public static IEnumerator LoadRemoteImage(RawImage image, string url)
		{
			WWW www = new WWW(url);

			yield return www;

			Texture2D texture = new Texture2D (1, 1);
				
			texture.LoadImage (www.bytes);
			image.texture = texture;
			www.Dispose();
			www = null;
		}

		public static IEnumerator GetResponse(string url)
		{
			WWW www = new WWW(url);
			
			yield return www;

		
		}
	}
}