using UnityEngine;

public class eulerbody : MonoBehaviour
{
    public Vector3 gravityVector = new Vector3(0f, -9.81f, 0f);
    public float simTimeFactor = 10f;
    Vector3 velocity;

    void Start()
    {
        velocity = Vector3.zero;
    }

    void Update()
    {
        var dt = Time.deltaTime * simTimeFactor;
        velocity += gravityVector * dt;
        transform.position += velocity * dt;
    }
}
