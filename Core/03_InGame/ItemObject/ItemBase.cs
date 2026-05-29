using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class ItemBase : MonoBehaviour
    {
        public ItemInfo ItemInfo { get; private set; }

        public static ItemBase TryCreateNewItem(Vector3 position, Quaternion rotation, int playerLevel)
        {
            string itemID = ItemInfo.GetRandomItemID(playerLevel);
            if (itemID.Length <= 0)
                return null;

            ItemInfo itemInfo = new ItemInfo();
            itemInfo.InitItem(itemID);

            ItemBase itemObj = Instantiate(itemInfo.ResourceData.Prefab, position, rotation);
            itemObj.ItemInfo = itemInfo;
            return itemObj;
        }
        public static ItemBase CreateNewItem(Vector3 position, Quaternion rotation, ItemInfo item)
        {
            ItemBase itemObj = Instantiate(item.ResourceData.Prefab, position, rotation);
            itemObj.ItemInfo = item;
            return itemObj;
        }

        public void OnPickedUp()
        {
            Destroy(gameObject);
        }
    }
}
