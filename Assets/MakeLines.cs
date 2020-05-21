using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeLines : MonoBehaviour
{
    public Texture2D MapToMake, MapToSet;
    public int numLines;
    List<List<Vector2Int>> lines;
    Dictionary<Vector2Int, List<List<Vector2Int>>> intersectionPoints;
    // Start is called before the first frame update
    void Start()
    {
        MapToMake = new Texture2D(1024, 1024);
        for (int i = 0; i < MapToMake.width; i++)
            for (int j = 0; j < MapToMake.height; j++)
                MapToMake.SetPixel(i, j, Color.red);
        lines = new List<List<Vector2Int>>();
        GenerateLines();
        PaintLines();
        //for (int i = 0; i < MapToSet.width; i++)
        //    for (int j = 0; j < MapToSet.height; j++)
        //        MapToSet.SetPixel(i, j, MapToMake.GetPixel(i, j));
        intersectionPoints = new Dictionary<Vector2Int, List<List<Vector2Int>>>();
        DefineIntersections();
        TextureToHeight.SetHeightFromTexture(MapToMake);
        GetComponent<Renderer>().material.mainTexture = MapToMake;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateLines()
    {
        Vector2Int start, end, startPlace;
 
        end = new Vector2Int();
        for (int i = 0; i < numLines; i++)
        {
            List<Vector2Int> newLine = new List<Vector2Int>();
            start = Random.Range(0, 2) == 1 ? new Vector2Int(Random.Range(256, 768), Random.Range(0, 2) * 1023) :
                new Vector2Int(Random.Range(0, 2) * 1023, Random.Range(256, 768));
            end = Random.Range(0, 2) == 1 ? new Vector2Int(Random.Range(256, 768), Random.Range(0, 2) * 1023) :
                new Vector2Int(Random.Range(0, 2) * 1023, Random.Range(256, 768));
            while(end.x==start.x||end.y ==start.y)
            {
                end = Random.Range(0, 2) == 1 ? new Vector2Int(Random.Range(256, 768), Random.Range(0, 2) * 1023) :
                    new Vector2Int(Random.Range(0, 2) * 1023, Random.Range(256, 768));
            }

            //end.x = start.y;
            //end.y = start.x;
            startPlace = start; 
            newLine.Add(start);
            while(start!=end)
            {
                start.y = (int)Mathf.Lerp(startPlace.y, end.y, (float)(newLine.IndexOf(start) + 1) * 1 / Mathf.Abs(startPlace.y - end.y));
                // if (start.x!=end.x)
                newLine.Add(start);
                start.x = (int)Mathf.Lerp(startPlace.x, end.x, (float)(newLine.IndexOf(start) + 1)* 1/Mathf.Abs(startPlace.x-end.x));
                //  if (start.y != end.y)
                newLine.Add(start);

                //  Debug.Log((int)Mathf.Lerp(startPlace.x, end.x, (newLine.IndexOf(start) + 2) / Mathf.Abs(end.x - startPlace.x)));
                //   Debug.Log(start);
            }
            newLine.Add(end);
            lines.Add(newLine);

        }

    }

    void PaintLines()
    {
        foreach (var l in lines)
            foreach (var ll in l)
                MapToMake.SetPixel(ll.x, ll.y, Color.black);
    }

    void DefineIntersections()
    {
        foreach (var line in lines)
            foreach (var l in line)
                foreach (var linesAgain in lines)
                    if (linesAgain != line)
                        if (linesAgain.Contains(l))
                        {
                            if (!intersectionPoints.ContainsKey(l))
                                intersectionPoints.Add(l, new List<List<Vector2Int>>() { linesAgain, line });
                            else intersectionPoints[l].Add(linesAgain);
                        }


    }
}
