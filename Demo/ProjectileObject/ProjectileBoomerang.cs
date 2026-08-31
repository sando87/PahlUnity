using UnityEngine;
using PahlUnity;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using NaughtyAttributes;

namespace PahlUnity.Demo
{
    public class ProjectileBoomerang : ProjectileBase
    {
        [SerializeField] float SpeedReduction = -15;

        private bool mIsReturning = false;
        private Vector3 mForwardDir = Vector3.zero;

        public float ReturnDistance { get; set; } = 10;

        protected override void Update()
        {
            base.Update();

            if (mIsReturning)
            {
                Vector3 diffFromStartPos = transform.position - mStartPos;
                if (Vector3.Dot(diffFromStartPos.normalized, mForwardDir) < 0)
                {
                    DoEndProjectile();
                }
            }
            else
            {
                Vector3 diffFromStartPos = transform.position - mStartPos;
                if (diffFromStartPos.magnitude > ReturnDistance)
                {
                    mForwardDir = diffFromStartPos.normalized;
                    Stats.Acceleration = SpeedReduction;
                    mIsReturning = true;
                }
            }
        }
    }
}