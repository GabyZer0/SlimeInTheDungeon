using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Grap representing a map
 **/
public class MapGraph : IEnumerable 
{
    private List<Node> nodes;

    public MapGraph()
    {
        nodes = new List<Node>();
    }

    public MapGraph(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public void AddNode(Node node)
    {
        nodes.Add(node);
    }

    public Node GetNode(int i)
    {
        return nodes[i];
    }

    public void DeugDrawGrahp()
    {
        foreach(Node node in nodes)
        {
            foreach(int id in node.neighbors)
            {
                Debug.DrawLine(node.center, nodes[id].center, Color.red, 100000, false);
            }
        }
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)nodes).GetEnumerator();
    }

    public class Node
    {
        public Vector3 center
        {
             get; protected set;
        }

        /** id of the neibors in Map.nodes **/
        public SortedSet<int> neighbors
        {
            get; protected set;
        }

        public Node(Vector3 vector3)
        {
            this.center = vector3;
            neighbors = new SortedSet<int>();
        }

        public void AddEdge(int i)
        {
            neighbors.Add(i);
        }


    }
}
