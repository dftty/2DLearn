﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            GetComponent<Rigidbody2D>().MovePosition(transform.position + new Vector3(1, 0));
        }
    }
}
