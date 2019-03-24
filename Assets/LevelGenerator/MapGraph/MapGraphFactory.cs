using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Facotoryu to create MapGraphFactory **/
public abstract class MapGraphFactory
{
    /** Final nodes that will be used to create the graph **/
    private static List<MapGraph.Node> nodes; 

    /** Each nodes has 3 vertices, the n-th vertices are on vertices[n*3], vertices[n*3+1] and vertices[n*3+2 **/
    private static List<Vector3> vertices;

    //private static List<Vector3[]> segments;

    /** Area of each triangle **/
   // private static List<float> areas;

    public static void Init()
    {
        nodes = new List<MapGraph.Node>();
        vertices = new List<Vector3>();
    }

    private static bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }

    private static bool TestEdges(Vector3 a, Vector3 b, Vector3 t) //probably simpler way
    {
        bool x = false, y = false, z = false;
        if(a.x<b.x)
        {
            x = (a.x <= t.x) && (t.x <= b.x);
        }
        else
        {
            x = (a.x >= t.x) && (t.x >= b.x);
        }

        if (a.y<b.y)
        {
            y = (a.y <= t.y) && (t.y <= b.y);
        }
        else
        {
            y = (a.y >= t.y) && (t.y >= b.y);
        }

        if (a.z<b.z)
        {
            z = (a.z <= t.z) && (t.z <= b.z);
        }
        else
        {
            z = (a.z >= t.z) && (t.z >= b.z);
        }

        return x && y && z;
    }

    /** Add the edges between the current node and the ones with whom their triangle shares an edge **/
    public static void AddEdges(MapGraph.Node node, Vector3 a, Vector3 b, Vector3 c)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            int n = 3 * i;
            //if a triangle share 2 points, they share an edge
            bool p1 = false, p2 = false, p3 = false;

            /*p1 = V3Equal(vertices[n],a) || V3Equal(vertices[n+1], a) || V3Equal(vertices[n+2], a);  
            p2 = V3Equal(vertices[n], b) || V3Equal(vertices[n + 1], b) || V3Equal(vertices[n + 2], b);
            p3 = V3Equal(vertices[n], c) || V3Equal(vertices[n + 1], c) || V3Equal(vertices[n + 2], c);*/
            p1 = (a == vertices[n] && (TestEdges(vertices[n], vertices[n + 1], a) || TestEdges(vertices[n], vertices[n + 2], a)))
              || (a == vertices[n + 1] && (TestEdges(vertices[n + 1], vertices[n], a) || TestEdges(vertices[n + 1], vertices[n + 2], a)))
              || (a == vertices[n + 2] && (TestEdges(vertices[n + 2], vertices[n], a) || TestEdges(vertices[n + 2], vertices[n + 1], a)));

            p2 = (b == vertices[n] && (TestEdges(vertices[n], vertices[n + 1], b) || TestEdges(vertices[n], vertices[n + 2], b)))
              || (b == vertices[n + 1] &&( TestEdges(vertices[n + 1], vertices[n], b) || TestEdges(vertices[n + 1], vertices[n + 2], b)))
              || (b == vertices[n + 2] && (TestEdges(vertices[n + 2], vertices[n], b) || TestEdges(vertices[n + 2], vertices[n + 1], b)));

            p3 = (c == vertices[n] &&( TestEdges(vertices[n], vertices[n + 1], c) || TestEdges(vertices[n], vertices[n + 2], c)))
              || (c == vertices[n + 1] &&( TestEdges(vertices[n + 1], vertices[n], c) || TestEdges(vertices[n + 1], vertices[n + 2], c)))
              || (c == vertices[n + 2] &&( TestEdges(vertices[n + 2], vertices[n], c) || TestEdges(vertices[n + 2], vertices[n + 1], c)));

            Debug.Assert(!(p1 && p2 && p3));

            if ((p1&&p2) || (p2&&p3) || (p3&&p1))
            {
                node.AddEdge(nodes[i]);
                nodes[i].AddEdge(node);
            }
        }
    }

    public static void TmpAddEdges(int id, Vector3 a, Vector3 b, Vector3 c)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (id == i) continue;
            int n = 3 * i;
            //if a triangle share 2 points, they share an edge
            bool p1 = false, p2 = false, p3 = false;

            p1 = V3Equal(vertices[n],a) || V3Equal(vertices[n+1], a) || V3Equal(vertices[n+2], a);  
            p2 = V3Equal(vertices[n], b) || V3Equal(vertices[n + 1], b) || V3Equal(vertices[n + 2], b);
            p3 = V3Equal(vertices[n], c) || V3Equal(vertices[n + 1], c) || V3Equal(vertices[n + 2], c);
            /*p1 = (a == vertices[n] && (TestEdges(vertices[n], vertices[n + 1], a) || TestEdges(vertices[n], vertices[n + 2], a)))
              || (a == vertices[n + 1] && (TestEdges(vertices[n + 1], vertices[n], a) || TestEdges(vertices[n + 1], vertices[n + 2], a)))
              || (a == vertices[n + 2] && (TestEdges(vertices[n + 2], vertices[n], a) || TestEdges(vertices[n + 2], vertices[n + 1], a)));

            p2 = (b == vertices[n] && (TestEdges(vertices[n], vertices[n + 1], b) || TestEdges(vertices[n], vertices[n + 2], b)))
              || (b == vertices[n + 1] && (TestEdges(vertices[n + 1], vertices[n], b) || TestEdges(vertices[n + 1], vertices[n + 2], b)))
              || (b == vertices[n + 2] && (TestEdges(vertices[n + 2], vertices[n], b) || TestEdges(vertices[n + 2], vertices[n + 1], b)));

            p3 = (c == vertices[n] && (TestEdges(vertices[n], vertices[n + 1], c) || TestEdges(vertices[n], vertices[n + 2], c)))
              || (c == vertices[n + 1] && (TestEdges(vertices[n + 1], vertices[n], c) || TestEdges(vertices[n + 1], vertices[n + 2], c)))
              || (c == vertices[n + 2] && (TestEdges(vertices[n + 2], vertices[n], c) || TestEdges(vertices[n + 2], vertices[n + 1], c)));*/


            if ((p1 && p2) || (p2 && p3) || (p3 && p1))
            {
                Debug.Assert(!(p1 && p2 && p3));

                nodes[id].AddEdge(nodes[i]);
                nodes[i].AddEdge(nodes[id]);
            }
        }
    }

    internal static void CreateEdges()
    {
        for(int i=0;i<nodes.Count;i++)
        {
            int n = 3 * i;
            TmpAddEdges(i, vertices[n], vertices[n + 1], vertices[n + 2]);
        }
    }

    public static void AddNode(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 barycenter = (a + b + c) / 3f;  
        MapGraph.Node node = new MapGraph.Node(barycenter);

        //AddEdges(node, a, b, c);

        float area = Mathf.Abs(a.x * (b.z - c.z) +
                               b.x * (a.z - c.z) +
                               c.x * (a.z - b.z));

        node.area = area;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        nodes.Add(node);
    }

    /**
     * Remove the node corresponging to the id and fusion it with one of its neighbors 
     **/
    public static void RemoveNode(int i)
    {
        //New Node
        MapGraph.Node fusion = nodes[i].neighbors.Min;

        if(fusion is null)
        {
            //ignore for the moment
            foreach (MapGraph.Node nei in nodes[i].neighbors)
            { 
                nei.neighbors.Remove(nodes[i]);
            }
            nodes.RemoveAt(i);
            return;
        }

        //Add the neighbors of the deleted node to the new node and deleting node from neighbors
        fusion.neighbors.Remove(nodes[i]);
        foreach (MapGraph.Node nei in nodes[i].neighbors)
        {
            if (nei == fusion) continue;

            fusion.neighbors.Add(nei);
            nei.neighbors.Remove(nodes[i]);
        }

        Vector3 newCenter = (nodes[i].center + fusion.center) / 2.0f; ;
        float newArea = nodes[i].area+fusion.area;
       
        fusion.center = newCenter;
        fusion.area = newArea;
        nodes.RemoveAt(i);
    } 


    /** Remove the nodes representing triangle of a too little area, Naive way to optimize
     * /!\ Not safe with an min_area thaht is too small /!\ 
     * Will probably never be used 
     **/
    public static void RemoveNodesWithMinArea(float min_area)
    {
        int debug = 0;
        bool did_something;
        //should begin with the last one, so there will be not too much memory deplacement

        do
        {
            did_something = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                debug++;
                if (nodes[i].area < min_area)
                {
                    Debug.Log("Removing a node");
                    RemoveNode(i);
                    did_something = true;
                    break;
                }
            }
        } while (did_something);

    } 


    public static MapGraph GetGraph()
    { 
        vertices.Clear();
        return new MapGraph(nodes);
    }
}
