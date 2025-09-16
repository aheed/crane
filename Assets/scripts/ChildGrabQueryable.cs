
using UnityEngine;

public class ChildGrabQueryable : MonoBehaviour, IGrabQueryable
{
    public IGrabbable GetGrabbable()
    {
        var parent = transform.parent.gameObject;
        var grabbable = parent.GetComponent<IGrabbable>();
        return grabbable;
    }
}
