using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemObject : MonoBehaviour
{
    public ItemInfo ItemInfo { get; private set; }

    public static ItemObject TryCreateNewItem(Vector3 position, Quaternion rotation, int playerLevel)
    {
        string itemID = ItemInfo.GetRandomItemID(playerLevel);
        if (itemID.Length <= 0)
            return null;

        ItemInfo itemInfo = new ItemInfo();
        itemInfo.InitItem(itemID);

        ItemObject itemObj = Instantiate(itemInfo.ResourceData.AssetData.Prefab, position, rotation);
        itemObj.ItemInfo = itemInfo;
        return itemObj;
    }
    public static ItemObject CreateNewItem(Vector3 position, Quaternion rotation, ItemInfo item)
    {
        ItemObject itemObj = Instantiate(item.ResourceData.AssetData.Prefab, position, rotation);
        itemObj.ItemInfo = item;
        return itemObj;
    }

    // public void OnPickedUpBy(Collider2D col)
    // {
    //     BaseObject pickerPlayer = col.GetComponentInParent<BaseObject>();
    //     ItemInventory inventory = pickerPlayer.GetComponentInChildren<ItemInventory>();
    //     inventory.AddItem(ItemInfo);

    //     OnPickedUp();
    // }

    public void OnPickedUp()
    {
        Destroy(gameObject);
    }
}
