using UnityEngine;
using UnityEngine.InputSystem;

public class ChainBody : MonoBehaviour
{
    public int numLinks = 10;
    public float linkLength = 0.5f;
    public float simTimeFactor = 10f;
    public int jakobsenIterations = 1;
    public float speedRetention = 0.99f;
    public float clawSpeedRetention = 0.9985f;
    public float clawMassRatio = 10f;
    public float maxVerticalSpeed = 5f;
    public float maxHorizontalSpeed = 5f;

    private Chain chain;
    public GameObject linkPrefab1;
    public GameObject linkPrefab2;
    public Claw clawPrefab;
    public InputAction clickAction;
    public InputAction moveAction;
    private GameObject[] linkGameObjects;
    private Claw clawGameObject;
    private bool clawOpen = true;

    void Start()
    {
        chain = new Chain(numLinks, transform.position, linkLength, simTimeFactor, jakobsenIterations, speedRetention, clawMassRatio, clawSpeedRetention, maxVerticalSpeed, maxHorizontalSpeed);
        linkGameObjects = new GameObject[numLinks];
        for (int i = 0; i < numLinks; i++)
        {
            var prefab = (i % 2 == 0) ? linkPrefab1 : linkPrefab2;
            linkGameObjects[i] = Instantiate(prefab, chain.links[i].position, Quaternion.identity, transform);
        }
        clawGameObject = Instantiate(clawPrefab, chain.links[numLinks - 1].position, Quaternion.identity, transform);
        /*clickAction.Enable();
        clickAction.performed += ctx =>
        {
            var mousePosition = ctx.ReadValue<Vector2>();
            var worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
            chain.links[0].position = worldPosition;
            chain.links[0].lastPosition = worldPosition;
        };*/
    }

    void FixedUpdate()
    {
        chain.Update(Time.fixedDeltaTime);
        var firstLinkIndex = chain.GetTopLinkIndex();
        for (int i = firstLinkIndex; i < chain.links.Length; i++)
        {
            linkGameObjects[i].transform.position = chain.links[i].position;
        }

        for (int i = firstLinkIndex + 1; i < chain.links.Length; i++)
        {
            linkGameObjects[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, chain.links[i].position - chain.links[i - 1].position);
        }

        linkGameObjects[firstLinkIndex].transform.rotation = Quaternion.identity;
        clawGameObject.transform.position = chain.links[numLinks - 1].position;

        UpdateLinkVisibility(); //expensive?
        clawGameObject.SetOpen(clawOpen);
    }

    void UpdateLinkVisibility()
    {
        var firstLinkIndex = chain.GetTopLinkIndex();
        for (int i = 0; i < firstLinkIndex; i++)
        {
            linkGameObjects[i].SetActive(false);
        }

        for (int i = firstLinkIndex; i < numLinks; i++)
        {
            linkGameObjects[i].SetActive(true);
        }
    }

    // If you are interested in the value from the control that triggers an action, you can declare a parameter of type InputValue.
    public void OnMove(InputValue value)
    {
        // Read value from control. The type depends on what type of controls.
        // the action is bound to.
        chain.SetMoveInput(value.Get<Vector2>());

        // IMPORTANT:
        // The given InputValue is only valid for the duration of the callback. Storing the InputValue references somewhere and calling Get<T>() later does not work correctly.
    }

    public void OnAttack()
    {
        clawOpen = !clawOpen;
    }
}
