using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CreateAssetMenu(fileName = "PlayerSpecData", menuName = "Demo/PlayerSpecData")]
    public class PlayerSpecData : ScriptableObject, ISpecContainer
    {
        [SerializeField] private string _PlayerName = "";
        [SerializeField] private string _PlayerDesc = "";
        [SerializeField] private Sprite _PlayerIcon = null;
        [SerializeField] private List<SpecFieldRaw> _Specs = new List<SpecFieldRaw>();

        public string PlayerID => name;
        public string PlayerName => _PlayerName;
        public string PlayerDesc => _PlayerDesc;
        public Sprite PlayerIcon => _PlayerIcon;

        public IReadOnlyList<SpecFieldRaw> Specs => _Specs;

    }
}