using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CreateAssetMenu(
    fileName = "ItemDatabase",
    menuName = "Demo/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemResourceData> _ItemList;

        public List<ItemResourceData> ItemList => _ItemList;
    }
}