//  Copyright 2014 Invex Games http://invexgames.com
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MaterialUI
{
	public static class ToastControl
	{
        public static GameObject toastPrefab;
		public static string toastText;
		public static float toastDuration;
		public static Color toastPanelColor;
		public static Color toastTextColor;
		public static int toastFontSize;
		public static Canvas parentCanvas;

        static List<GameObject> allToasts = new List<GameObject>();

        public static void AddToast(GameObject toast)
        {
            allToasts.Add(toast);
        }

        public static void RemoveLastToast()
        {
            try
            {
                allToasts.RemoveAt(allToasts.Count - 1);
            }
            catch { }
        }

        public static void HideAllToasts()
        {
            foreach (var toast in allToasts)
                toast.GetComponent<ToastAnim>().HideToast();
        }
	}
}