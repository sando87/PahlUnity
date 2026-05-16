using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "PahlBit/Item")]
public class ItemAssetData : ScriptableObject
{
    public Sprite Icon;
    public ItemObject Prefab;
}