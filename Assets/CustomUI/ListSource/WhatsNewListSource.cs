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
		public int categoryId;
        public string title;
        public string description;
		public string imageURL;
		public string backgroundImageURL;
		public string videoURL;
		public string linkURL;
		public DateTime createdTimeStamp;
		public DateTime lastModifiedTimeStamp;
    }
}