using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{

    private float timeMoving = 5f;

    private Vector2 missionSelectorPosition = new Vector2(-1908, 0); 
    private Vector2 controlRoomPosition = new Vector2(0, 0); 
    private Vector2 extraRoomPosition = new Vector2(1908, 0);

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
                    this.gameObject.transform.localPosition = new Vector3(positionToArrive.x, 0, 0);
                    previousIndex = currentIndex;
                }
        }
    }

    public void MoveTrain(int index)
    {
        Debug.Log("INDEaX: " + index);
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
                //Extra Room
                positionToArrive = extraRoomPosition;
                break; 
        }
        previousIndex = currentIndex;
        currentIndex = index; 

    }
    
    
}
