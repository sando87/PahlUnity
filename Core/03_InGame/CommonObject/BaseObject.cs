using UnityEngine;


namespace PahlBit
{
    public class BaseObject : MonoBehaviour
    {
        public RenderController Renderer => GetComponentInChildren<RenderController>();
        public FiniteStateMachine StateMachine { get => GetComponentInChildren<FiniteStateMachine>(); }
        public AnimatorHelper AnimHelper { get => GetComponentInChildren<AnimatorHelper>(); }
        public ObjectBody Body { get => GetComponentInChildren<ObjectBody>(); }
        public ObjectPhysics Phy { get => GetComponentInChildren<ObjectPhysics>(); }
        public PlayerUnitInput Input { get => GetComponentInChildren<PlayerUnitInput>(); }
        public Health Health { get => GetComponentInChildren<Health>(); }
        public PlayerController Ctrl { get => GetComponentInChildren<PlayerController>(); }
        public BuffController Buffs => GetComponentInChildren<BuffController>();
        public SpecBase Spec => GetComponentInChildren<SpecBase>();
        public InteractableCollider Interactor => GetComponentInChildren<InteractableCollider>();

        public PlayerMain PlayerObj => GetComponentInChildren<PlayerMain>();
        public EnemyBase EnemyObj => GetComponentInChildren<EnemyBase>();

        void Awake()
        {
        }

        void Start()
        {

        }

        public void DestroyObj()
        {
            Destroy(gameObject);
        }
        public void DestroyObj(float delay)
        {
            Destroy(gameObject, delay);
        }

        public void Turn(int worldDir)
        {
            if (worldDir == 0) return;

            Vector3 front = worldDir > 0 ? Vector3.forward : Vector3.back;
            transform.rotation = Quaternion.LookRotation(front, transform.up);
        }
        public void TurnToFloatX(float moveX)
        {
            if (moveX.ExIsAlmostZero()) return;

            int worldDir = moveX > 0 ? 1 : -1;
            Turn(worldDir);
        }
        public void TurnToTarget(Transform target)
        {
            if (target == null) return;

            int worldDir = target.position.x > transform.position.x ? 1 : -1;
            Turn(worldDir);
        }
        public void TurnToPosition(Vector2 targetPos)
        {
            int worldDir = targetPos.x > transform.position.x ? 1 : -1;
            Turn(worldDir);
        }

    }
}
