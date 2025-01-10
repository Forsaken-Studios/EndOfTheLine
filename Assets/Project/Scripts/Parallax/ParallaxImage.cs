using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxImage : MonoBehaviour
{
    private Material mat;
    private float distance;

    [Range(0f, 0.5f)] [SerializeField] private float speed;


    private void Start()
    {
        mat = GetComponent<Image>().material;
    }

    private void Update()
    {
        distance += Time.deltaTime * speed;

        mat.SetTextureOffset("_MainTex", Vector2.left * distance);
    }
}