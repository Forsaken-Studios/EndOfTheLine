using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Range(1, 23)]
    public int StartStation;
    [Range(1, 23)]
    public int EndStation;

    public bool runPathfinding = false;
    
    private MapNode StartLocation { get; set;}
    private MapNode EndLocation { get; set;}
    private Map map;
    
    private Material mat;

    private Vector2 currentCursorLocation = new Vector2(77.5f, 111.5f);
    private int currentPathIndex = 0;
    
    [Range(0.01f, 25f)]
    [SerializeField] private float indicatorDecay = 12f;

    private bool loadNewPath=false;
    public float indicatorThreshhold = 0.005f;
    
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        map = Map.MapInstance;
    }

    public void ResetMap()
    {
        foreach (var mapNode in map.stations)
        {
            mapNode.Visited = false;
            mapNode.ParentMapNode = null;
        }
        SetSearchParameters(StartStation,EndStation);
    }

    private void SetSearchParameters(int start, int end)
    {
        StartLocation = map.stations[map.StationNumber(start)];
        EndLocation = map.stations[map.StationNumber(end)];
    }

    private bool Search(MapNode currentNode)
    {
        currentNode.Visited = true;
        List<MapNode> nextNodes = new List<MapNode>();
        
        foreach (var edge in currentNode.Connections)
        {
            if (!edge.ConnectedNode.Visited)
            {
                edge.ConnectedNode.ParentMapNode = currentNode;
                nextNodes.Add(edge.ConnectedNode);
            }
        }
        nextNodes.Sort((x,y) => x.StraightLineDistanceTo(EndLocation).CompareTo(y.StraightLineDistanceTo(EndLocation)));
        foreach (var nextNode in nextNodes)
        {
            if (nextNode.Location == this.EndLocation.Location)
            {
                return true;
            }
            else
            {
                if (Search(nextNode))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public List<MapNode> FindPath()
    {
        List<MapNode> path = new List<MapNode>();
        bool success = Search(StartLocation);

        if (success)
        {
            MapNode node = this.EndLocation;
            while (node.ParentMapNode != null)
            {
                path.Add(node);
                Edge connectionToParent = new Edge();
                
                
                foreach (var connection in node.Connections)
                {
                    if (connection.ConnectedNode.Equals(node.ParentMapNode))
                    {
                        connectionToParent = connection;
                    }
                }
                foreach (var aux in connectionToParent.Auxiliars)
                {
                    path.Add(aux);
                }
                    
                node = node.ParentMapNode;
            }
            path.Reverse();
        }
        
        return path;
    } 
    
    Vector2 expDecay(Vector2 current, Vector2 goal, float decay, float dT)
    {
        //Advanced LERP function
        return goal + (current - goal)* Mathf.Exp(-decay * dT);
    }
    
    // Update is called once per frame
    void Update()
    {
        List<MapNode> path = new List<MapNode>();
        if (runPathfinding)
        {
            ResetMap(); //This also sets Start and Endpoint
            currentCursorLocation = StartLocation.Location;
            currentPathIndex = 0;
            loadNewPath = true;
            path = FindPath();
            Debug.Log("Start: ID " + StartLocation.ID);
            int counter = 0;
            foreach (var coord in path)
            {
                counter++;
                Debug.Log("Coord "+counter+": ID " + coord.ID);
            }
            runPathfinding = false;
        }
        
        mat.SetVector("_IndicatorCenter", currentCursorLocation);
        /*
        if (loadNewPath)
        {
            currentCursorLocation.x = expDecay(currentCursorLocation.x, path[currentPathIndex].Location.x,
                indicatorDecay, Time.deltaTime);
            currentCursorLocation.y = expDecay(currentCursorLocation.y, path[currentPathIndex].Location.y,
                indicatorDecay, Time.deltaTime);

            
            if (Vector2.Distance(currentCursorLocation, path[currentPathIndex].Location) < indicatorThreshhold)
            {
                currentCursorLocation = path[currentPathIndex].Location;
                
                if (currentPathIndex >= path.Count-1)
                {
                    loadNewPath = false;
                }
                else
                {
                    currentPathIndex++;
                }
                
            }
            

        }*/
    }
}
