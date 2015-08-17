using UnityEngine;
using System.Collections;
using System;

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
}