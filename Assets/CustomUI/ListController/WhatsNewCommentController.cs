using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CustomUI
{
	public class WhatsNewCommentController : BaseListController<WhatsNewCommentModel, WhatsNewCommentSource> 
	{
		private bool loaded = false;
        private int whatsNewId;

		public void LoadWhatsNewComments(int whatsNewId) 
		{
			if (!loaded)
				StartCoroutine(GetAllWhatsNewComments(whatsNewId));
		}

        public void AddComment(string text)
        {
            var comment = new WhatsNewCommentSource();
            comment.id = 0;
            comment.whatsnewid = whatsNewId;
            comment.comment = text;
            source.Add(comment);
            AddItem(comment);
        }

		private IEnumerator GetAllWhatsNewComments (int whatsNewId)
		{
            this.whatsNewId = whatsNewId;
			WWW www = new WWW (CanvasConstants.serverURL + "?request=getcomment&id=" + whatsNewId);
            CanvasConstants.ShowLoading(true);
			yield return www;
            CanvasConstants.ShowLoading(false);

			Debug.Log ("Response: " + www.text);
			if (www.text != null) {
				var response = new JSONObject (www.text).list;
				foreach (JSONObject comment in response) {
					try {
						var item = new WhatsNewCommentSource ();
						int id = 0;
						int.TryParse (comment ["id"].str, out id);
						item.id = id;
						item.comment = comment["comment"].str;
						item.whatsnewid = whatsNewId;
						item.createdby = comment["created_by"].str;
						double createdTimeStamp;
						if (double.TryParse (comment ["createdtimestamp"].str, out createdTimeStamp))
							item.createdTimeStamp = CanvasConstants.UnixTimeStampToDateTime (createdTimeStamp);
						source.Add (item);
					} catch (System.Exception e) {
						Debug.Log ("Exception: " + e.ToString ());
					}
				}
			}
			AddItems (source);
            ListContentChanged();
			loaded = true;
		}
	}
}