using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCreator : MonoBehaviour
{
    // Attributes
    public Sprite terrainTile;
    private Vector3 startPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosition = new Vector3(-9, -5, 0);

        MakeTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Places grid of terrain tiles
    void MakeTerrain()
    {
        // Iterate horizontal
        for(int i = 0; i < 18; i++)
        {
            // Iterate vertical
            for (int j = 0; j < 10; j++)
            {
                Vector3 position = new Vector3(startPosition.x + i, startPosition.y + j, 0);

                GameObject obj = new GameObject((i + j).ToString());

                obj.transform.position = position;

                SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();

                sr.sprite = terrainTile;
            }
        }
    }
}
