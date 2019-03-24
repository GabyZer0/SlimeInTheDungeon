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

    /**Prefab enemy **/
    public GameObject p_enemy1;

    /** NavMesh for level generation **/
    public NavMeshSurface surface;

    /** NavMesh for Movement of enemy **/
    public NavMeshSurface surface_enemy;

    //TODO: argument for size, enemy etc for the generation

    private int seed;

    public const int MIN_WALL = 5;
    public const int MAX_WALL = 7; //can change this to create a geant map


    // Start is called before the first frame update
    void Start()
    {
        float begin = Time.realtimeSinceStartup;
        this.seed = Mathf.FloorToInt(Random.value * int.MaxValue);
        Random.InitState(seed);
            
        MapGeneration();
        surface.BuildNavMesh();

        surface.AddData(); //Add the surface to the singleton Nav Mesh

        
        NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();
        DebugDisplayTriangle(navMeshTriangulation);
        

        MapGraph mapGraph = GenerateGraph(navMeshTriangulation);
        
        mapGraph.DeugDrawGrahp();

        Debug.Log("Generation took " + (Time.realtimeSinceStartup-begin) + " seconds");
        GenerateLights(mapGraph);

        TemporaryPlayerGeneration(mapGraph);

        surface.RemoveData(); //Remove the surface to the singleton NavMesh

        surface_enemy.BuildNavMesh();
        GenerateEnemies(5, mapGraph);


        gameObject.BroadcastMessage("StartTheGame");
    }

    private void GenerateEnemies(int nb, MapGraph map)
    {
        for(int i=0;i<nb;i++)
        {
            Vector3 pos =  map.GetRandomNode().center;
            pos.y = 1;

            GameObject enemy = Instantiate(p_enemy1, pos, new Quaternion(),transform); 
            enemy.SetActive(false);

            Golem1Controller controller = enemy.GetComponent<Golem1Controller>();
            if(controller != null)
            {
                int nbPatrol = 4;
                Vector3[] patrol = new Vector3[nbPatrol];
                for(int p=0;p<nbPatrol;p++)
                {
                    patrol[p] = map.GetRandomNode().center;
                }
                controller.SetPatrol(patrol);
                controller.enabled = false;
                enemy.SetActive(true);
            }
        }
    }

    private void DebugDisplayTriangle(NavMeshTriangulation navMeshTriangulation)
    {
        int[] ind = navMeshTriangulation.indices;

        for (int i = 0; i < ind.Length; i += 3)
        {
            Vector3 b = (navMeshTriangulation.vertices[ind[i]] + navMeshTriangulation.vertices[ind[i + 1]] + navMeshTriangulation.vertices[ind[i + 2]]) / 3f;

            /*Debug.DrawLine(b, navMeshTriangulation.vertices[ind[i]], Color.red, 10000, false);
            Debug.DrawLine(b, navMeshTriangulation.vertices[ind[i + 1]], Color.blue, 10000, false);
            Debug.DrawLine(b, navMeshTriangulation.vertices[ind[i + 2]], Color.green, 10000, false);*/


            Debug.DrawLine(navMeshTriangulation.vertices[ind[i]], navMeshTriangulation.vertices[ind[i + 1]], Color.white, 10000, false);
            Debug.DrawLine(navMeshTriangulation.vertices[ind[i]], navMeshTriangulation.vertices[ind[i + 2]], Color.white, 10000, false);
            Debug.DrawLine(navMeshTriangulation.vertices[ind[i + 1]], navMeshTriangulation.vertices[ind[i + 2]], Color.white, 100000, false);
        }
    }

    private void GenerateLights(MapGraph mapGraph)
    {
        GameObject lights = new GameObject
        {
            name = "Lights"
        };

        //should look if we can bake the light at the beginning
        foreach (MapGraph.Node node in mapGraph)
        {
            if (NavMesh.FindClosestEdge(node.center, out NavMeshHit hit, NavMesh.AllAreas))
            {
                Collider[] res = Physics.OverlapSphere(hit.position, 10);
                bool adding = true;
                foreach (Collider c in res)
                {
                    if (c.gameObject.GetComponent<Light>() != null || c.gameObject.GetComponentInChildren<Light>() != null)
                    {
                        adding = false;
                    }
                }

                if (adding)
                {
                    //adapt the position to the prefab
                    Vector3 position = hit.position;
                    position.y = (MIN_WALL + MAX_WALL) / 4f + Random.Range(-0.5f, 0.5f);
                    Instantiate(p_lamp, position, Quaternion.AngleAxis(30, new Vector3(hit.normal.z, 0, hit.normal.x)), lights.transform); //TODO: Rotation is not okay
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
        Debug.Log("Generating graph");
        MapGraphFactory.Init();
        int[] ind = navMeshTriangulation.indices;
        Debug.Assert(ind.Length % 3 == 0);
        for (int i = 0; i < ind.Length; i += 3)
        {
            MapGraphFactory.AddNode(navMeshTriangulation.vertices[ind[i]], navMeshTriangulation.vertices[ind[i+1]], navMeshTriangulation.vertices[ind[i+2]]);
        }

        MapGraphFactory.CreateEdges();
        //MapGraphFactory.RemoveNodesWithMinArea(500f);

        Debug.Log("The end");
        return MapGraphFactory.GetGraph();

    }

    private void MapGeneration()
    {
        CaveGeneratorFactory factory = new CaveGeneratorFactory();

        ThreeTierCaveConfiguration ttcc = new ThreeTierCaveConfiguration();
        ttcc.CaveType = ThreeTierCaveType.Enclosed;

        AKSaigyouji.Modules.HeightMaps.HeightMapRocky ceiling_gen = new AKSaigyouji.Modules.HeightMaps.HeightMapRocky();
        AKSaigyouji.HeightMaps.LayeredNoiseParameters ceiling_param = new AKSaigyouji.HeightMaps.LayeredNoiseParameters();
        ceiling_param.SetHeightRange(MIN_WALL, MAX_WALL);
        ceiling_param.SetSmoothness(10);
        ceiling_param.SetLayers(4, 0.5f, 2f);
        ceiling_gen.Properties = ceiling_param;
        ttcc.CeilingHeightMapModule = ceiling_gen;
        ttcc.CeilingMaterial = ceiling;

        AKSaigyouji.Modules.HeightMaps.HeightMapConstant floor_gen = new AKSaigyouji.Modules.HeightMaps.HeightMapConstant();
        floor_gen.Height = 0;
        ttcc.FloorHeightMapModule = floor_gen;
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
