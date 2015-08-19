using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CustomUI
{
    public class MyPicsListController : BaseListController<MyPicsModel,MyPicsListSource>
    {
        string path;
        string[] files;
        List<MyPicsListSource> source;

        public override void Start()
        {
            base.Start();
            source = new List<MyPicsListSource>();
            path = FileManager.GetScreenShotPath();
        }

        public void LoadImages()
        {
            StartCoroutine(loadImages());
        }

        private IEnumerator loadImages()
        {
            files = System.IO.Directory.GetFiles(path, "*.png");
            Debug.Log(path);
            string pathPreFix = FileManager.GetFilePrefixPath();

            foreach (string tstring in files)
            {
                MyPicsListSource thisImg = new MyPicsListSource();
                string pathTemp = pathPreFix + tstring;
                WWW www = new WWW(pathTemp);
                yield return www;
                Texture2D texTmp = new Texture2D(130, 180, TextureFormat.DXT1, false);
                www.LoadImageIntoTexture(texTmp);
                www.Dispose();
                www = null;
                thisImg.contentImage = texTmp;
                AddItem(thisImg);
            }
        }
    }
}