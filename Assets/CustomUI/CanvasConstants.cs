using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class CanvasConstants : MonoBehaviour
{
    public Canvas canvas;

    public static event EventHandler resizeHandler;
    public static string appName = "ActivateAR";
    public static string screenShotPath;

    [HideInInspector]
    public static float canvasWidth;
    [HideInInspector]
    public static float canvasHeight;

    private static RectTransform canvasRect;
	[HideInInspector]
	public static string serverURL = @"http://192.168.1.34/activatear/WhatsNewService.php?timestamp=2015-09-01";

    // Use this for initialization
    void Start()
    {
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
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

	public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
	{
		// Unix timestamp is seconds past epoch
		DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0,DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
		return dtDateTime;
	}
}