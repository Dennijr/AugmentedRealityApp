using UnityEngine;
using System.Collections.Generic;
using MaterialUI;

namespace CustomUI
{
    public class WhatsNewPage : BasePage
    {
        // List of whats new item
        public GameObject ListSection;
        // Details about the item
        public GameObject DetailsSection;
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
            if (listController != null) listController.ListItemClickedHandler += listController_ListItemClickedHandler;
        }

        public void listController_ListItemClickedHandler(object sender, System.EventArgs e)
        {
            ShowContentDetails(listController.currentModel);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.navigationType == NavigationType.New)
            {
                if (source != null && source.Count > 0 && listController != null)
                {
                    if (listController.GetCount() == 0) listController.AddItems(source);
                }
                ListSection.SetActive(true);
                DetailsSection.SetActive(false);
            }
        }

        // Hide details section on back key press
        public override void OnBackKeyPress(CancellationEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (DetailsSection.activeSelf)
            {
                DetailsSection.SetActive(false);
                e.CancelEvent = true;
            }
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        public void ShowContentDetails(WhatsNewModel thisModel)
        {
            DetailsSection.SetActive(true);
        }
    }
}