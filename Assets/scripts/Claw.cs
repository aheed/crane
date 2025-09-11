using UnityEngine;

public class Claw : MonoBehaviour
{
    public float openAngle = 45f;
    bool isOpen = true;
    GameObject leftPart;
    GameObject rightPart;

    void Start()
    {
        leftPart = transform.Find("claw_left").gameObject;
        rightPart = transform.Find("claw_right").gameObject;
        UpdateAppearance();
    }

    public void SetOpen(bool open)
    {
        if (isOpen != open)
        {
            isOpen = open;
            UpdateAppearance();
        }
    }

    void UpdateAppearance()
    {
        var leftRotation = isOpen ? -openAngle : openAngle;
        var rightRotation = isOpen ? openAngle : -openAngle;

        leftPart.transform.localRotation = Quaternion.Euler(0f, 0f, leftRotation);
        rightPart.transform.localRotation = Quaternion.Euler(0f, 0f, rightRotation);
    }
}
