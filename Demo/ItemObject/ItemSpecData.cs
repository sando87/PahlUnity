using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CreateAssetMenu(fileName = "ItemSpecData", menuName = "Demo/ItemSpecData")]
    public class ItemSpecData : ScriptableObject
    {
        [SerializeField] private string _ItemID = "";
        [SerializeField] private string _ItemName = "";
        [SerializeField] private string _ItemDesc = "";
        [SerializeField] private Sprite _ItemIcon = null;
        [SerializeField] private List<SpecValueInfo> _Specs = new List<SpecValueInfo>();

        public string ItemID => _ItemID;
        public string ItemName => _ItemName;
        public string ItemDesc => _ItemDesc;
        public Sprite ItemIcon => _ItemIcon;

        public IReadOnlyList<SpecValueInfo> Specs => _Specs;

    }
}