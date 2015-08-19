using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace CustomUI
{
    // List view controller
    // MVC pattern is used here
    // This is the controller part. All the actions to the list are controlled throught this base class
    public class BaseListController<M, S> : MonoBehaviour
        where M : BaseModel<S>
        where S : IBaseListSource
    {
        // List template element which is going to be repeated
        // Normally this is a prefab found in the folder views
        public GameObject view;
        // The panel to which the list items are added
        public Transform parentPanel;

        public virtual void Start()
        {

        }

        public bool AddItem(S source)
        {
            try
            {
                GameObject newElement = Instantiate(view) as GameObject;
                M newModel = newElement.GetComponent<M>();
                newModel.Copy(source);
                newModel.transform.SetParent(parentPanel);
                newModel.transform.localScale = new Vector3(1, 1, 1);
                return true;
            }
            catch { }
            return false;
        }

        public bool AddItems(List<S> sources)
        {
            foreach (var source in sources)
                AddItem(source);
            return true;
        }
    }
}