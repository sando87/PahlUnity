using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class SpecBase : MonoBehaviour
    {
        public virtual float MaxHealth { get; }
        public virtual float MaxMana { get; }
        public virtual float MaxShield { get; }
        public virtual float BaseAttack { get; }

        public virtual float PhyDefence { get; }

        SpecOption mTotalOption = new SpecOption();
        List<SpecOption> mLinkedOptions = new List<SpecOption>();

        public void LinkOption(SpecOption specOption)
        {
            mLinkedOptions.Add(specOption);
        }

        public SpecOption Option => GetTotalOption();
        
        SpecOption GetTotalOption()
        {
            if (IsDirty())
                UpdateTotalOption();
            return mTotalOption;
        }
        void UpdateTotalOption()
        {
            mTotalOption.SetAllZero();
            foreach (SpecOption option in mLinkedOptions)
            {
                mTotalOption.Add(option);
                option.IsDirty = false;
            }
            mTotalOption.IsDirty = false;
        }
        bool IsDirty()
        {
            foreach (SpecOption option in mLinkedOptions)
                if (option.IsDirty)
                    return true;
            return false;
        }
    }
}