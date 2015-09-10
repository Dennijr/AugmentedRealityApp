using UnityEngine;
using System.Collections.Generic;
using MaterialUI;
using System.Collections;

namespace CustomUI
{
    public class WhatsNewPage : BasePage
    {
        // List of whats new item
        public GameObject ListSection;
        // Details about the item
        public GameObject DetailsSection;
		// Whats new details shower
        public GameObject WhatsNewDetailsPrefab;
        // This will populated at run time
        private WhatsNewDetails whatsNewDetails;

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
                if (listController != null)
                {
                    if (listController.GetCount() == 0) listController.ReloadWhatsNewContent();
                }

                ListSection.SetActive(true);
                DetailsSection.SetActive(false);
            }
        }

        public override void OnNavigatingFrom(NavigationEventArgs e)
        {
            base.OnNavigatingFrom(e);
            CanvasConstants.ShowLoading(false);
        }
	
		// Hide details section on back key press
		public override void OnBackKeyPress(CancellationEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (DetailsSection.activeSelf)
            {
                HideContentDetails();
                e.CancelEvent = true;
            }
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        public void ShowContentDetails(WhatsNewModel thisModel)
        {
			var contentsource = listController.GetSource (thisModel.id);
			if (contentsource != null) {
                var detailsObject = Instantiate(WhatsNewDetailsPrefab) as GameObject;
                DetailsSection.SetActive(true);
                detailsObject.transform.SetParent(DetailsSection.transform);
                // Set default scaling
                detailsObject.transform.localScale = new Vector3(1, 1, 1);
                // Set default margin
                var rectTransform = detailsObject.GetComponent<RectTransform>();
                rectTransform.offsetMax = new Vector2(0, 0);
                rectTransform.offsetMin = new Vector2(0, 0);
                // Activate the prefab with fading animation
                whatsNewDetails = detailsObject.GetComponent<WhatsNewDetails>();
                whatsNewDetails.Enable();
				whatsNewDetails.LoadContent (contentsource);
			}
        }

        public void HideContentDetails()
        {
            if (whatsNewDetails != null && whatsNewDetails.CanNavigateBack())
                whatsNewDetails.Disable();
        }
    }
}