using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyGame
{
    
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        [Tooltip("The layers which represent gameobjects that the character Controller can be grounded on.")]
        public LayerMask groundedLayerMask;
        [Tooltip("The distance down to check for ground.")]
        public float groundedRaycastDistance = 0.1f;

        Rigidbody2D m_Rigidbody2D;
        CapsuleCollider2D m_Capsule;
        Vector2 m_PreviousPosition;
        Vector2 m_CurrentPosition;
        Vector2 m_NextMovement;
        ContactFilter2D m_ContactFilter;
        RaycastHit2D[] m_HitBuffer = new RaycastHit2D[3];
        RaycastHit2D[] m_FoundHits = new RaycastHit2D[3];
        Collider2D[] m_GroundColliders = new Collider2D[3];
        Vector2[] m_RayCastPositions = new Vector2[3];

        public bool IsGrounded {get; protected set;}
        public bool IsCeiling {get; protected set;}
        public Vector2 Velocity {get; protected set;}
        public Rigidbody2D Rigidbody2D {get { return m_Rigidbody2D; }}
        public Collider2D[] GroundColliders {get{ return m_GroundColliders; }}

        void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_Capsule = GetComponent<CapsuleCollider2D>();

            m_PreviousPosition = m_Rigidbody2D.position;
            m_CurrentPosition = m_Rigidbody2D.position;

            m_ContactFilter.layerMask = groundedLayerMask;
            m_ContactFilter.useLayerMask = true;
            m_ContactFilter.useTriggers = false;

            Physics2D.queriesStartInColliders = false;
        }

        void FixedUpdate()
        {
            m_PreviousPosition = m_Rigidbody2D.position;
            m_CurrentPosition = m_PreviousPosition + m_NextMovement;
            Velocity = (m_CurrentPosition - m_PreviousPosition) / Time.deltaTime;

            m_Rigidbody2D.MovePosition(m_CurrentPosition);
            m_NextMovement = Vector2.zero;
        }


        public void Move(Vector2 movement)
        {
            m_NextMovement = movement;
        }

        public void Teleport(Vector2 position)
        {
            Vector2 delta = position - m_CurrentPosition;
            m_PreviousPosition += delta;
            m_CurrentPosition = position;
            m_Rigidbody2D.MovePosition(m_CurrentPosition);
        }

        public void CheckCapsuleEndCollisions(bool bottom = true)
        {
            
        }

    }

}