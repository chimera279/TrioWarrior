using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureToHeight
{ 
    // Start is called before the first frame update


    public static void SetHeightFromTexture(Texture2D heightTex)
    {
        Terrain terry;
        terry = Terrain.activeTerrain;
        float[,] heights = terry.terrainData.GetHeights(0, 0, terry.terrainData.heightmapWidth, terry.terrainData.heightmapHeight);

        for (int i = 0; i < heightTex.width; i++)
        {
            for (int j = 0; j < heightTex.height; j++)
            {
                heights[i, j] = heightTex.GetPixel(i, j) == Color.red ? 0f : 0f;
  
            }

        }
        terry.terrainData.SetHeights(0, 0, heights);
    }
}
