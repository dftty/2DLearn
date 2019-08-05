using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lessice
{
    public class SceneLinkedSMB<T> : SealedSMB where T : MonoBehaviour
    {
        protected T m_MonoBehavior;
        bool m_FirstFrameHappened;
        bool m_LastFrameHappened;

        public static void Initialize(Animator animator, T monoBehaviour)
        {
            SceneLinkedSMB<T>[] behaviours = animator.GetBehaviours<SceneLinkedSMB<T>>();
            

            for(int i = 0; i < behaviours.Length; i++){
                behaviours[i].Init(animator, monoBehaviour);
            }
        }

        public void Init(Animator animator, T monoBehaviour)
        {
            m_MonoBehavior = monoBehaviour;
        }

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
            m_FirstFrameHappened = false;
            OnSLStateEnter(animator, stateInfo, layerIndex);
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
            if(!animator.isActiveAndEnabled)
            {
                return ;
            }

            if(animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionToStateUpdate(animator, stateInfo, layerIndex);
            }

            if(!animator.IsInTransition(layerIndex) && !m_FirstFrameHappened)
            {
                m_FirstFrameHappened = true;
                OnSLStatePostEnter(animator, stateInfo, layerIndex);
            }

            if(!animator.IsInTransition(layerIndex) && m_FirstFrameHappened){
                OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            }

            if(animator.IsInTransition(layerIndex) && !m_LastFrameHappened && m_FirstFrameHappened)
            {
                m_LastFrameHappened = true;
                OnSLStatePreExit(animator, stateInfo, layerIndex);
            }


            if(animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionFromStateUpdate(animator, stateInfo, layerIndex);
            }
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
            m_LastFrameHappened = false;
            OnSLStateExit(animator, stateInfo, layerIndex);
        }

        /// <summary>
        /// 在Start方法中调用
        /// </summary>
        public virtual void OnStart(){}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
        
        public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}

        public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}

        public virtual void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}

        public virtual void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}

        public virtual void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}

        public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}


    }

    /// <summary>
    /// sealed 关键字可以用在override一个虚方法或者需虚性前,可以防止其他类继承该类时重写该方法。
    /// </summary>
    public class SealedSMB : StateMachineBehaviour
    {
        
        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}
        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}
        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}
    }


}


