using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{

    private float timeMoving = 5f;

    private Vector2 missionSelectorPosition = new Vector2(-1908, -88); 
    private Vector2 controlRoomPosition = new Vector2(0, -88); 
    private Vector2 loreRoomPosition = new Vector2(1908, -88);
    private Vector2 barmanPosition = new Vector2(3816, -88);
    private Vector2 marketRoomPosition = new Vector2(5568, -88);
    private Vector2 expeditionRoomPosition = new Vector2(7414, -88);

    private float trainFloorTransformValue = -88;

    private int currentIndex = 0;
    private int previousIndex = 0;
    private Vector2 positionToArrive ;
    private void Update()
    {
        if (previousIndex != currentIndex)
        {
                Vector2 currentPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
                this.gameObject.transform.localPosition = Vector2.Lerp(currentPosition, positionToArrive, 
                    timeMoving * Time.deltaTime);
                if (Vector2.Distance(positionToArrive, this.gameObject.transform.localPosition) <= 3)
                {
                    this.gameObject.transform.localPosition = new Vector3(positionToArrive.x, trainFloorTransformValue, 0);
                    previousIndex = currentIndex;
                }
        }
    }

    public void MoveTrain(int index)
    {
        switch (index)
        {
            case 0:
                //Mission Selector
                positionToArrive = missionSelectorPosition;
                break;    
            case 1:
                //Control Room 
                positionToArrive = controlRoomPosition;
                break;  
            case 2:
                //Lore Room
                positionToArrive = loreRoomPosition;
                break;        
            case 3:
                //Barman Room 
                positionToArrive = barmanPosition;
                break; 
            case 4:
                //Market Room 
                positionToArrive = marketRoomPosition;
                break; 
            case 5:
                //Expedition Room 
                positionToArrive = expeditionRoomPosition;
                break; 
        }
        previousIndex = currentIndex;
        currentIndex = index; 

    }
    
    
}
