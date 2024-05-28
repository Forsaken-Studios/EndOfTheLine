using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] float fov;
    [SerializeField] int rayCount;
    [SerializeField] float angle;
    [SerializeField] float viewDistance;
    [SerializeField] private Vector3 origin = Vector3.zero;
    [SerializeField] private GameObject _enemyGameObject;
    [SerializeField] private float startingAngle; 
    private Mesh mesh;
    [Header("Detection bar properties")] 
    [SerializeField] private BarDetectionProgress detectionBar;
    [Header("Obstacle Layer Mask")]
    [SerializeField] private LayerMask obstacleLayerMask;

    [SerializeField] private List<RaycastHit2D> raycastHitsList;
    void Start()
    {
        mesh = new Mesh();
        raycastHitsList = new List<RaycastHit2D>();
        GetComponent<MeshFilter>().mesh = mesh;
       //SetOrigin(_enemyGameObject.transform.localPosition);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateFieldOfViewMesh();

       if (!CheckIfPlayerIsBeignDetected())
        {
            if(detectionBar.gameObject.activeSelf)
                detectionBar.SetIfPlayerIsBeingDetected(false);
        }
    }

    private bool CheckIfPlayerIsBeignDetected()
    {
        foreach (var raycast in raycastHitsList)
        {
            if (raycast.collider != null)
            {
                if (raycast.collider.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void UpdateFieldOfViewMesh()
    {
        raycastHitsList.Clear();
        angle = startingAngle; 
        float angleIncrease = fov / rayCount;
        
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
        
        vertices[0] = origin;
        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = Vector3.back;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle),
                viewDistance, obstacleLayerMask);
            raycastHitsList.Add(raycastHit2D);
            if (raycastHit2D.collider == null)
            {
                //Not hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                //hit
                if (raycastHit2D.collider.gameObject.CompareTag("Player"))
                {
                    vertex = origin + GetVectorFromAngle(angle) * viewDistance;
                    if (!_enemyGameObject.GetComponent<Enemy>().PlayerIsDetected)
                    {
                        float distance = Vector2.Distance(this.gameObject.transform.position, 
                            raycastHit2D.collider.gameObject.transform.position);
                        //Pilla la distancia desde el centro, no de donde debe. 
                        //Debug.Log(distance);
                        //detectionBar.GetComponent<BarDetectionProgress>().SetSpeedBasedInDistance(distance);
                        detectionBar.gameObject.SetActive(true);
                        Debug.Log("PLAYER SAW?"); 
                    }
                }
                else
                {
                    vertex = raycastHit2D.point;
                }
            }
            vertices[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0; 
                triangles[triangleIndex + 1] = vertexIndex - 1; 
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleIncrease;
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    


    private static Vector3 GetVectorFromAngle(float angle)
    {
        //Angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;

        return n;
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) - fov / 2f; 
    }

    public void SetAngle(float angle)
    {
        startingAngle = angle;
    }

    public float GetAngle()
    {
        return startingAngle;
    }
}
