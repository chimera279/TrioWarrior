using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Batcher
{
    public static Dictionary<GameObject, List<Vector3>> batchPossesMommy = new Dictionary<GameObject, List<Vector3>>();
    static Terrain terry;
    static int width = 15, height = 15;
    static float[,] heights;
    public static void AddToBatch(GameObject obj, Vector3 pos)
    {
        if (!batchPossesMommy.ContainsKey(obj))
            batchPossesMommy.Add(obj, new List<Vector3>());
        else batchPossesMommy[obj].Add(pos);
    }

    public static IEnumerator PerformBatching(Vector3 playerLocation, int bounds, Terrain terr, float rockfallTimer, float batchTime)
    {
        terry = terr;
        Vector3 tempPlayerCoord = playerLocation - terry.gameObject.transform.position;
        Vector3 playerCoord;
        playerCoord.x = tempPlayerCoord.x / terry.terrainData.size.x;
        playerCoord.y = tempPlayerCoord.y / terry.terrainData.size.y;
        playerCoord.z = tempPlayerCoord.z / terry.terrainData.size.z;

        int playerX = (int)(playerCoord.x * terry.terrainData.heightmapWidth);
        int playerY = (int)(playerCoord.z * terry.terrainData.heightmapHeight);


        if (playerX - bounds / 2 >= 0 && playerY - bounds / 2 >= 0 && playerX  <= terry.terrainData.heightmapWidth - 2 - bounds/2 && playerY  <= terry.terrainData.heightmapHeight - 2 - bounds/2)
            heights = terry.terrainData.GetHeights((playerX - bounds / 2), (playerY - bounds / 2), bounds, bounds);
        else yield break;
        var originalHeights = heights;

        bool isChanged = false;
        var onTheUp = new bool[bounds, bounds];

        foreach (var objs in batchPossesMommy.Keys)
        {
            //if (Physics.OverlapSphere(transform.position, .25f,1<<LayerMask.NameToLayer("Terrain")).Length == 0)

            foreach (var pos in batchPossesMommy[objs])

            {

                // get the normalized position of this game object relative to the terrain
                Vector3 tempCoord = (pos - terry.gameObject.transform.position);
                Vector3 coord;
                coord.x = tempCoord.x / terry.terrainData.size.x;
                coord.y = tempCoord.y / terry.terrainData.size.y;
                coord.z = tempCoord.z / terry.terrainData.size.z;


                int posXInTerrain = (int)(coord.x * terry.terrainData.heightmapWidth);
                int posYInTerrain = (int)(coord.z * terry.terrainData.heightmapHeight);

                // we set an offset so that all the raising terrain is under this game object
                int offsetx = width / 2;
                int offsety = height/ 2;

                // float[,] smallheights = terry.terrainData.GetHeights(posXInTerrain - offsetx, posYInTerrain - offsety, width, height);

                // we set each sample of the terrain in the size to the desired height
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        float newHeight = coord.y;  //(chunk[i, j] - (averageHeight - coord.y));
                        int x = (posXInTerrain - offsetx + i)  ;//* bounds / terry.terrainData.heightmapWidth);         //Mathf.Abs((posXInTerrain - offsetx + i) - (int)(playerX - bounds/1.57f));         //((posXInTerrain - offsetx + i) * heights.GetLength(0)/ terry.terrainData.heightmapWidth) ;         //- (int)(playerX - bounds / 2);
                        int y = ((posYInTerrain - offsety + j)) ;//* bounds / terry.terrainData.heightmapHeight);   //Mathf.Abs((posYInTerrain - offsety + j) - (int)(playerY - bounds/ 1.57f));        //((posYInTerrain - offsety + j) * heights.GetLength(1) / terry.terrainData.heightmapHeight) ;
                        if (!CheckEndCoords(x, y,playerX,playerY,bounds))
                        {
                            x = Mathf.Abs(x - (playerX-bounds/2));//x * bounds / terry.terrainData.heightmapWidth;
                            y = Mathf.Abs(y - (playerY - bounds / 2));

                            if (heights[x, y] == 0)
                            {
                                // if(Mathf.Abs(heights[x,y] - newHeight)>0.001f)
                                 // * (k+1)/heightFrames;
                                isChanged = true;
                                onTheUp[x, y] = false;
                                heights[x, y] = newHeight;
                            }
                            else onTheUp[x, y] = true;
                            if (Mathf.Abs(heights[x, y] - newHeight) > 0f)
                                heights[x, y] = newHeight;
                        }
                        
                    }
            }
            
        }
        if (isChanged)
        {

            float timer = 0;
            while (timer < rockfallTimer)
            {
                var newHeights = new float[bounds,bounds];
                for (int i = 0; i < bounds; i++)
                    for (int j = 0; j < bounds; j++)
                //        if(!onTheUp[i,j])
                        newHeights[i, j] = (heights[i, j] + Mathf.PerlinNoise(i, j)/bounds) * (timer + Time.deltaTime) / rockfallTimer;
                  //      else
                    //        newHeights[i, j] = heights[i, j];

                terry.terrainData.SetHeights((playerX - bounds / 2), (playerY - bounds / 2), newHeights);
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(batchTime - rockfallTimer * 2);
            if(timer>rockfallTimer)
                while(timer>0)
                {
                    var newHeights = new float[bounds, bounds];
                    for (int i = 0; i < bounds; i++)
                        for (int j = 0; j < bounds; j++)
                          if(heights[i,j]!=0)
                            newHeights[i, j] = (heights[i, j]+Mathf.PerlinNoise(i,j)/bounds) * (timer - Time.deltaTime) / rockfallTimer;
                    terry.terrainData.SetHeights((playerX - bounds / 2), (playerY - bounds / 2), newHeights);
                    timer -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();

                }
        }
        batchPossesMommy = new Dictionary<GameObject, List<Vector3>>();
    }

    static bool CheckEndCoords(int i, int j, int playerX, int playerY, int bounds)
    {
        if (i + bounds/2 - playerX  < 0 || j + bounds/2 - playerY < 0 || playerX + bounds / 2 - i < 0 || playerY + bounds / 2 - j < 0) //i >= heights.GetLength(0) - 1 || j >= heights.GetLength(1) - 1)
            return true;
        else return false;

    }



}
