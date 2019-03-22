using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AKSaigyouji.CaveGeneration;

/** Procedural generation of the level. Everything should be generated here **/
public class LevelGenerator : MonoBehaviour
{
    /** Materials of the cave **/
    public Material ceiling, floor, wall;

    /** Prefab Lights **/
    public GameObject p_lamp;

    /** Prefab player **/
    public GameObject p_player;

    public NavMeshSurface surface;

    private int seed;


    // Start is called before the first frame update
    void Start()
    {
        this.seed = Mathf.FloorToInt(Random.value * int.MaxValue);
        Random.InitState(seed);
            
        MapGeneration();
        surface.BuildNavMesh();

        surface.AddData(); //Add the surface to the singleton Nav Mesh

        NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();

        MapGraph mapGraph = GenerateGraph(navMeshTriangulation);

        //mapGraph.DeugDrawGrahp();

        GenerateLights(mapGraph);

        TemporaryPlayerGeneration(mapGraph);

        surface.RemoveData(); //Remove the surface to the singleton NavMesh
    }

    private void DebugDisplayTriangle(NavMeshTriangulation navMeshTriangulation)
    {
        int[] ind = navMeshTriangulation.indices;

        for (int i = 0; i < ind.Length; i += 3)
        {
            Debug.DrawLine(navMeshTriangulation.vertices[ind[i]], navMeshTriangulation.vertices[ind[i + 1]], Color.red, 10000, false);
            Debug.DrawLine(navMeshTriangulation.vertices[ind[i]], navMeshTriangulation.vertices[ind[i + 2]], Color.red, 10000, false);
            Debug.DrawLine(navMeshTriangulation.vertices[ind[i + 1]], navMeshTriangulation.vertices[ind[i + 2]], Color.red, 100000, false);
        }
    }

    private void GenerateLights(MapGraph mapGraph)
    {
        //should look if we can bake the light at the beginning
        foreach(MapGraph.Node node in mapGraph)
        {
            NavMeshHit hit;
            if(NavMesh.FindClosestEdge(node.center, out hit,NavMesh.AllAreas))
            {
                Collider[] res = Physics.OverlapSphere(hit.position, 10);
                bool adding = true;
                foreach(Collider c in res)
                {
                    if(c.gameObject.GetComponent<Light>()!=null || c.gameObject.GetComponentInChildren<Light>() !=null)
                    {
                        adding = false;
                    }
                }

                if (adding)
                {
                    //adapt the position to the prefab
                    Vector3 position = hit.position;
                    position.y = 1.5f+Random.Range(-0.5f,0.5f);
                    Debug.Log(hit.normal);
                    Instantiate(p_lamp, position, Quaternion.AngleAxis(30,new Vector3(hit.normal.z,0,hit.normal.x)));
                }
            }
        }
    }

    private void TemporaryPlayerGeneration(MapGraph mapGraph)
    {
        Vector3 pos = mapGraph.GetNode(0).center;

        pos.y = 1;

        GameObject go = Instantiate(p_player, pos, new Quaternion());
        go.tag = "Player";
    }

    private MapGraph GenerateGraph(NavMeshTriangulation navMeshTriangulation)
    {
        MapGraphFactory.Init();
        int[] ind = navMeshTriangulation.indices;

        for (int i = 0; i < ind.Length; i += 3)
        {
            MapGraphFactory.AddNode(navMeshTriangulation.vertices[ind[i]], navMeshTriangulation.vertices[ind[i+1]], navMeshTriangulation.vertices[ind[i+2]]);
        }

        //MapGraphFactory.RemoveNodesWithMinArea(50f);

        return MapGraphFactory.GetGraph();

    }

    private void MapGeneration()
    {
        CaveGeneratorFactory factory = new CaveGeneratorFactory();

        ThreeTierCaveConfiguration ttcc = new ThreeTierCaveConfiguration();
        ttcc.CaveType = ThreeTierCaveType.Enclosed;
        ttcc.CeilingHeightMapModule = new AKSaigyouji.Modules.HeightMaps.HeightMapRocky();
        ttcc.CeilingMaterial = ceiling;
        ttcc.FloorHeightMapModule = new AKSaigyouji.Modules.HeightMaps.HeightMapConstant();
        ttcc.FloorMaterial = floor;
        ttcc.WallModule = new AKSaigyouji.Modules.CaveWalls.CaveWallFlat();
        ttcc.WallMaterial = wall;
        AKSaigyouji.Modules.MapGeneration.MapGenCellAutomata map_gen = new AKSaigyouji.Modules.MapGeneration.MapGenCellAutomata();
        AKSaigyouji.Modules.MapGeneration.MapParameters param = new AKSaigyouji.Modules.MapGeneration.MapParameters
        {
            Length = 100,
            Width = 100,
            InitialDensity = 0.5f,
            ExpandTunnels = true,
            BorderSize = 1,
            MinWallSize = 15,
            MinFloorSize = 15
        };
        map_gen.Properties = param;
        ttcc.MapGenerator = map_gen;
        ttcc.Scale = 2;

        ttcc.SetSeed(seed);

        CaveGenerator cg = factory.BuildThreeTierCaveGen(ttcc);
        //CaveGenerator cg = new MyCaveGenerator(ttcc);

        GameObject map = cg.Generate();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
