using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class RigidTest : MonoBehaviour
{

    public Rigidbody2D rigidBody2D;
    public CharacterController2D characterController2D;
    private Vector2 m_PreviousPosition;
    private Vector2 m_CurrentPosition;
    private Vector2 m_Movement;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Movement = new Vector2(0, -0.04f);
    }
    

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        rigidBody2D.MovePosition(rigidBody2D.position + m_Movement);
    }
}
