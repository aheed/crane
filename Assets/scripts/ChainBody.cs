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
    public float drumOffsetX = 0f;
    public float drumOffsetY = -0.5f;
    public float drumRadius = 1f;
    public float linkCollisionRatio = 0.5f;
    public float maxOffsetX = 6f;

    private Chain chain;
    public GameObject linkPrefab1;
    public GameObject linkPrefab2;
    public Claw clawPrefab;
    public InputAction clickAction;
    public InputAction moveAction;
    private GameObject[] linkGameObjects;
    private Claw clawGameObject;
    private Rigidbody2D clawRigidbody;
    private GameObject drumGameObject;
    private bool clawOpen = true;
    private GameObject grabbedObject = null;
    private Vector3 grabbedObjectOffset;

    void Start()
    {
        chain = new Chain(numLinks, transform.position, linkLength, simTimeFactor, jakobsenIterations, speedRetention, clawMassRatio, clawSpeedRetention, maxVerticalSpeed, maxHorizontalSpeed, linkCollisionRatio, maxOffsetX);
        linkGameObjects = new GameObject[numLinks];
        for (int i = 0; i < numLinks; i++)
        {
            var prefab = (i % 2 == 0) ? linkPrefab1 : linkPrefab2;
            linkGameObjects[i] = Instantiate(prefab, chain.links[i].position, Quaternion.identity, transform);
        }
        clawGameObject = Instantiate(clawPrefab, chain.links[numLinks - 1].position, Quaternion.identity, transform);
        clawGameObject.SetOnOpenCallback(OnOpenClawCallback);
        clawGameObject.SetOnCloseCallback(OnCloseClawCallback);
        clawRigidbody = clawGameObject.GetComponent<Rigidbody2D>();
        drumGameObject = transform.Find("drum").gameObject;
    }

    void FixedUpdate()
    {
        chain.ClawPosition = clawGameObject.transform.position;
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
        clawGameObject.transform.position = chain.ClawPosition;
        clawRigidbody.linearVelocity = Vector2.zero;

        UpdateLinkVisibility(); //expensive?
        clawGameObject.SetOpen(clawOpen);

        var topPos = linkGameObjects[firstLinkIndex].transform.position;
        drumGameObject.transform.position = new Vector3(topPos.x + drumOffsetX, transform.position.y + drumOffsetY, 0);
        var drumAngle = chain.Length / (2 * UnityEngine.Mathf.PI * drumRadius) * 360f;
        drumGameObject.transform.rotation = Quaternion.Euler(0, 0, drumAngle);

        if (grabbedObject != null)
        {
            var grabOffset = new Vector3(0, clawGameObject.grabOffsetY, 0)
                + grabbedObjectOffset;
            grabbedObject.transform.SetPositionAndRotation(
                clawGameObject.transform.position + grabOffset,
                clawGameObject.transform.rotation);
        }
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
        var gameState = GameState.GetInstance();
        switch (gameState.GetStateContents().gameStatus)
        {
            case GameStatus.FINISHED:
            case GameStatus.DEAD:
                gameState.ReportEvent(GameEvent.RESTART_REQUESTED);
                break;
            case GameStatus.PLAYING:
                clawOpen = !clawOpen;
                break;
            default:
                break;
        }
    }

    void OnOpenClawCallback()
    {
        Debug.Log("Claw almost opened");
        if (grabbedObject != null)
        {
            Debug.Log("Released object: " + grabbedObject.name);
            grabbedObject.GetComponent<IGrabbable>().Release(chain.GetClawVelocity());
            grabbedObject = null;
        }
    }

    void OnCloseClawCallback()
    {
        Debug.Log("Claw almost closed");
        var grabbedHandle = clawGameObject.TryGrabObject();
        if (grabbedHandle != null)
        {
            IGrabQueryable grabQueryable = grabbedHandle.GetComponent<IGrabQueryable>();
            var grabbable = grabQueryable.GetGrabbable();
            grabbedObject = grabbable.GetGrabbedObject();
            grabbable.Grab();
            Debug.Log("Grabbed object: " + grabbedObject.name);
            grabbedObjectOffset = grabbable.GetGrabOffset();
        }
    }
}
