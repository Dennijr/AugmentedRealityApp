using UnityEngine;
using System.Collections;
using System;

namespace CustomUI
{
	// Source for whatsnew comment
	[System.Serializable]
	public class WhatsNewCommentSource : IBaseListSource
	{
		public int id;
		public DateTime createdTimeStamp;
		public string createdby;
		public int whatsnewid;
		public string comment;

        public WhatsNewCommentSource() { }

        public WhatsNewCommentSource(JSONObject obj)
        {
            try
            {
                this.id = Utils.GetInt(obj["id"]);
                this.createdTimeStamp = Utils.UnixTimeStampToDateTime(Utils.GetFloat(obj["createdtimestamp"]));
                this.createdby = obj["created_by"].str;
                this.whatsnewid = Utils.GetInt(obj["whatsnewid"]);
                this.comment = obj["comment"].str;
            }
            catch (Exception any)
            {
                Debug.Log("Whats new comments parse error : " + any.Message);
            }
        }
	}
}