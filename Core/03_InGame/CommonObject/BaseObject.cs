using Unity.VisualScripting;
using UnityEngine;


namespace PahlUnity
{
    public class BaseObject : MonoBehaviour
    {
        private InputPlayer mInput = null;
        public InputPlayer Input { get { CacheComp(ref mInput); return mInput; } }

        private ObjectPhysics2D mPhysics2D = null;
        public ObjectPhysics2D Physics2D { get { CacheComp(ref mPhysics2D); return mPhysics2D; } }

        private ObjectPhysics3D mPhysics3D = null;
        public ObjectPhysics3D Physics3D { get { CacheComp(ref mPhysics3D); return mPhysics3D; } }

        private ObjectBody2D mBody2D = null;
        public ObjectBody2D Body2D { get { CacheComp(ref mBody2D); return mBody2D; } }

        private ObjectBody3D mBody3D = null;
        public ObjectBody3D Body3D { get { CacheComp(ref mBody3D); return mBody3D; } }

        private Health mHealth = null;
        public Health Health { get { CacheComp(ref mHealth); return mHealth; } }

        private SpecBase mSpec = null;
        public SpecBase Spec { get { CacheComp(ref mSpec); return mSpec; } }

        private RenderController mRender = null;
        public RenderController Render { get { CacheComp(ref mRender); return mRender; } }

        private FiniteStateMachine mFSM = null;
        public FiniteStateMachine FSM { get { CacheComp(ref mFSM); return mFSM; } }

        private InteractableCollider mInteractor = null;
        public InteractableCollider Interactor { get { CacheComp(ref mInteractor); return mInteractor; } }

        private AnimatorHelper mAnim = null;
        public AnimatorHelper Anim { get { CacheComp(ref mAnim); return mAnim; } }

        public T GetComp<T>() where T : MonoBehaviour
        {
            return GetComponentInChildren<T>();
        }
        public bool HasComp<T>() where T : MonoBehaviour
        {
            return GetComponentInChildren<T>() != null;
        }
        public bool TryGetComp<T>(out T component) where T : MonoBehaviour
        {
            component = GetComponentInChildren<T>();
            return component != null;
        }
        private void CacheComp<T>(ref T comp) where T : MonoBehaviour
        {
            if (comp == null || !comp.gameObject.activeInHierarchy)
                comp = GetComp<T>();
        }

        public void DestroyObj()
        {
            Destroy(gameObject);
        }
        public void DestroyObj(float delay)
        {
            Destroy(gameObject, delay);
        }
    }
}
