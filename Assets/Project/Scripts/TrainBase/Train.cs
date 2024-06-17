using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    
    private bool isMoving = false;
    private bool isMovingToRight = false;
    private float distanceToMove = 1910f;
    private float timeMoving = 2f;
    private bool _canMove;

    public bool CanMove
    {
        get { return !isMoving; }
        set { _canMove = value; }
    }
    
    private Vector2 positionToArrive ;
    private void Update()
    {
        if (isMoving && !_canMove)
        {
            if (isMovingToRight)
            {
                //Restamos 
                Vector2 currentPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
                this.gameObject.transform.localPosition = Vector2.Lerp(currentPosition, positionToArrive, 
                    timeMoving * Time.deltaTime);
                if (Vector2.Distance(positionToArrive, this.gameObject.transform.localPosition) <= 5)
                {
                    this.gameObject.transform.localPosition = new Vector3(positionToArrive.x, 0, 0);
                    isMoving = false;
                }
            }
            else
            {
                //Sumamos
                Vector2 currentPosition = new Vector2(transform.localPosition.x, 0);
                this.gameObject.transform.localPosition = Vector2.Lerp(currentPosition, positionToArrive, 
                    timeMoving * Time.deltaTime);
                //Hay que ver, porque supuestamente en el lerp no es recomendable meter el time.delta time, porque habría 
                // que poner este if, pero por ahora se deja
                if (Vector2.Distance(positionToArrive, this.gameObject.transform.localPosition) <= 5)
                {
                    this.gameObject.transform.localPosition = new Vector3(positionToArrive.x, 0, 0);
                    isMoving = false;
                }
            }
        }
    }


    public void MoveTrainToLeft()
    {
        positionToArrive = new Vector2(transform.localPosition.x + distanceToMove, 0);
        isMovingToRight = false;
        isMoving = true; 
        //Mission Selector = -1910
        //Control Room = 9
        //Extra = 1910
    }
    
    
    public void MoveTrainToRight()
    {
        positionToArrive = new Vector2(transform.localPosition.x - distanceToMove, 0);
        isMovingToRight = true;
        isMoving = true; 
    }
    
}
