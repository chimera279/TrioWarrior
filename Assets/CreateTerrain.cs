using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CreateTerrain : MonoBehaviour
{
    public Texture2D Map1, Map2,Map3;
    List<Texture2D> Maps;
    Terrain terr,  manipulationTerrain;
    float[,] heights, manipulationHeights, textureHeights, othertextureHeights;
    public static float[,,] alphaMaps;
    float timer, maxTimer;
    Vector2Int position,positionPoint,destinationPoint;
    List<Vector2Int> chunk;
    int chunkWidth, chunkHeight, activeMap;
    int[] selectedMaps;
    public float batchTimer, batchTime, rockfallTimer;
    public int textureRange, miningOdds, heightFrames;
    public GameObject player;
    GameObject rockSpecial, paperSpecial, scissorSpecial;
    // Start is called before the first frame update
    void Start()
    {
        Maps = new List<Texture2D>();
        terr = GameObject.FindGameObjectWithTag("MainTerrain").GetComponent<Terrain>();
        manipulationTerrain = GameObject.FindGameObjectWithTag("ManipulationTerrain").GetComponent<Terrain>();
        player = GameObject.FindGameObjectWithTag("Player");
        rockSpecial = Resources.Load<GameObject>("Prefabs/Rock Paper Special");
        paperSpecial = Resources.Load<GameObject>("Prefabs/Paper Scissor Special");
        scissorSpecial = Resources.Load<GameObject>("Prefabs/Scissor Rock Special");
        chunkWidth = Random.Range(500, 513);
        chunkHeight = Random.Range(500, 513);
        ////chunkWidth = 1081;
        ////chunkHeight = 1081;
        chunk = new List<Vector2Int>();
        //textureHeights = new float[Map1.width, Map1.height];
        //othertextureHeights = new float[Map1.width, Map1.height];
        //for (int i = 0; i < Map1.width; i++)
        //{
        //    for (int j = 0; j < Map1.height; j++)
        //    {
        //        textureHeights[i, j] = Map1.GetPixel(i, j).r;
        //        othertextureHeights[i, j] = Map2.GetPixel(i, j).r;
        //    }
        //}
        timer = 0;
        maxTimer = Random.Range(1, 4) * 30;
        batchTimer = 0;
        rockfallTimer = Time.deltaTime * heightFrames;
       // batchTime = Time.deltaTime * 3;

        Maps = Resources.LoadAll<Texture2D>("HeightMaps").ToList();
        selectedMaps = new int[7];  // Depends on how many randomizers down below
        for (int i = 0; i < selectedMaps.Length; i++)
        {
            selectedMaps[i] = Random.Range(0, Maps.Count);
        }
        heights = terr.terrainData.GetHeights(0, 0, 1 * terr.terrainData.heightmapWidth, 1 * terr.terrainData.heightmapHeight);
        manipulationHeights = manipulationTerrain.terrainData.GetHeights(0, 0, 1 * terr.terrainData.heightmapWidth, 1 * terr.terrainData.heightmapHeight);
        GenerateATerrain();
        alphaMaps = terr.terrainData.GetAlphamaps(0, 0, terr.terrainData.alphamapWidth, terr.terrainData.alphamapHeight);
        PaintATerrain();

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > maxTimer)
        {
          //  StartCoroutine(UpdateRPSMaps());
            timer = 0;
          //  maxTimer = Random.Range(10f, 15f);
        }
        batchTimer += Time.unscaledDeltaTime;
        if(batchTimer>batchTime)
        {
            batchTimer = 0;

        //    Debug.LogError("WEOIH");
            StartCoroutine(Batcher.PerformBatching(player.transform.position, 150,manipulationTerrain,rockfallTimer,batchTime));
        }

        if(Input.GetKeyDown(KeyCode.M))
            manipulationTerrain.terrainData.SetHeights(0, 0, manipulationHeights);
        UpdateMining();
     //   CheckTextureMap();
    }

    void GenerateATerrain()
    {

        //int activeMap = Random.Range(0, Maps.Count);
        //while (positionPoint == destinationPoint)
        //{
        //    positionPoint = new Vector2Int(Random.Range(0, Maps[activeMap].width), Random.Range(0, Maps[activeMap].height));
        //    destinationPoint = new Vector2Int(Random.Range(0, Maps[activeMap].width), Random.Range(0, Maps[activeMap].height));
        //}
        //   position = positionPoint;

        int chunkCounter = 0;
        CreateChunk();

        for (int i = 0; i < heights.GetLength(0); i++)
        {
            for (int j = 0; j < heights.GetLength(1); j++)
            {
                //MoveOnTexture();
                if (j > heights.GetLength(1) / 3 && i > heights.GetLength(0) / 1.25f)
                    heights[i, j] = Maps[selectedMaps[0]].GetPixel(i % Map1.width / 4, j % Map1.height / 4).r;

                else if (j > heights.GetLength(1) / 3 && j <= 2 * heights.GetLength(1) / 3)
                {
                    if (i >= heights.GetLength(0) / 6 && i < heights.GetLength(0) / 4)
                        heights[i, j] = Maps[selectedMaps[1]].GetPixel((i % Map1.width / 2) + Map1.width / 2, (j % Map1.height / 3) + Map1.height / 2).r;
                    else if (i >= heights.GetLength(0) / 4 && i < heights.GetLength(0) / 2)
                        heights[i, j] = Maps[selectedMaps[4]].GetPixel((j % Map1.width / 3) + Map1.width / 2, (i % Map1.height / 2) + Map1.height / 2).r;
                    else heights[i,j] = Maps[selectedMaps[2]].GetPixel((j % Map1.width / 3) + Map1.width / 2, (i % Map1.height / 2) + Map1.height / 2).r;
                }
                else 
                {
                    if (i >= heights.GetLength(0) / 6 && i < heights.GetLength(0) / 4)
                        heights[i, j] = Maps[selectedMaps[3]].GetPixel((i % Map1.width / 4) + Map1.width / 2, (j % Map1.height / 2) + Map1.height / 3).r;
                    else if (i >= heights.GetLength(0) / 4 && i < heights.GetLength(0) / 2)
                        heights[i, j] = Maps[selectedMaps[5]].GetPixel((j % Map1.width / 3) + Map1.width / 2, (i % Map1.height / 2) + Map1.height / 4).r;

                    else heights[i, j] = Maps[selectedMaps[6]].GetPixel(j % Map1.width, i % Map1.width).r;
                }
                //heights[i, j] %= 0.9f;
                if (heights[i, j] == 0)
                    heights[i, j] = 0.0001f;
                manipulationHeights[i,j] = 0f;

               // heights[i, j] += 0.25f;

                //   heights[i, j] = Maps[0].GetPixel(i % Map1.width, j % Map1.width).r ;

                //if (position == destinationPoint)
                //{
                //    positionPoint = destinationPoint;
                //    while (positionPoint == destinationPoint)
                //    {
                //        destinationPoint = new Vector2Int(Random.Range(0, Maps[activeMap].width), Random.Range(0, Maps[activeMap].height));
                //    }
                //    // activeMap++;
                //    // activeMap %= Maps.Count;
                //}

                //    heights[i, j] = textureHeights[position.x,position.y];
                //if(chunkCounter == chunk.Count)
                //{
                //    chunkCounter = 0;
                //    chunkWidth = 1081;
                //    chunkHeight = 1081;
                //    CreateChunk();
                //}
                //   heights[i, j] = Maps[activeMap].GetPixel(chunk[i%Maps[activeMap].width].x, chunk[j% Maps[activeMap].width].y).r;
                chunkCounter++;

            }
        }
        manipulationTerrain.terrainData.SetHeights(0, 0, manipulationHeights);
        terr.terrainData.SetHeights(0, 0, heights);

    }

    void CreateChunk()
    {
        activeMap = Random.Range(0, Maps.Count);
        chunk = new List<Vector2Int>();
        //   positionPoint = new Vector2Int(Random.Range(0, Maps[activeMap].width/2), Random.Range(0, Maps[activeMap].height/2));
        positionPoint = new Vector2Int();
        for (int i = 0; i < chunkWidth; i++)
            for (int j = 0; j < chunkHeight; j++)
                chunk.Add(new Vector2Int(i, j));
    }

    void MoveOnTexture()        // So I wanted to make the chunk move along a heightmap and print those heights on the AI's trail, had they had a trail
    {
            position.y = (int)Mathf.Lerp(positionPoint.y, destinationPoint.y, (float)1 / Mathf.Abs(positionPoint.y - destinationPoint.y));
            // if (start.x!=end.x)
            position.x = (int)Mathf.Lerp(positionPoint.x, destinationPoint.x, (float)1 / Mathf.Abs(positionPoint.x - destinationPoint.x));
            //  if (start.y != end.y)

            //  Debug.Log((int)Mathf.Lerp(startPlace.x, end.x, (newLine.IndexOf(start) + 2) / Mathf.Abs(end.x - startPlace.x)));
            //   Debug.Log(start);
    }

    void PaintATerrain()        // Paints based on 5 (lol 10) terrain textures, and slope based angular painting. Everytime, different order of textures selected for blend, so few different field looks possible
    {
        for (int i = 0; i < (heights.GetLength(0)-1)  ; i++)
        {
            for (int j = 0; j < (heights.GetLength(1)-1); j++)
            {
                float steepiness = terr.terrainData.GetSteepness(i / terr.terrainData.size.x, j / terr.terrainData.size.z);
                Vector2 alphamapPos = new Vector2(i * terr.terrainData.alphamapWidth / terr.terrainData.heightmapWidth,//terr.terrainData.size.x,
                                               j * terr.terrainData.alphamapHeight / terr.terrainData.heightmapHeight);//terr.terrainData.size.z);

                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 0] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 1] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 2] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 3] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 4] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 5] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 6] = 0f; 
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 7] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 8] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 9] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 10] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 11] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 12] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 13] = 0f;
                alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 14] = 0f;

                //if (heights[i, j] <= 0.1f)
                //    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 3] = 1;

                //else if (heights[i, j] > 0.1f && heights[i, j] <= 0.2f)
                //    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 4] = 1;
                //else if (heights[i, j] > 0.2f && heights[i, j] <= 0.3f)
                //    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 2] = 1;
                //else if (heights[i, j] > 0.3f && heights[i, j] <= 0.4f)
                //    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 0] = 1;
                //else
                //    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 1] = 1;

                List<int> intLists = RandomizedInts();

                if (steepiness >= 0f && steepiness <= 9f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[0]] = 1;               //3
                else if (steepiness > 9f && steepiness <= 18f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[4]] = 1;               //4
                else if (steepiness > 18f && steepiness <= 27f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[2]] = 1;               //2
                else if (steepiness > 27f && steepiness <= 36f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[1]] = 1;               //0
                else if (steepiness > 36f && steepiness <= 45f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[3]] = 1;                  //0
                else if (steepiness > 45f && steepiness <= 54f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[6]] = 1;                //0
                else if (steepiness > 54f && steepiness <= 63f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[9]] = 1;                  //0
                else if (steepiness > 63f && steepiness <= 72f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[7]] = 1;                  //0
                else if (steepiness > 72f && steepiness <= 81f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[5]] = 1;                  //0
                else if (steepiness > 81f && steepiness <= 90f)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, intLists[8]] = 1;                 //1
                //else
                //{

                //}

                int Mining = Random.Range(0, miningOdds);
                if (Mining == 9)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 10] = 1f;//Random.Range(0.75f, 1f);
                else if (Mining == 27)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 11] = 1f;//Random.Range(0.75f, 1f);
                else if (Mining == 3)
                    alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 12] = 1f;//Random.Range(0.75f, 1f);


            }
        }
        terr.terrainData.SetAlphamaps(0, 0, alphaMaps);
        manipulationTerrain.terrainData.SetAlphamaps(0, 0, alphaMaps);
    }

    IEnumerator UpdateRPSMaps()
    {
        float[,,] smallalphaMaps;
        Vector3 tempPlayerCoord = player.transform.position - terr.gameObject.transform.position;
        Vector3 playerCoord;
        playerCoord.x = tempPlayerCoord.x / terr.terrainData.size.x;
        playerCoord.y = tempPlayerCoord.y / terr.terrainData.size.y;
        playerCoord.z = tempPlayerCoord.z / terr.terrainData.size.z;

        int playerX = (int)(playerCoord.x * terr.terrainData.heightmapWidth);
        int playerY = (int)(playerCoord.z * terr.terrainData.heightmapHeight);


        if (!(playerX - textureRange / 2 >= 0 && playerY - textureRange / 2 >= 0 && playerX <= terr.terrainData.alphamapWidth - 2 - textureRange / 2 && playerY <= terr.terrainData.alphamapHeight - 2 - textureRange / 2))
            ReadjustPlayer(ref playerX, ref playerY);
 
        smallalphaMaps = terr.terrainData.GetAlphamaps((playerX - textureRange / 2), (playerY - textureRange / 2), textureRange, textureRange);
        for (int i = 0; i < heights.GetLength(0); i++)
        {
            for (int j = 0; j < heights.GetLength(1); j++)
            {
                Vector2 alphamapPos = new Vector2(i * smallalphaMaps.GetLength(0) / terr.terrainData.heightmapWidth,
                                  j * smallalphaMaps.GetLength(1) / terr.terrainData.heightmapHeight);


                if (alphamapPos.x - textureRange / 2 >= 0 && alphamapPos.y - textureRange / 2 >= 0 && alphamapPos.x <= terr.terrainData.alphamapWidth - 2 - textureRange / 2 && alphamapPos.y <= terr.terrainData.alphamapHeight - 2 - textureRange / 2)
                {
                    smallalphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 10] = 0f;
                    smallalphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 11] = 0f;
                    smallalphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 12] = 0f;
                    int Mining = Random.Range(0, miningOdds);
                    if (Mining == 9)
                        smallalphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 10] = 1f;// Random.Range(0.75f, 1f);
                    else if (Mining == 27)
                        smallalphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 11] = 1f;// Random.Range(0.75f, 1f);
                    else if (Mining == 3)
                        smallalphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 12] = 1f;//Random.Range(0.75f, 1f);
                }
                
              
            }
        }
        terr.terrainData.SetAlphamaps((playerX - textureRange / 2), (playerY - textureRange / 2),smallalphaMaps);
        alphaMaps = terr.terrainData.GetAlphamaps(0, 0, terr.terrainData.alphamapWidth, terr.terrainData.alphamapHeight);
        yield return new WaitForSeconds(Time.deltaTime);
    }
    void CheckTextureMap()          // This became the function scissor uses to draw a trail
    {
        Vector3 tempPlayerCoord = player.transform.position - terr.gameObject.transform.position;
        Vector3 playerCoord;
        playerCoord.x = tempPlayerCoord.x / terr.terrainData.size.x;
        playerCoord.y = tempPlayerCoord.y / terr.terrainData.size.y;
        playerCoord.z = tempPlayerCoord.z / terr.terrainData.size.z;

        int playerX = (int)(playerCoord.x * terr.terrainData.heightmapWidth);
        int playerY = (int)(playerCoord.z * terr.terrainData.heightmapHeight);
        for (int i = 1; i < 2; i+=2)
        {
            for (int j = 1; j < 2; j+=2)
            {
                Vector2 alphamapPos = new Vector2((playerX + i) * terr.terrainData.alphamapWidth / terr.terrainData.heightmapWidth,
                                (playerY + j) * terr.terrainData.alphamapHeight / terr.terrainData.heightmapHeight);
                var newMap = terr.terrainData.GetAlphamaps((int)alphamapPos.x, (int)alphamapPos.y, 1, 1);
                newMap[0, 0, 10] = 1;
                terr.terrainData.SetAlphamaps((int)alphamapPos.x, (int)alphamapPos.y, newMap);
            }
        }
    }

    void UpdateMining()
    {
        Vector3 tempPlayerCoord = player.transform.position - terr.gameObject.transform.position;
        Vector3 playerCoord;
        playerCoord.x = tempPlayerCoord.x / terr.terrainData.size.x;
        playerCoord.y = tempPlayerCoord.y / terr.terrainData.size.y;
        playerCoord.z = tempPlayerCoord.z / terr.terrainData.size.z;

        int playerX = (int)(playerCoord.x * terr.terrainData.heightmapWidth);
        int playerY = (int)(playerCoord.z * terr.terrainData.heightmapHeight);
        int specials = -1;
        for (int i = 1; i < 2; i++)
        {
            for (int j = 1; j < 2; j++)
            {
                Vector2 alphamapPos = new Vector2((playerX + i) * terr.terrainData.alphamapWidth / terr.terrainData.heightmapWidth,
                                  (playerY + j) * terr.terrainData.alphamapHeight / terr.terrainData.heightmapHeight);
                var newMap = terr.terrainData.GetAlphamaps((int)alphamapPos.x, (int)alphamapPos.y, 1, 1);
                //Debug.Log(alphamapPos.x + "   " + (playerX + i));
                //Debug.Log(terr.terrainData.alphamapWidth + "   " + terr.terrainData.heightmapWidth);
                float height = terr.terrainData.GetHeights(playerX+i, playerY+j, 1, 1)[0, 0];
                if (player.GetComponent<Moveonterrain>().isGrounded)
                {
                    if (newMap[0, 0, 10] ==1)                             // Custom colors I picked on testing
                    {
                        specials = 0;
                        alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 10] = 0;
                        alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 13] = 1;
                        newMap[0, 0, 10] = 0;
                        newMap[0, 0, 13] = 1;
                        terr.terrainData.SetAlphamaps((int)alphamapPos.x, (int)alphamapPos.y, newMap);
                        break;
                    }
                    else
                    if (newMap[0, 0, 11] == 1)
                    {
                        specials = 1;
                        alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 11] = 0;
                        alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 13] = 1;
                        newMap[0, 0, 11] = 0;
                        newMap[0, 0, 13] = 1;
                        terr.terrainData.SetAlphamaps((int)alphamapPos.x, (int)alphamapPos.y, newMap);
                        break;
                    }
                    else
                    if (newMap[0, 0, 12] == 1)
                    {
                        specials = 2;
                        alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 12] = 0;
                        alphaMaps[(int)alphamapPos.x, (int)alphamapPos.y, 13] = 1;
                        newMap[0, 0, 12] = 0;
                        newMap[0, 0, 13] = 1;
                        terr.terrainData.SetAlphamaps((int)alphamapPos.x, (int)alphamapPos.y, newMap);
                        break;
                    }
                }

            }
        }
        if(specials>=0)
        {
            Vector3 spawnPos = new Vector3(Random.Range(player.transform.position.x - SpawnBugs.SpawnBox.x/2, player.transform.position.x + SpawnBugs.SpawnBox.x / 2),
        Random.Range(player.transform.position.y - SpawnBugs.SpawnBox.y / 2, player.transform.position.y + SpawnBugs.SpawnBox.y / 2),
        Random.Range(player.transform.position.z - SpawnBugs.SpawnBox.z / 2, player.transform.position.z + SpawnBugs.SpawnBox.z / 2));
            Passives passiveScript;
            Moveonterrain playerscript = player.GetComponent<Moveonterrain>();
            switch (specials)
            {
                case 0:
                    passiveScript = Instantiate(rockSpecial, spawnPos, Quaternion.identity).GetComponent<Passives>();
                    passiveScript.InitializePassives((Attacks.AtkDefElement)0, playerscript.attackElement,playerscript);
                    break;
                case 1:
                    passiveScript = Instantiate(paperSpecial, spawnPos, Quaternion.identity).GetComponent<Passives>();
                    passiveScript.InitializePassives((Attacks.AtkDefElement)1, playerscript.attackElement, playerscript);
                    break;
                case 2:
                    passiveScript = Instantiate(scissorSpecial, spawnPos, Quaternion.identity).GetComponent<Passives>();
                    passiveScript.InitializePassives((Attacks.AtkDefElement)2, playerscript.attackElement, playerscript);
                    break;

            }
        }

    }



    List<int> RandomizedInts()
    {
        List<int> intList = new List<int>();
        while(intList.Count<10)
        {
            int random = Random.Range(0, 10);
            if (!intList.Contains(random))
                intList.Add(random);
        }
        return intList;
    }

    void ReadjustPlayer(ref int x, ref int y)
    {
        if (x - textureRange / 2 < 0)
            x = textureRange / 2;
        if (y - textureRange / 2 < 0)
            y = textureRange / 2;

        if (x > terr.terrainData.alphamapWidth - 1 - textureRange / 2)
            x = terr.terrainData.alphamapWidth - 1 - textureRange / 2;
        if (y > terr.terrainData.alphamapHeight - 1 - textureRange / 2)
            y = terr.terrainData.alphamapHeight - 1 - textureRange / 2;
    }
}
