using UnityEngine;

namespace PahlUnity
{
    public class AnimatorStateEvent : StateMachineBehaviour
    {
        AnimatorHelper mHelper = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (mHelper == null)
                mHelper = animator.GetComponent<AnimatorHelper>();

            if (mHelper != null)
                mHelper.InvokeEventEnter(stateInfo.shortNameHash, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (mHelper != null)
                mHelper.InvokeEventLeave(stateInfo.shortNameHash, layerIndex);
        }

    }
}