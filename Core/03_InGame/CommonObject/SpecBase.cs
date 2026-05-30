using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    public class SpecBase : MonoBehaviour
    {
        public virtual float MaxHealth { get; }
        public virtual float MaxMana { get; }
        public virtual float MaxShield { get; }
        public virtual float BaseAttack { get; }
        public virtual float PhyDefence { get; }
        public virtual float MoveSpeed { get; }
        public virtual float AttackSpeed { get; }

        SpecOption mTotalOption = new SpecOption();
        List<SpecOption> mLinkedOptions = new List<SpecOption>();

        public void LinkOption(SpecOption specOption)
        {
            specOption.IsDirty = true;
            mLinkedOptions.Add(specOption);
        }

        public SpecOption Option => GetTotalOption();

        SpecOption GetTotalOption()
        {
            if (IsDirty())
            {
                RefreshTotalOption();
            }

            return mTotalOption;
        }
        void RefreshTotalOption()
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