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
	}
}