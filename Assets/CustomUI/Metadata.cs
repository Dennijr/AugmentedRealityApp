using UnityEngine;
using System.Collections;
using System;

namespace CustomUI
{
    public class Metadata
    {
        public int id;
        public string name;
        public string type;
        public DateTime created;
        public string description;
        public string materialurl;
        public string textureurl;
        public string facebooklink;
        public string twitterlink;
        public string hyperlink;

        public Metadata()
        {
            Init();
        }

        private void Init()
        {
            this.name = string.Empty;
            this.type = string.Empty;
            this.description = string.Empty;
            this.materialurl = string.Empty;
            this.textureurl = string.Empty;
            this.facebooklink = string.Empty;
            this.twitterlink = string.Empty;
            this.hyperlink = string.Empty;
        }

        public Metadata(JSONObject metadata)
        {
            this.Init();
            if (metadata["id"] != null)
            {
                try
                {
                    this.id = (int)metadata["id"].f;
                }
                catch { }
            }
            if (metadata["name"] != null) this.name = metadata["name"].str;
            if (metadata["type"] != null) this.type = metadata["type"].str;
            if (metadata["description"] != null) this.description = metadata["description"].str;
            if (metadata["materialurl"] != null) this.materialurl = metadata["materialurl"].str.Replace("\\", "");
            if (metadata["textureurl"] != null) this.textureurl = metadata["textureurl"].str.Replace("\\", "");
            if (metadata["facebooklink"] != null) this.facebooklink = metadata["facebooklink"].str.Replace("\\", "");
            if (metadata["twitterlink"] != null) this.twitterlink = metadata["twitterlink"].str.Replace("\\", "");
            if (metadata["hyperlink"] != null) this.hyperlink = metadata["hyperlink"].str.Replace("\\", "");

            if (metadata["imagelocation"] != null)
            {
                this.type = metadata["description"].str;
                this.materialurl = metadata["imagelocation"].str.Replace("\\", "");
                if (metadata["texturelocation"] != null) this.textureurl = metadata["texturelocation"].str.Replace("\\", "");
            }
        }
    }
}