using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class StationController : MonoBehaviour
{
    public List<GameObject> Directions;
    
    public List<GameObject> WallConfigurationNorth;
    public List<GameObject> WallConfigurationSouth;
    public List<GameObject> WallConfigurationEast ;
    public List<GameObject> WallConfigurationWest ;
    
    public List<GameObject> DecorationNorth;
    public List<GameObject> DecorationSouth;
    public List<GameObject> DecorationEast ;
    public List<GameObject> DecorationWest ;
    
    Random rnd = new Random();

    
    [Header("Tester")]
    public int N;
    public int S;
    public int E;
    public int W;

    public bool runWallSelector = false;
    
    public bool runResetWallSelector = false;


    
    public void WallSelector(int North, int South, int East, int West)
    {
        List<int> DirectionsCheck = new List<int>();
        DirectionsCheck.Add(North);
        DirectionsCheck.Add(South);
        DirectionsCheck.Add(East);
        DirectionsCheck.Add(West);

        bool checkInputCorrect = true;
        int railCounter = 0;
        int index = 0;
        int stationDirection = 0;
        foreach (int dir in DirectionsCheck)
        {
            if (checkInputCorrect)
            {
                if (dir > 3 || dir < 0)
                {
                    checkInputCorrect = false;
                    Debug.Log("[StationController.cs] Invalid Parameter. Range:[0,3]");
                }
                else if (dir == 3)
                {
                    railCounter++;
                    stationDirection = index;
                }
                if (railCounter > 1)
                {
                    checkInputCorrect = false;
                    Debug.Log("[StationController.cs] Rail Number different than 1.");
                }
                index++;
            }
        }

        if (checkInputCorrect)
        {
            Directions[stationDirection].SetActive(true); //Activate Direction

            switch (stationDirection)
            {
                case 0: //North
                    if ((East == 2)&&(West == 2)) //Both adjacent Stations?
                    {
                        if (South == 0) //South Wall?
                        {
                            WallConfigurationNorth[9].SetActive(true); //Conf10
                        }
                        else
                        {
                            WallConfigurationNorth[8].SetActive(true); //Conf9
                        }
                    }else if (East == 2) //East adjacent Station?
                    {
                        if (West == 1) //West adjacent Hallway?
                        {
                            if (South == 1) //South adjacent Hallway?
                            {
                                WallConfigurationNorth[4].SetActive(true); //Conf5
                            }
                            else
                            {
                                WallConfigurationNorth[5].SetActive(true); //Conf6
                            }
                            
                        }
                        else
                        {
                            if (South == 1) //South adjacent Hallway?
                            {
                                WallConfigurationNorth[6].SetActive(true); //Conf7
                            }
                            else
                            {
                                WallConfigurationNorth[7].SetActive(true); //Conf8
                            }
                        }
                    }else if (West == 2) //West adjacent Station?
                    {
                        if (East == 1) //East adjacent Hallway?
                        {
                            if (South == 1) //South adjacent Hallway?
                            {
                                WallConfigurationNorth[0].SetActive(true); //Conf1
                            }
                            else
                            {
                                WallConfigurationNorth[1].SetActive(true); //Conf2
                            }
                            
                        }
                        else
                        {
                            if (South == 1) //South adjacent Hallway?
                            {
                                WallConfigurationNorth[2].SetActive(true); //Conf3
                            }
                            else
                            {
                                WallConfigurationNorth[3].SetActive(true); //Conf4
                            }
                        }
                    }
                    else
                    {
                        WallConfigurationNorth[10].SetActive(true); //ConfAux
                        Debug.Log("[StationController.cs] Aux case: No Adjacent Station.");
                    }
                    
                    //Create Obstacles
                    //Create Decals
                    //Create Enemies

                    SelectRandomDecorationNorth();
                    
                    break;
                case 1: //South
                    
                    if ((East == 2)&&(West == 2)) //Both adjacent Stations?
                    {
                        if (North == 0) //North Wall?
                        {
                            WallConfigurationSouth[9].SetActive(true); //Conf10
                        }
                        else
                        {
                            WallConfigurationSouth[8].SetActive(true); //Conf9
                        }
                    }else if (West == 2) //West adjacent Station?
                    {
                        if (East == 1) //East adjacent Hallway?
                        {
                            if (North == 1) //North adjacent Hallway?
                            {
                                WallConfigurationSouth[4].SetActive(true); //Conf5
                            }
                            else
                            {
                                WallConfigurationSouth[5].SetActive(true); //Conf6
                            }
                            
                        }
                        else
                        {
                            if (North == 1) //North adjacent Hallway?
                            {
                                WallConfigurationSouth[6].SetActive(true); //Conf7
                            }
                            else
                            {
                                WallConfigurationSouth[7].SetActive(true); //Conf8
                            }
                        }
                    }else if (East == 2) //East adjacent Station?
                    {
                        if (West == 1) //West adjacent Hallway?
                        {
                            if (North == 1) //North adjacent Hallway?
                            {
                                WallConfigurationSouth[0].SetActive(true); //Conf1
                            }
                            else
                            {
                                WallConfigurationSouth[1].SetActive(true); //Conf2
                            }
                            
                        }
                        else
                        {
                            if (North == 1) //North adjacent Hallway?
                            {
                                WallConfigurationSouth[2].SetActive(true); //Conf3
                            }
                            else
                            {
                                WallConfigurationSouth[3].SetActive(true); //Conf4
                            }
                        }
                    }
                    else
                    {
                        WallConfigurationSouth[10].SetActive(true); //ConfAux
                        Debug.Log("[StationController.cs] Aux case: No Adjacent Station.");
                    }

                    SelectRandomDecorationSouth();
                    
                    
                    break;
                case 2: //East
                    
                    if ((North == 2)&&(South == 2)) //Both adjacent Stations?
                    {
                        if (West == 0) //West Wall?
                        {
                            WallConfigurationEast[9].SetActive(true); //Conf10
                        }
                        else
                        {
                            WallConfigurationEast[8].SetActive(true); //Conf9
                        }
                    }else if (South == 2) //South adjacent Station?
                    {
                        if (North == 1) //North adjacent Hallway?
                        {
                            if (West == 1) //West adjacent Hallway?
                            {
                                WallConfigurationEast[4].SetActive(true); //Conf5
                            }
                            else
                            {
                                WallConfigurationEast[5].SetActive(true); //Conf6
                            }
                            
                        }
                        else
                        {
                            if (West == 1) //West adjacent Hallway?
                            {
                                WallConfigurationEast[6].SetActive(true); //Conf7
                            }
                            else
                            {
                                WallConfigurationEast[7].SetActive(true); //Conf8
                            }
                        }
                    }else if (North == 2) //North adjacent Station?
                    {
                        if (South == 1) //South adjacent Hallway?
                        {
                            if (West == 1) //West adjacent Hallway?
                            {
                                WallConfigurationEast[0].SetActive(true); //Conf1
                            }
                            else
                            {
                                WallConfigurationEast[1].SetActive(true); //Conf2
                            }
                            
                        }
                        else
                        {
                            if (West == 1) //West adjacent Hallway?
                            {
                                WallConfigurationEast[2].SetActive(true); //Conf3
                            }
                            else
                            {
                                WallConfigurationEast[3].SetActive(true); //Conf4
                            }
                        }
                    }
                    else
                    {
                        WallConfigurationEast[10].SetActive(true); //ConfAux
                        Debug.Log("[StationController.cs] Aux case: No Adjacent Station.");
                    }
                    
                    SelectRandomDecorationEast();
                    
                    break;
                case 3: //West
                    
                    if ((North == 2)&&(South == 2)) //Both adjacent Stations?
                    {
                        if (East == 0) //East Wall?
                        {
                            WallConfigurationWest[9].SetActive(true); //Conf10
                        }
                        else
                        {
                            WallConfigurationWest[8].SetActive(true); //Conf0
                        }
                    }else if (North == 2) //North adjacent Station?
                    {
                        if (South == 1) //South adjacent Hallway?
                        {
                            if (East == 1) //East adjacent Hallway?
                            {
                                WallConfigurationWest[4].SetActive(true); //Conf5
                            }
                            else
                            {
                                WallConfigurationWest[5].SetActive(true); //Conf6
                            }
                            
                        }
                        else
                        {
                            if (East == 1) //East adjacent Hallway?
                            {
                                WallConfigurationWest[6].SetActive(true); //Conf7
                            }
                            else
                            {
                                WallConfigurationWest[7].SetActive(true); //Conf8
                            }
                        }
                    }else if (South == 2) //South adjacent Station?
                    {
                        if (North == 1) //North adjacent Hallway?
                        {
                            if (East == 1) //East adjacent Hallway?
                            {
                                WallConfigurationWest[0].SetActive(true); //Conf1
                            }
                            else
                            {
                                WallConfigurationWest[1].SetActive(true); //Conf2
                            }
                            
                        }
                        else
                        {
                            if (East == 1) //East adjacent Hallway?
                            {
                                WallConfigurationWest[2].SetActive(true); //Conf3
                            }
                            else
                            {
                                WallConfigurationWest[3].SetActive(true); //Conf4
                            }
                        }
                    }
                    else
                    {
                        WallConfigurationWest[10].SetActive(true); //ConfAux
                        Debug.Log("[StationController.cs] Aux case: No Adjacent Station.");
                    }
                    
                    SelectRandomDecorationWest();
                    
                    break;

                
                
            }
        }
        
        
        
        
        
    }
    
    private void SelectRandomDecorationNorth(){int decoSeed = rnd.Next(DecorationNorth.Count);DecorationNorth[decoSeed].SetActive(true);}
    private void SelectRandomDecorationSouth(){int decoSeed = rnd.Next(DecorationSouth.Count);DecorationSouth[decoSeed].SetActive(true);}
    private void SelectRandomDecorationEast (){int decoSeed = rnd.Next(DecorationEast .Count);DecorationEast [decoSeed].SetActive(true);}
    private void SelectRandomDecorationWest (){int decoSeed = rnd.Next(DecorationWest .Count);DecorationWest [decoSeed].SetActive(true);}

    public void ResetWallSelection()
    {
        foreach (var x in DecorationNorth) { x.SetActive(false); }
        foreach (var x in DecorationSouth) { x.SetActive(false); }
        foreach (var x in DecorationEast ) { x.SetActive(false); }
        foreach (var x in DecorationWest ) { x.SetActive(false); }
        
        foreach (var x in WallConfigurationNorth) { x.SetActive(false); }
        foreach (var x in WallConfigurationSouth) { x.SetActive(false); }
        foreach (var x in WallConfigurationEast ) { x.SetActive(false); }
        foreach (var x in WallConfigurationWest ) { x.SetActive(false); }
        
        foreach (var x in Directions) { x.SetActive(false); }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        
        
    }


    // Update is called once per frame
    void Update()
    {
        if (runWallSelector)
        {
            WallSelector(N,S,E,W);
            runWallSelector = false;
        }

        if (runResetWallSelector)
        {
            ResetWallSelection();
            runResetWallSelector = false;
        }
    }
}
