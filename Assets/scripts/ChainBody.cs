using UnityEngine;

public class ChainBody : MonoBehaviour
{
    public int numLinks = 10;
    public float linkLength = 0.5f;
    public float simTimeFactor = 10f;
    public int jakobsenIterations = 1;
    public float speedRetention = 0.99f;
    public float clawSpeedRetention = 0.9985f;
    public float clawMassRatio = 10f;

    private Chain chain;
    public GameObject linkPrefab1;
    public GameObject linkPrefab2;
    public GameObject clawPrefab;
    private GameObject[] linkGameObjects;
    private GameObject clawGameObject;

    void Start()
    {
        chain = new Chain(numLinks, transform.position, linkLength, simTimeFactor, jakobsenIterations, speedRetention, clawMassRatio, clawSpeedRetention);
        linkGameObjects = new GameObject[numLinks];
        for (int i = 0; i < numLinks; i++)
        {
            var prefab = (i % 2 == 0) ? linkPrefab1 : linkPrefab2;
            linkGameObjects[i] = Instantiate(prefab, chain.links[i].position, Quaternion.identity, transform);
        }
        clawGameObject = Instantiate(clawPrefab, chain.links[numLinks - 1].position, Quaternion.identity, transform);
    }

    void FixedUpdate()
    {
        chain.Update(Time.fixedDeltaTime);
        for (int i = 0; i < chain.links.Length; i++)
        {
            linkGameObjects[i].transform.position = chain.links[i].position;
        }

        for (int i = 1; i < chain.links.Length; i++)
        {
            linkGameObjects[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, chain.links[i].position - chain.links[i - 1].position);
        }
        clawGameObject.transform.position = chain.links[numLinks - 1].position;
    }
}
