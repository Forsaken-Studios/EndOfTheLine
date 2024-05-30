using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace FieldOfView
{

    public class FieldOfView : MonoBehaviour
    {
        [Header("Properties")] [SerializeField]
        float fov;

        [SerializeField] int rayCount;
        [SerializeField] float angle;
        [SerializeField] float viewDistance;
        [SerializeField] private float currentAngle;
        [SerializeField] private Vector3 origin = Vector3.zero;

        [Header("Enemy Script")] [SerializeField]
        private Enemy enemyScript;

        [Header("Detection bar properties")] [SerializeField]
        private BarDetectionProgress detectionBar;

        [FormerlySerializedAs("obstacleLayerMask")] [Header("Obstacle Layer Mask")] [SerializeField]
        private LayerMask obstaclePlayerLayerMask;

        private List<RaycastHit2D> raycastHitsList;
        private Mesh mesh;

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
            // FindTargetPlayer();

            if (!CheckIfPlayerIsBeingDetected())
            {
                if (detectionBar.GetIfPlayerIsDetected())
                {
                    //Start countdown to forget player
                    enemyScript.ActivateCountdownToForgetPlayer();
                }
                else
                {
                    if (detectionBar.gameObject.activeSelf)
                    {
                        Debug.Log("CHECK");
                        detectionBar.SetIfPlayerIsBeingDetected(false);
                    }

                }
            }
            else
            {
                if (enemyScript.GetIfEnemyIsForgetting())
                {
                    enemyScript.StopEnemyActionOfForgettingPlayer();
                }

                if (!detectionBar.GetIfPlayerIsDetected())
                {
                    detectionBar.gameObject.SetActive(true);
                }
            }
        }

        private bool CheckIfPlayerIsBeingDetected()
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
            angle = currentAngle;
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
                    viewDistance, obstaclePlayerLayerMask);
                raycastHitsList.Add(raycastHit2D);
                if (raycastHit2D.collider == null)
                {
                    //Not hit
                    vertex = origin + GetVectorFromAngle(angle) * viewDistance;
                }
                else
                {

                    /*if (raycastHit2D.collider.gameObject.CompareTag("Player"))
                     {
                         if(raycastHit2D.collider.gameObject.layer == obstacleLayerMask)
                             vertex = origin + GetVectorFromAngle(angle) * viewDistance;

                         if (!enemyGameObject.GetComponent<Enemy>().PlayerIsDetected)
                         {
                             detectionBar.gameObject.SetActive(true);
                             Debug.Log("PLAYER SAW?");
                         }
                     }*/
                    //hit obstacle
                    vertex = raycastHit2D.point;

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

        //Deprecated
        /*private void FindTargetPlayer()
        {
            if (Vector3.Distance(enemyGameObject.transform.position, playerGameObject.transform.position) < viewDistance)
            {
                //Player inside vision
                Vector2 dirToPlayer = (playerGameObject.transform.position - enemyGameObject.transform.position).normalized;
                Debug.Log("DIR TO PLAYER: " + dirToPlayer);

                Vector3 angleVector3 = GetVectorFromAngle(currentAngle);
                Vector2 angle = new Vector2(angleVector3.x, angleVector3.y);
                Debug.Log("CCTV: " + angle);
                Debug.Log("ANGLE: "  + Vector2.Angle(angle, dirToPlayer));
                if (Vector2.Angle(angle, dirToPlayer) < fov)
                {
                    //DETECTION
                   RaycastHit2D raycastHit2D = Physics2D.Raycast(enemyGameObject.transform.position,
                       dirToPlayer);
                    Debug.DrawRay(enemyGameObject.transform.position, dirToPlayer);
                   if (raycastHit2D.collider != null)
                   {
                       if (raycastHit2D.collider.gameObject.CompareTag("Player"))
                       {

                       }
                   }
                }
            }
        }*/

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
            currentAngle = GetAngleFromVectorFloat(aimDirection) - fov / 2f;
        }

        public void SetAngle(float angle)
        {
            currentAngle = angle;
        }

        public float GetAngle()
        {
            return currentAngle;
        }
    }
}