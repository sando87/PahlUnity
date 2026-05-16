using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.Events;

public class InteractableCollider : MonoBehaviour
{
    [SerializeField] InteractMask _MyProperty = InteractMask.Everything;
    [SerializeField] InteractMask _InteractableWith = InteractMask.Everything;

    public InteractMask MyProperty => _MyProperty;
    public bool LockInteract { get; set; } = false;

    public UnityEvent<Collider2D> OnInteractEnter;
    public UnityEvent<Collider2D> OnInteractLeave;
    public UnityEvent<BaseObject, InteractMask> OnInteractSignal;

    private Collider2D mCollider = null;

    void Awake()
    {
        mCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInteractable(collision.collider))
        {
            DoInteractEnter(collision.collider);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (IsInteractable(collision.collider))
        {
            DoInteractLeave(collision.collider);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInteractable(collision))
        {
            DoInteractEnter(collision);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInteractable(collision))
        {
            DoInteractLeave(collision);
        }
    }

    private bool IsInteractable(Collider2D other)
    {
        if (LockInteract)
            return false;

        // 콜라이더 이벤트는 콜라이더가 붙어있는 객체에게만 이벤트가 전달 되도록 하기 위함
        if (gameObject != mCollider.gameObject)
            return false;

        InteractableCollider opp = other.GetComponent<InteractableCollider>();
        if (opp == null)
            return false;

        InteractMask mask = _InteractableWith & opp._MyProperty;
        return mask != InteractMask.Nothing;
    }
    private void DoInteractEnter(Collider2D other)
    {
        OnInteractEnter?.Invoke(other);
    }
    private void DoInteractLeave(Collider2D other)
    {
        OnInteractLeave?.Invoke(other);
    }
    public void InvokeInteractSignal(BaseObject invoker, InteractMask signal)
    {
        if (LockInteract)
            return;

        // 콜라이더 이벤트는 콜라이더가 붙어있는 객체에게만 이벤트가 전달 되도록 하기 위함
        if (gameObject != mCollider.gameObject)
            return;

        InteractMask mask = _InteractableWith & signal;
        if (mask != InteractMask.Nothing)
        {
            OnInteractSignal?.Invoke(invoker, signal);
        }
    }

}

[System.Flags]
public enum InteractMask : uint
{
    Nothing = 0,
    Unit = 1 << 0,
    Skill = 1 << 1,
    Terrain = 1 << 2,
    Projectile = 1 << 3,
    Props = 1 << 4,
    Item = 1 << 5,
    DetectSignal = 1 << 6,
    TriggerSignal = 1 << 7,
    Everything = 0xffffffff
}