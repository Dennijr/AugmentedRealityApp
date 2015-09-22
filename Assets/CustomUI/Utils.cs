using UnityEngine;
using System.Collections;
using System;

namespace CustomUI
{
    public static class Utils
    {
        public static int GetInt(JSONObject obj)
        {
            if (obj != null)
            {
                try
                {
                    if (obj.IsNumber)
                        return (int)obj.f;
                    else if (obj.IsString)
                    {
                        int value = 0;
                        int.TryParse(obj.str, out value);
                        return value;
                    }
                }
                catch { }
            }
            return 0;
        }

        public static float GetFloat(JSONObject obj)
        {
            if (obj != null)
            {
                try
                {
                    if (obj.IsNumber)
                        return obj.f;
                    else if (obj.IsString)
                    {
                        float value = 0;
                        float.TryParse(obj.str, out value);
                        return value;
                    }
                }
                catch { }
            }
            return 0f;
        }

        public static DateTime UnixTimeStampToDateTime(float unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        public static IEnumerator PostRequest(string serviceURL, bool showLoading, bool requiresToken, EventHandler responseHandler)
        {
            Debug.Log(serviceURL);
            if (showLoading) CanvasConstants.ShowLoading(true);
            WWW www = new WWW(serviceURL);
            yield return www;
            if (showLoading) CanvasConstants.ShowLoading(false);
            if (responseHandler != null)
            {
                responseHandler(www, new EventArgs());
            }
        }
    }
}