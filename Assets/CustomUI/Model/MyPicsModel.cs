using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CustomUI
{
    public class MyPicsModel : BaseModel<MyPicsListSource>
    {
        public RawImage contentImage;

        public override void Copy(MyPicsListSource source)
        {
            base.Copy(source);
            this.contentImage.texture = source.contentImage;
        }
    }
}