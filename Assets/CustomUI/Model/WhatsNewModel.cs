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
		public int id;

        public override void Copy(WhatsNewListSource source)
        {
            base.Copy(source);
			this.id = source.id;
            this.mainContent.text = source.title;
            this.subContent.text = source.description;
        }

		public override IEnumerator LoadModel(WhatsNewListSource source)
		{
			WWW www = new WWW(source.backgroundImageURL);
			
			yield return www;
			
			Texture2D texture = new Texture2D (1, 1);

			www.LoadImageIntoTexture(texture);
			backgroundImage.texture = texture;

			www.Dispose();
			www = null;

		}
	}
}