using UnityEngine;

public interface IGrabQueryable
{
    IGrabbable GetGrabbable();
}

public interface IGrabbable
{
    GameObject GetGrabbedObject();
    Vector3 GetGrabOffset();
    void Grab();
    void Release(Vector2 velocity);
    float GetMassRatio();
}