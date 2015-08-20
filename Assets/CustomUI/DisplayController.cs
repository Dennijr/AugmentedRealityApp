using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace CustomUI
{
    public class DisplayController : MonoBehaviour
    {
        public void Hide(bool onlyEventFromMe)
        {
            if (onlyEventFromMe)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log(EventSystem.current.currentSelectedGameObject);
                    gameObject.SetActive(false);
                }
            }
            else
            gameObject.SetActive(false);
        }
    }
}