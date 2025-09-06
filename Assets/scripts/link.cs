using UnityEngine;

public class link : MonoBehaviour
{
    //public float gravity = -9.81f;
    public Vector3 gravityVector = new Vector3(0f, -9.81f, 0f);
    public float simTimeFactor = 10f;
    Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        var dt = Time.deltaTime * simTimeFactor;
        var currentPosition = transform.position;
        //transform.position = new Vector3(0, 2 * currentPosition.y - lastPosition.y + gravity * dt * dt, 0);
        transform.position = 2 * currentPosition - lastPosition + (dt * dt) * gravityVector;
        lastPosition = currentPosition;
    }
}
