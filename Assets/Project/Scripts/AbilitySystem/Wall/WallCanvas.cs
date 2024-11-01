using Player;
using UnityEngine;
using UnityEngine.UI;

public class WallCanvas : MonoBehaviour
{
    private AbilityHolder holder;

    [SerializeField] private Image positionImage;
    [SerializeField] private Image oppositeWallImage;
    [SerializeField] private LayerMask wallLayerMask;

    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite redSprite;
    
    private Vector2 oppositeWall;
    private Vector2 closestWall;
    private GameObject parent;
    private float minDistance = 40;
    private float minDistanceBetweenWires = 70;
    private void Update()
    {
        closestWall =  GameManager.Instance.GetWallCollider().ClosestPoint(GetPosition());
        if (GameManager.Instance.GetFloorCollider().OverlapPoint(GetPosition()))
        {
            Vector2 rayDirection = (closestWall - GetPosition()).normalized;
            float escalar = Vector2.Dot(rayDirection, ((Vector2)parent.transform.position - closestWall));
            positionImage.rectTransform.position = closestWall;
            Vector2 directionInFront = -(GetPosition() - closestWall).normalized;
            RaycastHit2D hitInFront = Physics2D.Raycast(closestWall, -directionInFront, 2000f,wallLayerMask);
            oppositeWall = hitInFront.point;
            if (hitInFront.collider != null && hitInFront.collider.gameObject.tag == "Wall")
            {
                if (escalar < 0)
                {
                    if (Vector2.Distance(hitInFront.point, closestWall) < AbilityManager.Instance.GetMaxWallDistance())
                    {
                        Vector2 dir = (GetPosition() - (Vector2) PlayerController.Instance.gameObject.transform.position).normalized;
                        float distance = Vector2.Distance(GetPosition(),(Vector2)PlayerController.Instance.gameObject.transform.position);
                        //escalar 2.0
                        RaycastHit2D raycastToMousePosition = Physics2D.Raycast(PlayerController.Instance.gameObject.transform.position, dir, distance,wallLayerMask);
                        if (raycastToMousePosition.collider != null)
                        {
                            //Not visible Position
                            PositionNotValidButShowPositon(hitInFront);
                        }
                        else
                        {
                            //Valid Position
                            holder.SetIfCanThrowAbility(true); 
                            oppositeWallImage.rectTransform.position = hitInFront.point;
                            holder.UpdatePositionToThrowAbility(closestWall, hitInFront.point);
                            positionImage.sprite = greenSprite;
                            oppositeWallImage.sprite = greenSprite;
                        
                        }
                    }
                    else
                    {
                        //InValid Position
                        PositionNotValidButShowPositon(hitInFront);
                    }
                } else
                {
                    //Not visible Position
                    PositionNotValidButShowPositon(hitInFront);
                }
                
            }
            else
            {
                //InValid Position
                holder.SetIfCanThrowAbility(false);
                NotValidPosition();
            }
        }
    }

    private void NotValidPosition()
    {
        positionImage.sprite = redSprite;
        oppositeWallImage.sprite = redSprite; 
        oppositeWallImage.gameObject.SetActive(true);
        positionImage.gameObject.SetActive(true);
    }

    private void PositionNotValidButShowPositon(RaycastHit2D hitInFront)
    {
        holder.SetIfCanThrowAbility(false);
        oppositeWallImage.rectTransform.position = hitInFront.point;
        NotValidPosition();
    }
    
   void OnDrawGizmos()
    {
    
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(closestWall, 0.5f);
        Gizmos.DrawSphere(oppositeWall, 0.5f);
        Gizmos.DrawLine(PlayerController.Instance.gameObject.transform.position, GetPosition());
    
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
