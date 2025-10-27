using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Claw : MonoBehaviour
{
    static readonly float CompleteThresholdOverflow = 100000f;
    public float openAngle = 45f;
    public float offsetAngle = 15f;
    public float openDuration = 0.6f;
    public float grabOffsetY = -0.2f;
    public float openCompleteFraction = 0.9f;
    bool isOpen = true;
    GameObject leftPart;
    GameObject rightPart;
    Tween openTween;
    float currentAngle;
    Collider2D grabDetectorCollider;
    Action onOpenCallback;
    Action onCloseCallback;
    Action<Collision2D> onMarbleCollisionCallback;
    private float openCompleteThreshold = CompleteThresholdOverflow;

    public void Reset()
    {
        isOpen = true;
        currentAngle = isOpen ? openAngle : 0f;
        openTween = CreateOpenTween(openAngle, openAngle);
    }

    void Start()
    {
        leftPart = transform.Find("claw_left").gameObject;
        rightPart = transform.Find("claw_right").gameObject;
        Reset();
        grabDetectorCollider = transform.Find("claw_detector").GetComponent<Collider2D>();
    }

    Tween CreateOpenTween(float from, float to)
    {
        //Easing bez = Easings.CubicBezier(0.00f, 0.00f, 0.58f, 1.00f);
        Easing bez = Easings.CubicBezier(.83f, 0f, .64f, 1.41f);
        return new Tween(from, to, openDuration, OnTweenUpdate, OnTweenComplete, easing: /*Easings.EaseIn*/ bez);
    }

    void Update()
    {
        openTween?.Update(Time.deltaTime);
    }

    void OnTweenComplete()
    {
        Debug.Log("Tween complete");
    }

    void OnTweenUpdate(float value)
    {
        currentAngle = value;
        leftPart.transform.localRotation = Quaternion.Euler(0f, 0f, -(currentAngle + offsetAngle));
        rightPart.transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle + offsetAngle);
        var signMultiplier = isOpen ? 1 : -1;
        if (isOpen && currentAngle >= openCompleteThreshold)
        {
            openCompleteThreshold = CompleteThresholdOverflow;
            onOpenCallback?.Invoke();
        }
        else if (!isOpen && currentAngle <= openCompleteThreshold)
        {
            openCompleteThreshold = -CompleteThresholdOverflow;
            onCloseCallback?.Invoke();
        }
    }

    public void SetOpen(bool open)
    {
        if (isOpen != open)
        {
            isOpen = open;
            openTween = CreateOpenTween(currentAngle, isOpen ? openAngle : -openAngle);
            openCompleteThreshold = openAngle * (2 * openCompleteFraction - 1) * (isOpen ? 1 : -1);
        }
    }

    public bool IsOpen() => isOpen;

    public void SetOnOpenCallback(Action onOpen)
    {
        onOpenCallback = onOpen;
    }

    public void SetOnCloseCallback(Action onClose)
    {
        onCloseCallback = onClose;
    }

    public void SetOnMarbleCollisionCallback(Action<Collision2D> onMarbleCollision)
    {
        onMarbleCollisionCallback = onMarbleCollision;
    }

    // Print on collision with another object
    void OnCollisionEnter2D(Collision2D c)
    {
        //Debug.Log($"Claw collision with {c.gameObject.name} {c.collider.name} {c.otherCollider.name}");
        //Debug.Log($"this body: {c.rigidbody?.name}, this collider: {c.collider?.name}");
        //Debug.Log($"other body: {c.otherRigidbody?.name}, other collider: {c.otherCollider?.name}");
        if (c.gameObject.name.StartsWith("marble"))
        {
            onMarbleCollisionCallback?.Invoke(c);            
        }
    }

    void OnCollisionExit2D(Collision2D c)
    {
        //Debug.Log($"Claw collision exit with {c.gameObject.name} {c.collider.name} {c.otherCollider.name}");
    }

    public GameObject TryGrabObject()
    {
        var contactingColliders = new List<Collider2D>();
        var numContacts = grabDetectorCollider.GetContacts(contactingColliders);
        if (numContacts <= 0)
        {
            return null;
        }

        Debug.Log($"Claw contacting {numContacts} colliders");
        openTween = null;

        // Grab the first one
        return contactingColliders[0].gameObject;
    }

    public void DropGrabbedObject()
    {
        if (isOpen)
        {
            // should never happen?
            return;
        }

        openTween = CreateOpenTween(currentAngle, -openAngle);
        openCompleteThreshold = -CompleteThresholdOverflow;
    }
}
