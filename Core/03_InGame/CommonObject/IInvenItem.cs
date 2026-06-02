using UnityEngine;

namespace PahlUnity
{
    public interface IInvenItem
    {
        int ResourceID { get; }
        bool IsStackable { get; }
        int MaxStackCount { get; }
    }
}