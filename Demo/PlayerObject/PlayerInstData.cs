using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class PlayerInstData
    {
        private PlayerSpecData mSpecData;
        private int mResourceID;
        private long mInstanceID;

        public PlayerInstData(PlayerSpecData specData)
        {
            mSpecData = specData;
            mResourceID = mSpecData.PlayerID.ExGetStableHash32();
            mInstanceID = DateTime.Now.Ticks;
        }
        public PlayerInstData(PlayerSpecData specData, long instanceID)
        {
            mSpecData = specData;
            mResourceID = mSpecData.PlayerID.ExGetStableHash32();
            mInstanceID = instanceID;
        }

        public int ResourceID => mResourceID;
        public long InstanceID => mInstanceID;
        public PlayerSpecData SpecData => mSpecData;
    }
}
