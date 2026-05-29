using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    public class EnemyController : MonoBehaviour
    {
        protected BaseObject mBase = null;

        void Awake()
        {
            mBase = this.ExGetBase();
        }
    }
}