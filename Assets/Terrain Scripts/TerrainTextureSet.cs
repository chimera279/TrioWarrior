using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTextureSet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TextureTerrain();
    }

    void TextureTerrain()
    {
        Terrain terrain = Terrain.activeTerrain;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(hit.point);
            Vector2 normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, terrain.terrainData.size.x, terrainLocalPos.x),
                                                   Mathf.InverseLerp(0.0f, terrain.terrainData.size.z, terrainLocalPos.z));
            float steepiness = terrain.terrainData.GetSteepness(normalizedPos.x, normalizedPos.y);
            if (steepiness > 30f)
            {
                Vector2 alphamapPos = new Vector2(normalizedPos.x * terrain.terrainData.alphamapWidth,
                                               normalizedPos.y * terrain.terrainData.alphamapHeight);
              //  var splatPrototypes = Terrain.activeTerrain.terrainData.splatPrototypes;
                //var splatAtPoint = 
                float[,,] alphamap = Terrain.activeTerrain.terrainData.GetAlphamaps(0,0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
              //  Debug.Log("alpha " + alphamap + " " + alphamap.GetLength(0) + " " + alphamap.GetLength(1));
                float rockiness = alphamap[(int)alphamapPos.x, (int)alphamapPos.y, 1];
                
                Debug.Log(rockiness);
                if (rockiness > 0.15f)
                {
                    Debug.Log("MINING");
                }
            }
        }
    }
}
