using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using UnityEngine.UI;

namespace CustomUI
{
    public class MyPicsPage : BasePage
    {
        public GameObject ImagePanel;

        GameObject[] gameObj;
        string[] files;

        public override void Start()
        {
            base.Start();
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string path = FileManager.GetScreenShotPath();
            files = System.IO.Directory.GetFiles(path, "*.png");
            gameObj = GameObject.FindGameObjectsWithTag("MyPicImg");
            StartCoroutine(LoadImages());
        }

        private IEnumerator LoadImages()
        {
            //load all images in default folder as textures and apply dynamically to plane game objects.
            //6 pictures per page
            string pathPreFix = FileManager.GetFilePrefixPath();
            int dummy = 0;
            foreach (string tstring in files)
            {
                if (dummy == gameObj.Length) break;
                Debug.Log(tstring);
                string pathTemp = pathPreFix + tstring;
                WWW www = new WWW(pathTemp);
                yield return www;
                Texture2D texTmp = new Texture2D(130, 180, TextureFormat.DXT1, false);
                www.LoadImageIntoTexture(texTmp);
                www.Dispose();
                www = null;

                gameObj[dummy].GetComponent<RawImage>().texture = texTmp;//SetTexture("_MainTex", texTmp);
                dummy++;
            }
        }
    }
}