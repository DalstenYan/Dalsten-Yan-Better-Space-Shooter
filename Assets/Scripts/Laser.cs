﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.up);
        if (transform.position.y >= 7) 
        {
            Destroy(transform.parent != null ? transform.parent.gameObject : gameObject);
        }
    }
}
