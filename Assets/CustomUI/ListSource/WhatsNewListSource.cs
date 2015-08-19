using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace CustomUI
{
    // This is the source for whats new
    // Now making this populatable throught Inspector
    [System.Serializable]
    public class WhatsNewListSource : IBaseListSource
    {
        public Texture contentImage;
        public string mainContent;
        public string subContent;
    }
}