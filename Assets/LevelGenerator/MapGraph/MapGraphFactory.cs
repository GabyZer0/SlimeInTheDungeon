using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Facotoryu to create MapGraphFactory **/
public abstract class MapGraphFactory
{
    /** Final nodes that will be used to create the graph **/
    private static List<MapGraph.Node> nodes;

    /** Each nodes has 3 vertices, the n-th vertices are on vertices[n*3], vertices[n*3+1] and vertices[n*3+2] **/
    private static List<Vector3> vertices;

    /** Area of each triangle **/
    private static List<float> areas;

    public static void Init()
    {
        nodes = new List<MapGraph.Node>();
        vertices = new List<Vector3>();
        areas = new List<float>();
    }

    public static void AddNode(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 barycenter = (a + b + c) / 3f;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        MapGraph.Node node = new MapGraph.Node(barycenter);

        for(int i = 0; i < nodes.Count; i++)
        {
            int n = 3*i;
            if(vertices[n].Equals(a) || vertices[n].Equals(b) || vertices[n].Equals(c)
                || vertices[n+1].Equals(a) || vertices[n+1].Equals(b) || vertices[n+1].Equals(c)
                || vertices[n+2].Equals(a) || vertices[n+2].Equals(b) || vertices[n+2].Equals(c))
            {
                node.AddEdge(i);
                nodes[i].AddEdge(nodes.Count);
                Debug.Assert(i != nodes.Count);
            }
        }
        nodes.Add(node);

        float area = Mathf.Abs(a.x * (b.z - c.z) +
                               b.x * (a.z - c.z) +
                               c.x * (a.z - b.z));


        areas.Add(area);
    }

    /**
     * Remove the node corresponging to the id and fusion it with one of its neighbors 
     **/
    public static void RemoveNodeWithId(int id)
    {
        int newId = nodes[id].neighbors.Min;
        Debug.Assert(id != newId);
        areas[newId] += areas[id];

        nodes[newId].neighbors.Remove(id); //Removing id of the old in the new

        foreach(int i in nodes[id].neighbors) //Replace the neigbors of the old by the new and add these neighors to the new
        {
            Debug.Assert(id != i);
            if (newId == i) continue;
            nodes[i].neighbors.Remove(id); //Remove the node that will be erased
            nodes[i].neighbors.Add(newId); //Replace with the one that will replace it
            nodes[newId].neighbors.Add(i); //Add the old neighbors to the new node
        }

        nodes.RemoveAt(id);
        vertices.RemoveRange(3 * id, 3);
        areas.RemoveAt(id);

        //Update the ids that have been modified (id+1 to max)
        List<int> tmp = new List<int>();
        for(int i = 0; i < nodes.Count; i++)
        {
            tmp.Clear();
            foreach(int n in nodes[i].neighbors)
            {
                if (n > id)
                {
                    tmp.Add(n);
                }
            }

            foreach(int n in tmp)
            {
                nodes[i].neighbors.Remove(n);  //is there a better way to do it ?
                Debug.Assert(i != (n - 1));
                nodes[i].neighbors.Add(n - 1);
            }
        }

    }

    /** Remove the nodes representing triangle of a too little area, Naive way to optimize
     * /!\ Not safe with an min_area thaht is too small /!\ 
     * Will probably never be used 
     **/
    public static void RemoveNodesWithMinArea(float min_area)
    {
        bool did_something = false;
        //should begin with the last one, so there will be not too much memory deplacement
        for (int i = nodes.Count-1;i>=0;i--)
        {
            if(areas[i]<min_area)
            {
                RemoveNodeWithId(i);
                did_something = true;
                break; //since id changed, we start again
            }
        }

        if (did_something) RemoveNodesWithMinArea(min_area);
    }


    public static MapGraph GetGraph()
    { 
        vertices.Clear();
        areas.Clear();
        return new MapGraph(nodes);
    }
}
