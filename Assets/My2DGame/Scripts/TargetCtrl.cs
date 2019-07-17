using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCtrl : MonoBehaviour
{


    protected Animator animator;    
    
    //the platform object in the scene
    public Transform jumpTarget = null; 
    void Start () {
        animator = GetComponent<Animator>();
    }
    
    void Update () {
        if(animator) {
            if(Input.GetKey(KeyCode.Space))   
            {
                animator.SetTrigger("jump");
                animator.MatchTarget(jumpTarget.position, jumpTarget.rotation, AvatarTarget.LeftFoot, 
                                                      new MatchTargetWeightMask(Vector3.one, 1f), 0.141f, 0.78f);
            }      
                
        }       
    }
}
