using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
    // MVC patter
    // This is the template for all Models.
    // There shouldn't be a contructor as this will instantiated from the view (prefab game object)
    public class BaseModel<S> : MonoBehaviour where S : IBaseListSource
    {
        public Button thisButton;

        public virtual void Copy(S source)
        {

        }
    }
}