using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // Attributes
    [Header("Assets")]
    public List<Sprite> civilianSprites;
    public List<Sprite> hostileSprites;
    public GameObject civiliansPrefab;
    public GameObject hostilesPrefab;
    [Header("Placement Data")]
    private Camera cam;
    private float height;
    private float width;

    // Start is called before the first frame update
    void Start()
    {
        //Setup camera variables
        cam = Camera.main;
        height = 2f * cam.orthographicSize;
        width = height * cam.aspect;

        CreateTargets();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateTargets()
    {
        GameObject parent = GameObject.Find("NPCs");

        // Create civilians
        for (int i = 0; i < 20; i++)
        {
            // Create gameobject
            GameObject obj = Instantiate(civiliansPrefab, PickRandomLocation(), Quaternion.identity);
            // Set parent
            obj.transform.SetParent(parent.transform);
            // Add sprite
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = civilianSprites[Random.Range(0, civilianSprites.Count)]; // pick random sprite
        }

        // Create hostiles
        for (int j = 0; j < 5; j++)
        {
            // Create gameobject
            GameObject obj = Instantiate(hostilesPrefab, PickRandomLocation(), Quaternion.identity);
            // Set parent
            obj.transform.SetParent(parent.transform);
            // Add sprite
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = hostileSprites[Random.Range(0, hostileSprites.Count)]; // pick random sprite
        }
    }

    private Vector3 PickRandomLocation()
    {
        return new Vector3(Random.Range(-9, 7), Random.Range(-4, 4), 0);
    }
}
