using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomUI
{
    // List view controller
    // MVC pattern is used here
    // This is the controller part. All the actions to the list are controlled throught this base class
    public abstract class BaseListController<M, S> : MonoBehaviour
        where M : BaseModel<S>
        where S : IBaseListSource
    {
        // List template element which is going to be repeated
        // Normally this is a prefab found in the folder views
        public GameObject view;
        // The panel to which the list items are added
        public Transform parentPanel;
        // Model of the item clicked
		[HideInInspector]
        public M currentModel;
        // Event handler when the list item is clicked
        public event EventHandler ListItemClickedHandler;
        // When no item found text to display
        public GameObject NoContentText;
        // List of source items
        [HideInInspector]
        public List<S> source = new List<S>();

        public virtual void Start()
        {
            ShowNoContent(false);
        }

        public bool AddItem(S source)
        {
            try
            {
                ShowNoContent(false);
                GameObject newElement = Instantiate(view) as GameObject;
                M newModel = newElement.GetComponent<M>();
                newModel.Copy(source);
                newModel.thisButton.onClick.AddListener(() => ListItemClicked(newModel));
                newModel.transform.SetParent(parentPanel);
                newModel.transform.localScale = new Vector3(1, 1, 1);
				StartCoroutine(newModel.LoadModel(source));
                return true;
            }
            catch { }
            return false;
        }

        public virtual void ListItemClicked(M model)
        {
            currentModel = model;
            if (currentModel != null && ListItemClickedHandler != null)
            {
                ListItemClickedHandler(this, new EventArgs());
            }
        }

        public bool AddItems(List<S> sources)
        {
            foreach (var source in sources)
                AddItem(source);
            return true;
        }

        public int GetCount()
        {
            return parentPanel.childCount;
        }

		public bool RemoveAllItems()
		{
			try {
				for (var i = 0; i < parentPanel.childCount; i++)
				{
					var item = parentPanel.GetChild(i);
					if (item != null)
					{
						item.transform.parent = null;
						Destroy(item);
					}
				}
                ListContentChanged();
				return true;
			}
			catch {}
			return false;
		}

        protected void ListContentChanged()
        {
            ShowNoContent(source.Count == 0);
        }

        public void ShowNoContent(bool show)
        {
            if (NoContentText != null) NoContentText.SetActive(show);
        }
	}
}