using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class PlatformCatcher : MonoBehaviour
    {
        [Serializable]
        public class CaughtObject
        {
            public Rigidbody2D rigidbody;
            public Collider2D collider;
            public CharacterController2D character;
            public bool inContact;
            public bool checkedThisFrame;

            public void Move(Vector2 movement)
            {
                if(!inContact)
                    return; 
                if(character != null)
                    character.Move(movement);
                else
                    rigidbody.MovePosition(rigidbody.position + movement);
            }
        }

        public Rigidbody2D platformRigidbody;
        public ContactFilter2D contactFilter;

        protected List<CaughtObject> m_CaughtObjects = new List<CaughtObject>(128);
        protected ContactPoint2D[] m_ContactPoints = new ContactPoint2D[20];
        protected Collider2D m_Collider;
        protected PlatformCatcher m_ParentCatcher;

        protected Action<Vector2> m_MoveDelegate = null;

        public int CaughtObjectCount
        {
            get
            {
                int count = 0;
                for(int i = 0; i < m_CaughtObjects.Count; i++)
                {
                    if(m_CaughtObjects[i].inContact)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public float CaughtOBjectsMass
        {
            get
            {
                float mass = 0f;
                for(int i = 0; i < m_CaughtObjects.Count; i++)
                {
                    if(m_CaughtObjects[i].inContact)
                    {
                        mass += m_CaughtObjects[i].rigidbody.mass;
                    }
                }

                return mass;
            }
        }

        void Awake()
        {
            if(platformRigidbody == null)
            {
                platformRigidbody = GetComponent<Rigidbody2D>();
            }

            if(m_Collider == null)
            {
                m_Collider = GetComponent<Collider2D>();
            }

            m_ParentCatcher = null;
            Transform currentParent = transform.parent;
            while(currentParent != null)
            {
                PlatformCatcher catcher = currentParent.GetComponent<PlatformCatcher>();
                if(catcher != null)
                {
                    m_ParentCatcher = catcher;
                }
                currentParent = catcher.transform.parent;
            }

            if(m_ParentCatcher != null)
                m_ParentCatcher.m_MoveDelegate += MoveCaughtObjects;
        }

        void FixedUpdate()
        {
            for (int i = 0; i < m_CaughtObjects.Count; i++)
            {
                m_CaughtObjects[i].inContact = false;
                m_CaughtObjects[i].checkedThisFrame = false;
            }

            CheckRigidbodyContacts(platformRigidbody);

            bool checkAgain = false;
            do
            {
                for (int i = 0; i < m_CaughtObjects.Count; i++)
                {
                    CaughtObject caughtObject = m_CaughtObjects[i];

                    if(caughtObject.inContact)
                    {
                        if(!caughtObject.checkedThisFrame)
                        {
                            CheckRigidbodyContacts(caughtObject.rigidbody);
                            caughtObject.checkedThisFrame = true;
                        }
                    }

                    //Some cases will remove all contacts (collider resize etc.) leading to loosing contact with the platform
                    //so we check the distance of the object to the top of the platform.
                    if (!caughtObject.inContact)
                    {
                        Collider2D caughtObjectCollider = caughtObject.collider;

                        bool verticalAlignement = (caughtObjectCollider.bounds.max.x > m_Collider.bounds.min.x) && (caughtObjectCollider.bounds.min.x < m_Collider.bounds.max.x);
                        if (verticalAlignement)
                        {
                            float yDiff = caughtObject.collider.bounds.min.y - m_Collider.bounds.max.y;

                            if (yDiff > 0 && yDiff < 0.05f)
                            {
                                caughtObject.inContact = true;
                                caughtObject.checkedThisFrame = true;
                            }
                        }
                    }
                }

                checkAgain = false;

                for (int i = 0; i < m_CaughtObjects.Count; i++)
                {
                    CaughtObject caughtObject = m_CaughtObjects[i];
                    if(caughtObject.inContact && !caughtObject.checkedThisFrame)
                    {
                        checkAgain = true;
                        break;
                    }
                }
            }
            while(checkAgain);
        }

        void CheckRigidbodyContacts(Rigidbody2D rigidbody2D)
        {
            int contactCount = rigidbody2D.GetContacts(contactFilter, m_ContactPoints);

            for (int i = 0; i < contactCount; i++)
            {
                ContactPoint2D contactPoint2D = m_ContactPoints[i];
                Rigidbody2D contactRigidbody = contactPoint2D.rigidbody == rigidbody2D ? contactPoint2D.otherRigidbody : contactPoint2D.rigidbody;
                int listIndex = -1;

                for (int k = 0; k < m_CaughtObjects.Count; k++){
                    if(contactRigidbody == m_CaughtObjects[k].rigidbody){
                        listIndex = k;
                        break;
                    }
                }

                if (listIndex == -1)
                {
                    if (contactRigidbody != null)
                    {
                        if (contactRigidbody.bodyType != RigidbodyType2D.Static && contactRigidbody != platformRigidbody)
                        {
                            float dot = Vector2.Dot(contactPoint2D.normal, Vector2.down);

                            if (dot > 0.8f)
                            {
                                CaughtObject newCaughtObject = new CaughtObject
                                {
                                    rigidbody = contactRigidbody,
                                    character = contactRigidbody.GetComponent<CharacterController2D>(),
                                    collider = contactRigidbody.GetComponent<Collider2D>(),
                                    inContact = true,
                                    checkedThisFrame = false,
                                };

                                m_CaughtObjects.Add(newCaughtObject);
                            }
                        }
                    }
                }
                else
                {
                    m_CaughtObjects[listIndex].inContact = true;
                }

            }
        }

        public void MoveCaughtObjects(Vector2 velocity)
        {
            m_MoveDelegate?.Invoke(velocity);
            

            for(int i = 0; i < m_CaughtObjects.Count; i++)
            {
                CaughtObject caughtObject = m_CaughtObjects[i];
                if(m_ParentCatcher != null && m_ParentCatcher.m_CaughtObjects.Find((CaughtObject A) => { return A.rigidbody == caughtObject.rigidbody;}) != null)
                {
                    continue;
                }

                caughtObject.Move(velocity);
            }
        }

        public bool HasCaughtObject(GameObject gameObject)
        {
            for(int i = 0; i < m_CaughtObjects.Count; i++)
            {
                if(m_CaughtObjects[i].collider.gameObject == gameObject && m_CaughtObjects[i].inContact)
                {
                    return true;
                }
            }

            return false;
        }


    }
}