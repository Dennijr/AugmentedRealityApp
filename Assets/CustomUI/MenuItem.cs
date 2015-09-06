using UnityEngine;
using System;
using UnityEngine.UI;
using MaterialUI;

namespace CustomUI
{
    public class MenuItem : MonoBehaviour
    {
        private Image thisImage;
        private Color activeColor, normalColor;

        void Start()
        {
            thisImage = gameObject.GetComponent<Image>();
            Color.TryParseHexString("#0A0A0AFF", out activeColor);
            Color.TryParseHexString("#212121FF", out normalColor);
        }

        public void UnsetActive()
        {
            if (thisImage != null)
            {
                thisImage.color = normalColor;
            }
        }

        public void SetActive()
        {
            if (thisImage != null)
            {
                thisImage.color = activeColor;
            }
        }
    }
}