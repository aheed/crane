using UnityEngine;

public class ChainBody : MonoBehaviour
{
    public int numLinks = 10;
    public float linkLength = 0.5f;
    public float simTimeFactor = 10f;
    public int jakobsenIterations = 1;
    public float speedRetention = 0.99f;

    private Chain chain;
    public GameObject linkPrefab;
    private GameObject[] linkGameObjects;

    void Start()
    {
        chain = new Chain(numLinks, transform.position, linkLength, simTimeFactor, jakobsenIterations, speedRetention);
        linkGameObjects = new GameObject[numLinks];
        for (int i = 0; i < numLinks; i++)
        {
            linkGameObjects[i] = Instantiate(linkPrefab, chain.links[i].position, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        chain.Update(Time.deltaTime);
        for (int i = 0; i < chain.links.Length; i++)
        {
            linkGameObjects[i].transform.position = chain.links[i].position;
        }
    }
}
