using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CustomUI
{
    public class ImageToggle : MonoBehaviour
    {
        private Sprite OriginalImage;
        private Image thisImage;
        public Sprite ToggleImage;

        void Start()
        {
            init();
        }

        bool init()
        {
            if (thisImage != null && OriginalImage != null)
                return true;
            thisImage = gameObject.GetComponent<Image>();
            if (thisImage != null)
                OriginalImage = thisImage.sprite;
            return (thisImage != null && OriginalImage != null);
        }

        public void ToggleSprite()
        {
            if (init())
                SetSprite(thisImage.sprite != OriginalImage);
        }

        public void SetSprite(bool setOriginal)
        {
            if (init())
                thisImage.sprite = setOriginal ? OriginalImage : ToggleImage;
        }
    }
}