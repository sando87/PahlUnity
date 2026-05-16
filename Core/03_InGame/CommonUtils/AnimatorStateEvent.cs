using UnityEngine;

namespace PahlBit
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
                mHelper.InvokeEventEnter(stateInfo.shortNameHash);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (mHelper != null)
                mHelper.InvokeEventLeave(stateInfo.shortNameHash);
        }

    }
}