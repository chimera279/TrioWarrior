using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTerrain : MonoBehaviour
{
    public Texture2D Map1,Map2;
    Terrain terr;
    float[,] heights, textureHeights, othertextureHeights;
    float[,,] alphaMaps;
    // Start is called before the first frame update
    void Start()
    {
        terr = Terrain.activeTerrain;
        textureHeights = new float[Map1.width, Map1.height];
        othertextureHeights = new float[Map1.width, Map1.height];
        for (int i = 0; i < Map1.width; i++)
        {
            for (int j = 0; j < Map1.height; j++)
            {
                textureHeights[i, j] = Map1.GetPixel(i, j).r;
                othertextureHeights[i, j] = Map2.GetPixel(i, j).r;
            }
        }

        heights = terr.terrainData.GetHeights(0,0, 1 * terr.terrainData.heightmapWidth,1 * terr.terrainData.heightmapHeight);
        GenerateATerrain();
        alphaMaps = terr.terrainData.GetAlphamaps(0, 0, terr.terrainData.alphamapWidth, terr.terrainData.alphamapHeight);
        PaintATerrain();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateATerrain()
    {
        for (int i = 0; i < heights.GetLength(0); i++)
        {
            for (int j = 0; j < heights.GetLength(1); j++)
            {
                if (j > heights.GetLength(1) / 3 && i > heights.GetLength(0) / 2)
                    heights[i, j] = textureHeights[i % Map1.width / 2, j % Map1.height / 2];
                else if (j > heights.GetLength(1) / 3 && j <= 2 * heights.GetLength(1) / 3)
                    heights[i, j] = textureHeights[(i % Map1.width / 2) + Map1.width / 2, (j % Map1.height / 2) + Map1.height / 2];
                else heights[i, j] = othertextureHeights[j % Map1.width, i % Map1.width];

                //  heights[i, j] = textureHeights[j % Map1.width, i % Map1.width];
            }
        }
        terr.terrainData.SetHeights(0, 0, heights);

    }

    void PaintATerrain()
    {
        for (int i = 0; i < heights.GetLength(0); i++)
        {
            for (int j = 0; j < heights.GetLength(1); j++)
            {
                float steepiness = terr.terrainData.GetSteepness(i / terr.terrainData.heightmapWidth, j / terr.terrainData.heightmapHeight);
                Vector2 alphamapPos = new Vector2(i * terr.terrainData.alphamapWidth/terr.terrainData.heightmapWidth,
                                               j * terr.terrainData.alphamapHeight / terr.terrainData.heightmapHeight);

                if (heights[i,j]>0.15f)
                {
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 0] = 0f;
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 1] = 1;
                  //  Debug.Log("WAI");
                }
                else
                {
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 0] = 1;
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 1] = 0f;
                    //Debug.Log("WAI");
                }

            }
        }
        terr.terrainData.SetAlphamaps(0, 0, alphaMaps);
    }
}
