using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CustomUI
{
    // List controller for WhatsNew. Look at base class for more info
    public class WhatsNewListController : BaseListController<WhatsNewModel, WhatsNewListSource>
    {
		public void ReloadWhatsNewContent()
		{
            var url = CanvasConstants.WhatsNewSURL + "?request=get";
            StartCoroutine(Utils.PostRequest(url, true, false, HandleWhatsNewResponse));
		}

        private void HandleWhatsNewResponse(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var www = sender as WWW;
                var response = new JSONObject(www.text);
                Debug.Log(response);
                if (response != null && response.Count > 0)
                {
                    var whatsnewitems = response.list;
                    foreach(JSONObject whatsnew in whatsnewitems)
                    {
                        var item = new WhatsNewListSource(whatsnew);
                        source.Add(item);
                    }
                }
            }
            AddItems(source);
            ListContentChanged();
        }

		public WhatsNewListSource GetSource(int id)
		{
			var thisSource = source.FirstOrDefault (p => p.id == id);
			return thisSource;
		}
    }
}