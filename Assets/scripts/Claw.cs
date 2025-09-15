using UnityEngine;

public class Claw : MonoBehaviour
{
    public float openAngle = 45f;
    public float offsetAngle = 15f;
    public float openDuration = 0.6f;
    bool isOpen = true;
    GameObject leftPart;
    GameObject rightPart;
    Tween openTween;
    float currentAngle;

    void Start()
    {
        leftPart = transform.Find("claw_left").gameObject;
        rightPart = transform.Find("claw_right").gameObject;
        currentAngle = isOpen ? openAngle : 0f;
        openTween = CreateOpenTween(openAngle, openAngle);
    }

    Tween CreateOpenTween(float from, float to)
    {
        //Easing bez = Easings.CubicBezier(0.00f, 0.00f, 0.58f, 1.00f);
        Easing bez = Easings.CubicBezier(.83f, 0f, .64f, 1.41f);
        return new Tween(from, to, openDuration, OnTweenUpdate, OnTweenComplete, easing: /*Easings.EaseIn*/ bez);
    }

    void Update()
    {
        openTween.Update(Time.deltaTime);
    }

    void OnTweenComplete()
    {
        //Debug.Log("Tween complete");
    }

    void OnTweenUpdate(float value)
    {
        currentAngle = value;
        leftPart.transform.localRotation = Quaternion.Euler(0f, 0f, -(currentAngle + offsetAngle));
        rightPart.transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle + offsetAngle);
    }

    public void SetOpen(bool open)
    {
        if (isOpen != open)
        {
            isOpen = open;
            openTween = CreateOpenTween(currentAngle, isOpen ? openAngle : -openAngle);
        }
    }

    // Print on collision with another object
    void OnCollisionEnter2D(Collision2D c)
    {
        Debug.Log($"Claw collision with {c.gameObject.name} {c.collider.name} {c.otherCollider.name}");
        //Debug.Log($"this body: {c.rigidbody?.name}, this collider: {c.collider?.name}");
        //Debug.Log($"other body: {c.otherRigidbody?.name}, other collider: {c.otherCollider?.name}");
    }

    void OnCollisionExit2D(Collision2D c)
    {
        Debug.Log($"Claw collision exit with {c.gameObject.name} {c.collider.name} {c.otherCollider.name}");
    }
}
