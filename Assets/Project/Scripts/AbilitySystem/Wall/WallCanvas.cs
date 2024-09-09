using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WallCanvas : MonoBehaviour
{
    private AbilityHolder holder;

    [SerializeField] private Image positionImage;
    [SerializeField] private Image oppositeWallImage;
    [SerializeField] private LayerMask wallLayerMask;

    private Vector2 oppositeWall;
    private Vector2 closestWall;
    private GameObject parent;
    private float minDistance = 40;
    private void Update()
    {
        closestWall =  GameManager.Instance.GetWallCollider().ClosestPoint(GetPosition());
        if (GameManager.Instance.GetFloorCollider().OverlapPoint(GetPosition()))
        {
            if (Vector2.Distance(closestWall, GetPosition()) < minDistance)
            {
                Vector2 rayDirection = (closestWall - GetPosition()).normalized;
                float escalar = Vector2.Dot(rayDirection, ((Vector2)parent.transform.position - closestWall));
                if (escalar < 0)
                {
                    positionImage.rectTransform.position = closestWall;
                    Vector2 directionInFront = -(GetPosition() - closestWall).normalized;
                    RaycastHit2D hitInFront = Physics2D.Raycast(closestWall, -directionInFront, 2000f,wallLayerMask);
                    oppositeWall = hitInFront.point;
                    if (hitInFront.collider != null && hitInFront.collider.gameObject.tag == "Wall")
                    {
                        oppositeWallImage.rectTransform.position = hitInFront.point;
                        holder.UpdatePositionToThrowAbility(closestWall, hitInFront.point);
                        holder.SetIfCanThrowAbility(true);
                    }
                    oppositeWallImage.gameObject.SetActive(true);
                }
                else
                {
                    holder.SetIfCanThrowAbility(false);
                }
            }
            else
            {
                holder.SetIfCanThrowAbility(false);
                oppositeWallImage.gameObject.SetActive(false);
                positionImage.rectTransform.position = GetPosition();
                holder.UpdatePositionToThrowAbility(GetPosition(), Vector2.zero); 
            }  
        }
    }
    
    void OnDrawGizmos()
    {
     
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(closestWall, 2f);
            Gizmos.DrawSphere(oppositeWall, 2f);
            Gizmos.DrawLine(closestWall, oppositeWall);
        
    }
    
    public void SetHolder(AbilityHolder holder, GameObject parent)
    {
        this.holder = holder;
        this.parent = parent;
    }
    
    
    private Vector2 GetPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}
