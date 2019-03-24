using System;
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

    public Node GetRandomNode()
    {
        return nodes[Mathf.FloorToInt(UnityEngine.Random.Range(0, nodes.Count))];
    }

    public void DeugDrawGrahp()
    {
        foreach(Node node in nodes)
        {
            foreach(Node neighbors in node.neighbors)
            {
                Debug.DrawLine(node.center, neighbors.center, Color.magenta, 100000, false);
            }
    
        }
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)nodes).GetEnumerator();
    }

    public class Node : IComparable
    {
        public Vector3 center
        {
             get; set;
        }

        public float area
        {
            get; set;
        }

        /** id of the neibors in Map.nodes **/
        public SortedSet<Node> neighbors
        {
            get; protected set;
        }

        public Node(Vector3 vector3)
        {
            this.center = vector3;
            neighbors = new SortedSet<Node>();
        }

        public void AddEdge(Node n)
        {
            neighbors.Add(n);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Node)) return -1;
            Node node = (Node)obj;

            return Mathf.FloorToInt((node.center - center).sqrMagnitude);
            
        }
    }

}
