using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarPosition : MonoBehaviour
{
    [SerializeField] Transform goalPos;

    void Start()
    {
    }

    void Update()
    {
        transform.position = goalPos.position;
    }
}