using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace CustomUI
{
    // This is the source for whats new
    // Now making this populatable throught Inspector
    [System.Serializable]
    public class WhatsNewListSource : IBaseListSource
    {
		public int id;
        public int likescount;
        public int commentscount;
        public bool liked;
        public string title;
        public string description;
		public string imageURL;
		public string backgroundImageURL;
		public string videoURL;
		public string linkURL;

        public WhatsNewListSource() { }

        public WhatsNewListSource(JSONObject whatsnew)
        {
            try
            {
                this.id = Utils.GetInt(whatsnew["id"]);
                this.likescount = Utils.GetInt(whatsnew["likescount"]);
                this.commentscount = Utils.GetInt(whatsnew["commentscount"]);
                this.liked = Utils.GetInt(whatsnew["liked"]) > 0;
                this.title = whatsnew["title"].str;
                this.description = whatsnew["description"].str;
                this.imageURL = whatsnew["imageurl"].str.Replace("\\", "");
                this.backgroundImageURL = whatsnew["backgroundimageurl"].str.Replace("\\", "");
                this.videoURL = whatsnew["videourl"].str.Replace("\\", "");
                this.linkURL = whatsnew["linkurl"].str.Replace("\\", "");
            }
            catch (Exception any)
            {
                Debug.Log("Whats new item parse error : " + any.Message);
            }
        }
    }
}