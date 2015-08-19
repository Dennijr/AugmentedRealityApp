using UnityEngine;
using System.Collections.Generic;
using MaterialUI;

namespace CustomUI
{
    public class WhatsNewPage : BasePage
    {
        // List of items to be displayed in the whatsnew page
        // For now making this public so that it can be populated through inspector
        public List<WhatsNewListSource> source;

        // List controller for whatsnew. This script is attached to this game object.(Whatsnew page)
        // Get a reference for it to interact with it
        WhatsNewListController listController;

        public override void Start()
        {
            base.Start();
            listController = gameObject.GetComponent<WhatsNewListController>();
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (source != null && source.Count > 0 && listController != null)
            {
                listController.AddItems(source);
            }
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
    }
}