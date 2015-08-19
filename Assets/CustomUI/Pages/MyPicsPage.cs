using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using UnityEngine.UI;

namespace CustomUI
{
    public class MyPicsPage : BasePage
    {
        //List controller for this page attached to this game object
        MyPicsListController listController;

        public override void Start()
        {
            base.Start();
            listController = gameObject.GetComponent<MyPicsListController>();
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            listController.LoadImages();
        }
    }
}