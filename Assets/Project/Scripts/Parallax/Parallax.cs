using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Material mat;
    private float distance;

    [Range(0f, 0.5f)] [SerializeField] private float speed;


    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        distance += Time.deltaTime * speed;

        mat.SetTextureOffset("_MainTex", Vector2.right * distance);
    }
}
