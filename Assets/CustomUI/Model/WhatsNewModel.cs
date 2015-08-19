using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CustomUI
{
    public class WhatsNewModel : BaseModel<WhatsNewListSource>
    {
        public RawImage backgroundImage;
        public Text mainContent;
        public Text subContent;

        public override void Copy(WhatsNewListSource source)
        {
            base.Copy(source);
            this.backgroundImage.texture = source.contentImage;
            this.mainContent.text = source.mainContent;
            this.subContent.text = source.subContent;
        }
    }
}