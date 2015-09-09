using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CustomUI
{
	public class WhatsNewCommentModel : BaseModel<WhatsNewCommentSource>
	{
		public int id;
		public Text comment;
		public GameObject wrapper;

		public override void Copy (WhatsNewCommentSource source)
		{
			base.Copy (source);
			this.id = source.id;
			this.comment.text = source.comment;
		}

		public override IEnumerator LoadModel (WhatsNewCommentSource source)
		{
			yield return null;
			var layoutElement = this.wrapper.GetComponent<LayoutElement> ();
			var rectTransform = this.comment.gameObject.GetComponent<RectTransform> ();
			var height = rectTransform.sizeDelta.y;
			layoutElement.preferredHeight = height + 20;
		}
	}
}