using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class VisionCone : MonoBehaviour
{


    [SerializeField] private Material VisionConeMaterial;
    [SerializeField] private float visionRange;
    [SerializeField] private float visionAngle;
    [Header("Layer")]
    [Tooltip("Layer with Objects that obstruct the enemy view, like walls")]
    [SerializeField] private LayerMask visionObstructingLayer;
    [Tooltip("The vision cone will be made up of triangles, the higher this value is, the prettier the vision cone will be")]
    [SerializeField] private int visionConeResolution = 120;

    private Mesh visionConeMesh;
    private MeshFilter meshFilter; 
    
    void Start()
    {
        //Create the mesh renderer, the mesh filter, the mesh object, and converts the vision angle from degrees to
        //radians (we need to use radians because the method that gives us the sine and cosine works with radians)
        
        transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        meshFilter = transform.AddComponent<MeshFilter>();
        visionConeMesh = new Mesh();
        visionAngle = Mathf.Deg2Rad;
    }


    void Update()
    {
        DrawVisionCone();
    }

    private void DrawVisionCone()
    {
        int[] triangles = new int[(visionConeResolution - 1) * 3];
        Vector3[] vertices = new Vector3[visionConeResolution + 1];
        vertices[0] = Vector3.zero;
        float currentAngle = -visionAngle / 2;
        float angleIncrement = visionAngle / (visionConeResolution - 1);
        float sine;
        float cosine;

        for (int i = 0; i < visionConeResolution; i++)
        {
            sine = Mathf.Sin(currentAngle);
            cosine = Mathf.Cos(currentAngle);

            Vector3 raycastDirection = (transform.forward * cosine) + (transform.right * sine);
            Vector3 vertForward = (Vector3.forward * cosine) + (Vector3.right * sine);
            if (Physics.Raycast(transform.position, raycastDirection, out RaycastHit hit, visionRange,
                    visionObstructingLayer))
            {
                vertices[i + 1] = vertForward * hit.distance;
            }
            else
            {
                vertices[i + 1] = vertForward * visionRange;
            }

            currentAngle += angleIncrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        visionConeMesh.Clear();
        visionConeMesh.vertices = vertices;
        visionConeMesh.triangles = triangles;
        meshFilter.mesh = visionConeMesh;
    }
}
