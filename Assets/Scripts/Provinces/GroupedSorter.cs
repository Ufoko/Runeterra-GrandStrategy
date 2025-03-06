using UnityEngine;
using System.Collections.Generic;
using System.IO;
using csDelaunay;

public class GroupedSorter : MonoBehaviour
{
    public static GroupedSorter singleton;
    [SerializeField] bool enableNames = false;
    public Texture2D mapColorTexture;
    public Texture2D mapTexture;
    [SerializeField] private GameObject province;
    // The number of polygons/sites we want
    public int polygonNumber = 200;
    [SerializeField] private GameObject provinceHolder;
    private List<ProvinceScript> provinces = new List<ProvinceScript>();

    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;
    int[,] pixelRegions;
    string[] names;

    private void Awake()
    {
        singleton = this;


        /*    tempString = Directory.GetCurrentDirectory();

            tempString += "\\Assets\\ProvinceNames.txt";
            Debug.Log(tempString);
        */
        /*Debug.Log(Resources.Load<TextAsset>("ProvinceNames").text);

        for (int i = 0; i < provinceNames.Length; i++)
        {
            provinceNames[i] = File.ReadAllLines(Resources.Load<TextAsset>("ProvinceNames").text)[i];
        }*/


        

        TextAsset nameFile = Resources.Load<TextAsset>("ProvinceNames");
        names = nameFile.text.Split("\n");

    }

    public void GenerateMap(int seed)
    {
        // Store the old random state
        Random.State oldRngState = Random.state;
        // Set a new random state, with a preset seed
        Random.InitState(seed);

        pixelRegions = new int[512,512];

        // Create your sites (lets call that the center of your polygons)
        List<Vector2f> points = CreateRandomPoint();

        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        Rectf bounds = new Rectf(0, 0, 512, 512);

        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        Voronoi voronoi = new Voronoi(points, bounds, 5);

        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);

        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;

        DisplayVoronoiDiagram();

        // Reset the random state to the old one
        Random.state = oldRngState;
    }

    private List<Vector2f> CreateRandomPoint()
    {
        // Use Vector2f, instead of Vector2
        // Vector2f is pretty much the same than Vector2, but like you could run Voronoi in another thread
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonNumber; i++)
        {
            float tempRandomNumX = Random.Range(0, 512); 
            float tempRandomNumY = Random.Range(0, 512);
            points.Add(new Vector2f(tempRandomNumX, tempRandomNumY));
           
        }

        return points;
    }


    // Here is a very simple way to display the result using a simple bresenham line algorithm
    // Just attach this script to a quad
    private void DisplayVoronoiDiagram()
    {
        float[] pixelX = new float[polygonNumber];
        float[] pixelY = new float[polygonNumber];

        int k = 0;
        Texture2D tx = new Texture2D(mapTexture.width, mapTexture.height);
        tx.SetPixels(mapTexture.GetPixels());
        tx.Apply();
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
           // tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);
            pixelX[k] = kv.Key.x;
            pixelY[k] = kv.Key.y;
            GameObject tempProvince = Instantiate(province,Vector3.zero, Quaternion.identity, provinceHolder.transform);
            tempProvince.name = "Province: " + (k + 1);
            ProvinceScript tempProvinceScript = tempProvince.GetComponent<ProvinceScript>();
            tempProvinceScript.x = (int)(kv.Key.x);
            tempProvinceScript.z = (int)(kv.Key.y);


            if (enableNames)
            {
                tempProvinceScript.provinceName = GenerateProvinceName(k);
            }
            
            
            provinces.Add(tempProvinceScript);
            k++;
        }
        foreach (Edge edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.grey);
        }
        tx.Apply();

        GetComponent<Renderer>().material.mainTexture = tx;

        //læs xy værdier som et array
        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                float minumumDistance = Mathf.Infinity;
                int provinceNumber = 0;
                for (int i = 0; i < pixelX.Length; i++)
                {
                    float tempDistance = Vector2.Distance(new Vector2(x, y), new Vector2(pixelX[i],pixelY[i]));
                    if (tempDistance < minumumDistance)
                    {
                        minumumDistance = tempDistance;
                        provinceNumber = i;
                    }

                }
                pixelRegions[x, y] = provinceNumber;
            }
        }



        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                provinces[pixelRegions[x, y]].terrainColor.Add(mapColorTexture.GetPixel(x, y));
            }
        }

    }


    private string GenerateProvinceName(int k)
    {
        string tempName = "Bugged name";
        tempName = names[k];


       /* bool uniqe = true;
      int randomNumer;
        do
        {
            randomNumer = Random.Range(0, provinceNames.Length);
            tempName = provinceNames[randomNumer];
            for (int i = 0; i < provinces.Count; i++)
            {
                if (tempName == provinces[i].provinceName)
                {
                    uniqe = false;
                    break;
                }
                else
                {
                    uniqe = true;
                }
            }
        } while (!uniqe);
        */


        return tempName;
    }


    // Bresenham line algorithm
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0)
    {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}

