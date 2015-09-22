using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CustomUI
{
	public class WhatsNewCommentController : BaseListController<WhatsNewCommentModel, WhatsNewCommentSource> 
	{
		private bool loaded = false;
        private int whatsNewId;

		public void LoadWhatsNewComments(int id) 
		{
			if (!loaded) {
                whatsNewId = id;
                var url = CanvasConstants.WhatsNewSURL + "?request=getcomment&id=" + whatsNewId;
                StartCoroutine(Utils.PostRequest(url, true, false, HandleCommentsResponse));
            }
		}

        public void AddComment(JSONObject obj)
        {
            var comment = new WhatsNewCommentSource(obj);
            source.Add(comment);
            AddItem(comment);
            ListContentChanged();
        }

        private void HandleCommentsResponse(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var www = sender as WWW;
                var response = new JSONObject(www.text);
                if (response != null && response.Count > 0)
                {
                    var comments = response.list;
                    foreach (var comment in comments)
                    {
                        var item = new WhatsNewCommentSource(comment);
                        source.Add(item);
                    }
                }
            }
            AddItems(source);
            ListContentChanged();
            loaded = true;
        }
	}
}