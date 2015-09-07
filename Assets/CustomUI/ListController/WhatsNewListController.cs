using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomUI
{
    // List controller for WhatsNew. Look at base class for more info
    public class WhatsNewListController : BaseListController<WhatsNewModel, WhatsNewListSource>
    {
		public List<WhatsNewListSource> source = new List<WhatsNewListSource>();

		public void ReloadWhatsNewContent() 
		{
			StartCoroutine(GetAllWhatsNewItems());

		}

		public IEnumerator GetAllWhatsNewItems() 
		{

			WWW www = new WWW(CanvasConstants.serverURL + "?request=get");
			yield return www;

			Debug.Log ("Response: " + www.text);
			if (www.text != null) {
				var response = new JSONObject (www.text).list;
				foreach (JSONObject whatsnew in response) {
					try {
						var item = new WhatsNewListSource ();
						int id = 0;
						int.TryParse (whatsnew ["id"].str, out id);
						item.id = id;

						var categoryId = 0;
						int.TryParse (whatsnew ["category_id"].str, out categoryId);
						item.categoryId = categoryId;
						item.title = whatsnew ["title"].str;
						item.description = whatsnew ["description"].str;
						item.imageURL = whatsnew ["imageurl"].str.Replace ("\\", "");
						item.backgroundImageURL = whatsnew ["backgroundimageurl"].str.Replace ("\\", "");
						item.videoURL = whatsnew ["videourl"].str.Replace ("\\", "");
						item.linkURL = whatsnew ["linkurl"].str.Replace ("\\", "");
						double createdTimeStamp, lastModifiedTimeStamp;
						if (double.TryParse (whatsnew ["createdtimestamp"].str, out createdTimeStamp))
							item.createdTimeStamp = CanvasConstants.UnixTimeStampToDateTime (createdTimeStamp);
						if (double.TryParse (whatsnew ["lastmodifiedtimestamp"].str, out lastModifiedTimeStamp))
							item.lastModifiedTimeStamp = CanvasConstants.UnixTimeStampToDateTime (lastModifiedTimeStamp);
					
						source.Add (item);
					} catch (System.Exception e) {
						Debug.Log ("Exception: " + e.ToString ());
					}
				}
				AddItems (source);
			}
		}

		public WhatsNewListSource GetSource(int id)
		{
			var thisSource = source.FirstOrDefault (p => p.id == id);
			return thisSource;
		}
	}
}