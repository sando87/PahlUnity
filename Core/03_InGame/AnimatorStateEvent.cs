using UnityEngine;

namespace PahlUnity
{
    public class AnimatorStateEvent : StateMachineBehaviour
    {
        AnimatorHelper mHelper = null;
        int mLastLoop = -1;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            mLastLoop = Mathf.FloorToInt(stateInfo.normalizedTime);

            if (mHelper == null)
                mHelper = animator.GetComponent<AnimatorHelper>();

            if (mHelper != null)
                mHelper.InvokeEventEnter(stateInfo.shortNameHash, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            int loop = Mathf.FloorToInt(stateInfo.normalizedTime);
            if (loop == mLastLoop)
                return;

            mLastLoop = loop;
            if (mHelper != null)
                mHelper.InvokeEventLoopStart(stateInfo.shortNameHash, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (mHelper != null)
                mHelper.InvokeEventLeave(stateInfo.shortNameHash, layerIndex);
        }

    }
}