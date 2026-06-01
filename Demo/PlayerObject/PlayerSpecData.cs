using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CreateAssetMenu(fileName = "PlayerSpecData", menuName = "Demo/PlayerSpecData")]
    public class PlayerSpecData : ScriptableObject
    {
        [SerializeField] private string _PlayerID = "";
        [SerializeField] private string _PlayerName = "";
        [SerializeField] private string _PlayerDesc = "";
        [SerializeField] private Sprite _PlayerIcon = null;
        [SerializeField] private List<SpecValueInfo> _Specs = new List<SpecValueInfo>();

        public string PlayerID => _PlayerID;
        public string PlayerName => _PlayerName;
        public string PlayerDesc => _PlayerDesc;
        public Sprite PlayerIcon => _PlayerIcon;

        public IReadOnlyList<SpecValueInfo> Specs => _Specs;

    }
}