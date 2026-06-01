using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CreateAssetMenu(fileName = "SkillSpecData", menuName = "Demo/SkillSpecData")]
    public class SkillSpecData : ScriptableObject
    {
        [SerializeField] private string _SkillID = "";
        [SerializeField] private string _SkillName = "";
        [SerializeField] private string _SkillDesc = "";
        [SerializeField] private Sprite _SkillIcon = null;
        [SerializeField] private List<SpecValueInfo> _Specs = new List<SpecValueInfo>();

        public string SkillID => _SkillID;
        public string SkillName => _SkillName;
        public string SkillDesc => _SkillDesc;
        public Sprite SkillIcon => _SkillIcon;

        public IReadOnlyList<SpecValueInfo> Specs => _Specs;

    }
}