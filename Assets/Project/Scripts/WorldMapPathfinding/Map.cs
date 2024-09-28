using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map MapInstance;
    
    public List<MapNode> stations = new List<MapNode>();
    public List<MapNode> aux = new List<MapNode>();


    private void Awake()
    {
        if (MapInstance != null)
        {
            Debug.LogWarning("[Map.cs] : There is already a Map Instance");
            Destroy(this);
        }
        MapInstance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {

        //List of Stations
        stations.Add(new MapNode { ID = "st01", Location = new Vector2(77.5f, 111.5f)  , Visited = false});
        stations.Add(new MapNode { ID = "st02", Location = new Vector2(95.5f, 111.5f)  , Visited = false});
        stations.Add(new MapNode { ID = "st03", Location = new Vector2(111.5f, 105.5f) , Visited = false});
        stations.Add(new MapNode { ID = "st04", Location = new Vector2(38.5f, 104.5f)  , Visited = false});
        stations.Add(new MapNode { ID = "st05", Location = new Vector2(61.5f, 104.5f)  , Visited = false});
        stations.Add(new MapNode { ID = "st06", Location = new Vector2(84.5f, 96.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st07", Location = new Vector2(16.5f, 94.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st08", Location = new Vector2(66.5f, 88.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st09", Location = new Vector2(100.5f, 84.5f)  , Visited = false});
        stations.Add(new MapNode { ID = "st10", Location = new Vector2(25.5f, 77.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st11", Location = new Vector2(42.5f, 77.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st12", Location = new Vector2(80.5f, 77.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st13", Location = new Vector2(111.5f, 69.5f)  , Visited = false});
        stations.Add(new MapNode { ID = "st14", Location = new Vector2(51.5f, 60.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st15", Location = new Vector2(80.5f, 60.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st16", Location = new Vector2(16.5f, 51.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st17", Location = new Vector2(35.5f, 44.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st18", Location = new Vector2(65.5f, 44.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st19", Location = new Vector2(97.5f, 44.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st20", Location = new Vector2(16.5f, 37.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st21", Location = new Vector2(51.5f, 29.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st22", Location = new Vector2(79.5f, 19.5f)   , Visited = false});
        stations.Add(new MapNode { ID = "st23", Location = new Vector2(103.5f, 19.5f)  , Visited = false});
    
        
        //List of Connections between stations
        stations[StationNumber(1)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(2)]});
        stations[StationNumber(1)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(5)]});
        
        stations[StationNumber(2)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(1)]});
        stations[StationNumber(2)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(3)]});
        
        stations[StationNumber(3)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(2)]});
        stations[StationNumber(3)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(9)]});
        
        stations[StationNumber(4)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(5)]});
        stations[StationNumber(4)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(7)]});
        stations[StationNumber(4)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(10)]});
        
        stations[StationNumber(5)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(1)]});
        stations[StationNumber(5)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(4)]});
        stations[StationNumber(5)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(6)]});
        stations[StationNumber(5)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(11)]});
        
        stations[StationNumber(6)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(5)]});
        stations[StationNumber(6)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(8)]});
        stations[StationNumber(6)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(12)]});
        
        stations[StationNumber(7)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(4)]});
        stations[StationNumber(7)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(10)]}); //Weird corner?
        stations[StationNumber(7)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(16)]});
        
        stations[StationNumber(8)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(6)]});
        stations[StationNumber(8)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(12)]});
        stations[StationNumber(8)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(14)]});
        
        stations[StationNumber(9)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(3)]});
        stations[StationNumber(9)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(12)]});
        
        stations[StationNumber(10)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(4)]});
        stations[StationNumber(10)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(7)]}); //Weird Corner?
        stations[StationNumber(10)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(11)]});
        stations[StationNumber(10)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(16)]});
        
        stations[StationNumber(11)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(5)]});
        stations[StationNumber(11)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(10)]});
        stations[StationNumber(11)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(12)]});
        stations[StationNumber(11)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(17)]});
        
        stations[StationNumber(12)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(6)]});
        stations[StationNumber(12)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(8)]});
        stations[StationNumber(12)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(9)]});
        stations[StationNumber(12)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(11)]});
        stations[StationNumber(12)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(15)]});
        
        stations[StationNumber(13)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(15)]});
        
        stations[StationNumber(14)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(8)]});
        stations[StationNumber(14)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(15)]});
        stations[StationNumber(14)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(17)]});
        stations[StationNumber(14)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(18)]});
        
        stations[StationNumber(15)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(12)]});
        stations[StationNumber(15)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(13)]});
        stations[StationNumber(15)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(14)]});
        stations[StationNumber(15)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(19)]});
        
        stations[StationNumber(16)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(7)]});
        stations[StationNumber(16)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(10)]});
        stations[StationNumber(16)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(17)]});
        stations[StationNumber(16)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(20)]}); //Weird corner?
        
        stations[StationNumber(17)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(11)]});
        stations[StationNumber(17)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(14)]});
        stations[StationNumber(17)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(16)]});
        stations[StationNumber(17)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(20)]});
        //stations[stationNumber(17)].Connections.Add(new Edge{ConnectedNode = stations[stationNumber(20)]}); //Weird corner + Doubled + longer
        stations[StationNumber(17)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(21)]});
        
        stations[StationNumber(18)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(14)]});
        stations[StationNumber(18)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(19)]});
        //stations[stationNumber(18)].Connections.Add(new Edge{ConnectedNode = stations[stationNumber(19)]}); //Doubled + longer
        stations[StationNumber(18)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(21)]});
        
        stations[StationNumber(19)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(15)]});
        stations[StationNumber(19)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(18)]});
        //stations[stationNumber(19)].Connections.Add(new Edge{ConnectedNode = stations[stationNumber(18)]}); //Doubled + longer
        stations[StationNumber(19)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(21)]});
        
        stations[StationNumber(20)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(16)]}); //Weird corner?
        stations[StationNumber(20)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(17)]});
        //stations[stationNumber(20)].Connections.Add(new Edge{ConnectedNode = stations[stationNumber(17)]}); //Doubled + longer
        stations[StationNumber(20)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(21)]}); 
        
        stations[StationNumber(21)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(17)]});
        stations[StationNumber(21)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(18)]});
        stations[StationNumber(21)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(19)]});
        stations[StationNumber(21)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(20)]});
        stations[StationNumber(21)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(22)]});
        
        stations[StationNumber(22)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(21)]});
        stations[StationNumber(22)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(23)]});
        
        stations[StationNumber(23)].Connections.Add(new Edge{ConnectedNode = stations[StationNumber(22)]});
        
        //List of auxilliar nodes
        
        aux.Add(new MapNode{ID= "aux01", Location = new Vector2(40.5f,  111.5f)});
        aux.Add(new MapNode{ID= "aux02", Location = new Vector2(59.5f,  111.5f)});
        aux.Add(new MapNode{ID= "aux03", Location = new Vector2(38.5f,  109.5f)});
        aux.Add(new MapNode{ID= "aux04", Location = new Vector2(61.5f,  109.5f)});
        aux.Add(new MapNode{ID= "aux05", Location = new Vector2(20.5f,  104.5f)});
        aux.Add(new MapNode{ID= "aux06", Location = new Vector2(16.5f,  101.5f)});
        aux.Add(new MapNode{ID= "aux07", Location = new Vector2(38.5f,  98.5f)});
        aux.Add(new MapNode{ID= "aux08", Location = new Vector2(61.5f,  98.5f)});
        aux.Add(new MapNode{ID= "aux09", Location = new Vector2(40.5f,  96.5f)});
        aux.Add(new MapNode{ID= "aux10", Location = new Vector2(103.5f, 94.5f)});
        aux.Add(new MapNode{ID= "aux11", Location = new Vector2(108.5f, 94.5f)});
        aux.Add(new MapNode{ID= "aux12", Location = new Vector2(38.5f,  93.5f)});
        aux.Add(new MapNode{ID= "aux13", Location = new Vector2(84.5f,  90.5f)});
        aux.Add(new MapNode{ID= "aux14", Location = new Vector2(100.5f, 90.5f)});
        aux.Add(new MapNode{ID= "aux15", Location = new Vector2(28.5f,  89.5f)});
        aux.Add(new MapNode{ID= "aux16", Location = new Vector2(35.5f,  89.5f)});
        aux.Add(new MapNode{ID= "aux17", Location = new Vector2(62.5f,  88.5f)});
        aux.Add(new MapNode{ID= "aux18", Location = new Vector2(80.5f,  88.5f)});
        aux.Add(new MapNode{ID= "aux19", Location = new Vector2(25.5f,  83.5f)});
        aux.Add(new MapNode{ID= "aux20", Location = new Vector2(42.5f,  83.5f)});
        aux.Add(new MapNode{ID= "aux21", Location = new Vector2(59.5f,  82.5f)});
        aux.Add(new MapNode{ID= "aux22", Location = new Vector2(100.5f, 78.5f)});
        aux.Add(new MapNode{ID= "aux23", Location = new Vector2(96.5f,  75.5f)});
        aux.Add(new MapNode{ID= "aux24", Location = new Vector2(59.5f,  69.5f)});
        aux.Add(new MapNode{ID= "aux25", Location = new Vector2(42.5f,  66.5f)});
        aux.Add(new MapNode{ID= "aux26", Location = new Vector2(53.5f,  66.5f)});
        aux.Add(new MapNode{ID= "aux27", Location = new Vector2(37.5f,  63.5f)});
        aux.Add(new MapNode{ID= "aux28", Location = new Vector2(40.5f,  63.5f)});
        aux.Add(new MapNode{ID= "aux29", Location = new Vector2(51.5f,  63.5f)});
        aux.Add(new MapNode{ID= "aux30", Location = new Vector2(25.5f,  60.5f)});
        aux.Add(new MapNode{ID= "aux31", Location = new Vector2(35.5f,  60.5f)});
        aux.Add(new MapNode{ID= "aux32", Location = new Vector2(111.5f, 58.5f)});
        aux.Add(new MapNode{ID= "aux33", Location = new Vector2(23.5f,  57.5f)});
        aux.Add(new MapNode{ID= "aux34", Location = new Vector2(16.5f,  55.5f)});
        aux.Add(new MapNode{ID= "aux35", Location = new Vector2(107.5f, 55.5f)});
        aux.Add(new MapNode{ID= "aux36", Location = new Vector2(80.5f,  48.5f)});
        aux.Add(new MapNode{ID= "aux37", Location = new Vector2(83.5f,  46.5f)});
        aux.Add(new MapNode{ID= "aux38", Location = new Vector2(94.5f,  46.5f)});
        aux.Add(new MapNode{ID= "aux39", Location = new Vector2(97.5f,  44.5f)});
        aux.Add(new MapNode{ID= "aux40", Location = new Vector2(51.5f,  43.5f)});
        aux.Add(new MapNode{ID= "aux41", Location = new Vector2(16.5f,  42.5f)});
        aux.Add(new MapNode{ID= "aux42", Location = new Vector2(21.5f,  39.5f)});
        aux.Add(new MapNode{ID= "aux43", Location = new Vector2(56.5f,  39.5f)});
        aux.Add(new MapNode{ID= "aux44", Location = new Vector2(16.5f,  36.5f)});
        aux.Add(new MapNode{ID= "aux45", Location = new Vector2(35.5f,  28.5f)});
        aux.Add(new MapNode{ID= "aux46", Location = new Vector2(97.5f,  28.5f)});
        aux.Add(new MapNode{ID= "aux47", Location = new Vector2(16.5f,  27.5f)});
        aux.Add(new MapNode{ID= "aux48", Location = new Vector2(20.5f,  24.5f)});
        aux.Add(new MapNode{ID= "aux49", Location = new Vector2(40.5f,  24.5f)});
        aux.Add(new MapNode{ID= "aux50", Location = new Vector2(65.5f,  24.5f)});
        aux.Add(new MapNode{ID= "aux51", Location = new Vector2(92.5f,  24.5f)});
        aux.Add(new MapNode{ID= "aux52", Location = new Vector2(51.5f,  17.5f)});
        aux.Add(new MapNode{ID= "aux53", Location = new Vector2(55.5f,  14.5f)});
        
        //Setting Auxilliars on Edges
        stations[StationNumber(1)].Connections[1].Auxiliars.Add(aux[StationNumber(1)]);
        stations[StationNumber(1)].Connections[1].Auxiliars.Add(aux[StationNumber(3)]);
        
        stations[StationNumber(2)].Connections[1].Auxiliars.Add(aux[StationNumber(2)]);
        stations[StationNumber(2)].Connections[1].Auxiliars.Add(aux[StationNumber(4)]);
        
        stations[StationNumber(3)].Connections[0].Auxiliars.Add(aux[StationNumber(4)]);
        stations[StationNumber(3)].Connections[0].Auxiliars.Add(aux[StationNumber(2)]);
        stations[StationNumber(3)].Connections[1].Auxiliars.Add(aux[StationNumber(8)]);
        stations[StationNumber(3)].Connections[1].Auxiliars.Add(aux[StationNumber(11)]);
        stations[StationNumber(3)].Connections[1].Auxiliars.Add(aux[StationNumber(10)]);
        stations[StationNumber(3)].Connections[1].Auxiliars.Add(aux[StationNumber(14)]);
        
        stations[StationNumber(4)].Connections[1].Auxiliars.Add(aux[StationNumber(5)]);
        stations[StationNumber(4)].Connections[1].Auxiliars.Add(aux[StationNumber(6)]);
        stations[StationNumber(4)].Connections[2].Auxiliars.Add(aux[StationNumber(12)]);
        stations[StationNumber(4)].Connections[2].Auxiliars.Add(aux[StationNumber(16)]);
        stations[StationNumber(4)].Connections[2].Auxiliars.Add(aux[StationNumber(15)]);
        stations[StationNumber(4)].Connections[2].Auxiliars.Add(aux[StationNumber(19)]);
        
        stations[StationNumber(5)].Connections[0].Auxiliars.Add(aux[StationNumber(3)]);
        stations[StationNumber(5)].Connections[0].Auxiliars.Add(aux[StationNumber(1)]);
        stations[StationNumber(5)].Connections[2].Auxiliars.Add(aux[StationNumber(7)]);
        stations[StationNumber(5)].Connections[2].Auxiliars.Add(aux[StationNumber(9)]);
        stations[StationNumber(5)].Connections[3].Auxiliars.Add(aux[StationNumber(20)]);
        
        stations[StationNumber(6)].Connections[0].Auxiliars.Add(aux[StationNumber(9)]);
        stations[StationNumber(6)].Connections[0].Auxiliars.Add(aux[StationNumber(7)]);
        stations[StationNumber(6)].Connections[1].Auxiliars.Add(aux[StationNumber(13)]);
        stations[StationNumber(6)].Connections[1].Auxiliars.Add(aux[StationNumber(18)]);
        stations[StationNumber(6)].Connections[2].Auxiliars.Add(aux[StationNumber(13)]);
        stations[StationNumber(6)].Connections[2].Auxiliars.Add(aux[StationNumber(18)]);
        
        stations[StationNumber(7)].Connections[0].Auxiliars.Add(aux[StationNumber(6)]);
        stations[StationNumber(7)].Connections[0].Auxiliars.Add(aux[StationNumber(5)]);
        stations[StationNumber(7)].Connections[1].Auxiliars.Add(aux[StationNumber(34)]);
        stations[StationNumber(7)].Connections[1].Auxiliars.Add(aux[StationNumber(33)]);
        stations[StationNumber(7)].Connections[1].Auxiliars.Add(aux[StationNumber(30)]);
        stations[StationNumber(7)].Connections[2].Auxiliars.Add(aux[StationNumber(34)]);
        
        stations[StationNumber(8)].Connections[0].Auxiliars.Add(aux[StationNumber(18)]);
        stations[StationNumber(8)].Connections[0].Auxiliars.Add(aux[StationNumber(13)]);
        stations[StationNumber(8)].Connections[1].Auxiliars.Add(aux[StationNumber(18)]);
        stations[StationNumber(8)].Connections[2].Auxiliars.Add(aux[StationNumber(17)]);
        stations[StationNumber(8)].Connections[2].Auxiliars.Add(aux[StationNumber(21)]);
        stations[StationNumber(8)].Connections[2].Auxiliars.Add(aux[StationNumber(24)]);
        stations[StationNumber(8)].Connections[2].Auxiliars.Add(aux[StationNumber(26)]);
        stations[StationNumber(8)].Connections[2].Auxiliars.Add(aux[StationNumber(29)]);
        
        stations[StationNumber(9)].Connections[0].Auxiliars.Add(aux[StationNumber(14)]);
        stations[StationNumber(9)].Connections[0].Auxiliars.Add(aux[StationNumber(10)]);
        stations[StationNumber(9)].Connections[0].Auxiliars.Add(aux[StationNumber(11)]);
        stations[StationNumber(9)].Connections[0].Auxiliars.Add(aux[StationNumber(8)]);
        stations[StationNumber(9)].Connections[1].Auxiliars.Add(aux[StationNumber(22)]);
        stations[StationNumber(9)].Connections[1].Auxiliars.Add(aux[StationNumber(23)]);
        
        stations[StationNumber(10)].Connections[0].Auxiliars.Add(aux[StationNumber(19)]);
        stations[StationNumber(10)].Connections[0].Auxiliars.Add(aux[StationNumber(15)]);
        stations[StationNumber(10)].Connections[0].Auxiliars.Add(aux[StationNumber(16)]);
        stations[StationNumber(10)].Connections[0].Auxiliars.Add(aux[StationNumber(12)]);
        stations[StationNumber(10)].Connections[1].Auxiliars.Add(aux[StationNumber(30)]);
        stations[StationNumber(10)].Connections[1].Auxiliars.Add(aux[StationNumber(33)]);
        stations[StationNumber(10)].Connections[1].Auxiliars.Add(aux[StationNumber(34)]);
        stations[StationNumber(10)].Connections[3].Auxiliars.Add(aux[StationNumber(30)]);
        stations[StationNumber(10)].Connections[3].Auxiliars.Add(aux[StationNumber(33)]);
        stations[StationNumber(10)].Connections[3].Auxiliars.Add(aux[StationNumber(34)]);
        
        stations[StationNumber(11)].Connections[0].Auxiliars.Add(aux[StationNumber(20)]);
        stations[StationNumber(11)].Connections[3].Auxiliars.Add(aux[StationNumber(25)]);
        stations[StationNumber(11)].Connections[3].Auxiliars.Add(aux[StationNumber(28)]);
        stations[StationNumber(11)].Connections[3].Auxiliars.Add(aux[StationNumber(27)]);
        stations[StationNumber(11)].Connections[3].Auxiliars.Add(aux[StationNumber(31)]);
        
        stations[StationNumber(12)].Connections[0].Auxiliars.Add(aux[StationNumber(18)]);
        stations[StationNumber(12)].Connections[0].Auxiliars.Add(aux[StationNumber(13)]);
        stations[StationNumber(12)].Connections[1].Auxiliars.Add(aux[StationNumber(18)]);
        stations[StationNumber(12)].Connections[2].Auxiliars.Add(aux[StationNumber(23)]);
        stations[StationNumber(12)].Connections[2].Auxiliars.Add(aux[StationNumber(22)]);
        
        stations[StationNumber(13)].Connections[0].Auxiliars.Add(aux[StationNumber(32)]);
        stations[StationNumber(13)].Connections[0].Auxiliars.Add(aux[StationNumber(35)]);
        
        stations[StationNumber(14)].Connections[0].Auxiliars.Add(aux[StationNumber(29)]);
        stations[StationNumber(14)].Connections[0].Auxiliars.Add(aux[StationNumber(26)]);
        stations[StationNumber(14)].Connections[0].Auxiliars.Add(aux[StationNumber(24)]);
        stations[StationNumber(14)].Connections[0].Auxiliars.Add(aux[StationNumber(27)]);
        stations[StationNumber(14)].Connections[3].Auxiliars.Add(aux[StationNumber(40)]);
        stations[StationNumber(14)].Connections[3].Auxiliars.Add(aux[StationNumber(43)]);
        
        stations[StationNumber(15)].Connections[1].Auxiliars.Add(aux[StationNumber(35)]);
        stations[StationNumber(15)].Connections[1].Auxiliars.Add(aux[StationNumber(32)]);
        stations[StationNumber(15)].Connections[3].Auxiliars.Add(aux[StationNumber(36)]);
        stations[StationNumber(15)].Connections[3].Auxiliars.Add(aux[StationNumber(37)]);
        stations[StationNumber(15)].Connections[3].Auxiliars.Add(aux[StationNumber(38)]);
        stations[StationNumber(15)].Connections[3].Auxiliars.Add(aux[StationNumber(39)]);
        
        stations[StationNumber(16)].Connections[1].Auxiliars.Add(aux[StationNumber(34)]);
        stations[StationNumber(16)].Connections[1].Auxiliars.Add(aux[StationNumber(33)]);
        stations[StationNumber(16)].Connections[1].Auxiliars.Add(aux[StationNumber(30)]);
        stations[StationNumber(16)].Connections[2].Auxiliars.Add(aux[StationNumber(41)]);
        stations[StationNumber(16)].Connections[2].Auxiliars.Add(aux[StationNumber(42)]);
        stations[StationNumber(16)].Connections[3].Auxiliars.Add(aux[StationNumber(41)]);
        stations[StationNumber(16)].Connections[3].Auxiliars.Add(aux[StationNumber(42)]);
        stations[StationNumber(16)].Connections[3].Auxiliars.Add(aux[StationNumber(44)]);
        
        stations[StationNumber(17)].Connections[0].Auxiliars.Add(aux[StationNumber(31)]);
        stations[StationNumber(17)].Connections[0].Auxiliars.Add(aux[StationNumber(27)]);
        stations[StationNumber(17)].Connections[0].Auxiliars.Add(aux[StationNumber(28)]);
        stations[StationNumber(17)].Connections[0].Auxiliars.Add(aux[StationNumber(25)]);
        stations[StationNumber(17)].Connections[2].Auxiliars.Add(aux[StationNumber(42)]);
        stations[StationNumber(17)].Connections[2].Auxiliars.Add(aux[StationNumber(41)]);
        stations[StationNumber(17)].Connections[3].Auxiliars.Add(aux[StationNumber(42)]);
        stations[StationNumber(17)].Connections[3].Auxiliars.Add(aux[StationNumber(44)]);
        //stations[stationNumber(17)].Connections[4].Auxiliars.Add(aux[stationNumber(45)]);
        //stations[stationNumber(17)].Connections[4].Auxiliars.Add(aux[stationNumber(49)]);
        //stations[stationNumber(17)].Connections[4].Auxiliars.Add(aux[stationNumber(48)]);
        //stations[stationNumber(17)].Connections[4].Auxiliars.Add(aux[stationNumber(47)]);
        stations[StationNumber(17)].Connections[4].Auxiliars.Add(aux[StationNumber(45)]);
        stations[StationNumber(17)].Connections[4].Auxiliars.Add(aux[StationNumber(49)]);

        stations[StationNumber(18)].Connections[0].Auxiliars.Add(aux[StationNumber(43)]);
        stations[StationNumber(18)].Connections[0].Auxiliars.Add(aux[StationNumber(40)]);
        //stations[stationNumber(18)].Connections[2].Auxiliars.Add(aux[stationNumber(50)]);
        //stations[stationNumber(18)].Connections[2].Auxiliars.Add(aux[stationNumber(51)]);
        //stations[stationNumber(18)].Connections[2].Auxiliars.Add(aux[stationNumber(46)]);
        stations[StationNumber(18)].Connections[2].Auxiliars.Add(aux[StationNumber(50)]);
        
        stations[StationNumber(19)].Connections[0].Auxiliars.Add(aux[StationNumber(39)]);
        stations[StationNumber(19)].Connections[0].Auxiliars.Add(aux[StationNumber(38)]);
        stations[StationNumber(19)].Connections[0].Auxiliars.Add(aux[StationNumber(37)]);
        stations[StationNumber(19)].Connections[0].Auxiliars.Add(aux[StationNumber(36)]);
        //stations[stationNumber(19)].Connections[2].Auxiliars.Add(aux[stationNumber(46)]);
        //stations[stationNumber(19)].Connections[2].Auxiliars.Add(aux[stationNumber(51)]);
        //stations[stationNumber(19)].Connections[2].Auxiliars.Add(aux[stationNumber(50)]);
        stations[StationNumber(19)].Connections[2].Auxiliars.Add(aux[StationNumber(46)]);
        stations[StationNumber(19)].Connections[2].Auxiliars.Add(aux[StationNumber(51)]);
        stations[StationNumber(19)].Connections[2].Auxiliars.Add(aux[StationNumber(50)]);
        
        stations[StationNumber(20)].Connections[0].Auxiliars.Add(aux[StationNumber(44)]);
        stations[StationNumber(20)].Connections[0].Auxiliars.Add(aux[StationNumber(42)]);
        stations[StationNumber(20)].Connections[0].Auxiliars.Add(aux[StationNumber(41)]);
        stations[StationNumber(20)].Connections[1].Auxiliars.Add(aux[StationNumber(44)]);
        stations[StationNumber(20)].Connections[1].Auxiliars.Add(aux[StationNumber(42)]);
        //stations[stationNumber(20)].Connections[2].Auxiliars.Add(aux[stationNumber(47)]);
        //stations[stationNumber(20)].Connections[2].Auxiliars.Add(aux[stationNumber(48)]);
        //stations[stationNumber(20)].Connections[2].Auxiliars.Add(aux[stationNumber(49)]);
        //stations[stationNumber(20)].Connections[2].Auxiliars.Add(aux[stationNumber(45)]);
        stations[StationNumber(20)].Connections[2].Auxiliars.Add(aux[StationNumber(47)]);
        stations[StationNumber(20)].Connections[2].Auxiliars.Add(aux[StationNumber(48)]);
        stations[StationNumber(20)].Connections[2].Auxiliars.Add(aux[StationNumber(49)]);
        
        stations[StationNumber(21)].Connections[0].Auxiliars.Add(aux[StationNumber(49)]);
        stations[StationNumber(21)].Connections[0].Auxiliars.Add(aux[StationNumber(45)]);
        stations[StationNumber(21)].Connections[1].Auxiliars.Add(aux[StationNumber(50)]);
        stations[StationNumber(21)].Connections[2].Auxiliars.Add(aux[StationNumber(50)]);
        stations[StationNumber(21)].Connections[2].Auxiliars.Add(aux[StationNumber(51)]);
        stations[StationNumber(21)].Connections[2].Auxiliars.Add(aux[StationNumber(46)]);
        stations[StationNumber(21)].Connections[3].Auxiliars.Add(aux[StationNumber(49)]);
        stations[StationNumber(21)].Connections[3].Auxiliars.Add(aux[StationNumber(48)]);
        stations[StationNumber(21)].Connections[3].Auxiliars.Add(aux[StationNumber(47)]);
        stations[StationNumber(21)].Connections[4].Auxiliars.Add(aux[StationNumber(52)]);
        stations[StationNumber(21)].Connections[4].Auxiliars.Add(aux[StationNumber(53)]);
        
        stations[StationNumber(22)].Connections[0].Auxiliars.Add(aux[StationNumber(53)]);
        stations[StationNumber(22)].Connections[0].Auxiliars.Add(aux[StationNumber(52)]);
        
        
        
    
}
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static Map GetMapInstance
    {
        get
        {
            return MapInstance;
        }
    }
    
    public int StationNumber(int station)
    {
        return station - 1;
    }
    
}

public class MapNode
{
    public string ID {get; set; }

    public List<Edge> Connections { get; set; } = new List<Edge>();
    public bool Visited { get; set;}
    public Vector2 Location { get; set; }
    public float StraightLineDistanceToEnd { get; set; }
    public MapNode ParentMapNode { get; set; }


    public float StraightLineDistanceTo(MapNode end)
    {
        return Mathf.Sqrt(Mathf.Pow(Location.x - end.Location.x, 2) + Mathf.Pow(Location.y - end.Location.y, 2));
    }
    
}


public class Edge
{
    public float Cost { get; set; } = 1;
    public MapNode ConnectedNode { get; set; }

    public List<MapNode> Auxiliars { get; set; } = new List<MapNode>();

    public override string ToString()
    {
        return "-> " + ConnectedNode.ToString();
    }
    
}



