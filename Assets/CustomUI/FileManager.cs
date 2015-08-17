using UnityEngine;
using System.Collections;
using System;

namespace CustomUI
{
    public static class FileManager
    {
        public static string GetScreenShotPath()
        {
            return Application.persistentDataPath + "/";
        }

        /// <summary>
        /// Get file name to store screen shot. Return full path
        /// </summary>
        /// <returns></returns>
        public static string GetScreenShotFileName()
        {
            string path = Application.persistentDataPath;
            //Note in android and IOs, Application.CaptureScreenshot appends the filename to Application.persistentDataPath
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                path = "";
            }
            return path + "/Screenshot_" + DateTime.Now.ToString("yyyyMMdd_HHmmssff") + ".png";
        }

        public static string GetFilePrefixPath()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return @"file://";
            }
            return @"file: //";
        }
    }
}