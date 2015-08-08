﻿using UnityEngine;
using System.Collections;
using MaterialUI;
using System;

namespace CustomUI
{
    public class HomePage : BasePage
    {
        private DateTime firstBackKeyPress;

        void Start()
        {
            firstBackKeyPress = DateTime.Today;
        }

        public override void OnBackKeyPress(CancellationEventArgs e)
        {
            e.CancelEvent = true;
            if (firstBackKeyPress.AddSeconds(2) > DateTime.Now)
            {
                Application.Quit();
            }
            else
            {
                firstBackKeyPress = DateTime.Now;
                PagesManager.DisplayToast("Press Back again to exit");
            }
        }
    }
}