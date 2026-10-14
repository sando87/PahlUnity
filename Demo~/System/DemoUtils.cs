using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    public static class DemoUtils
    {
        static public int GetAttackableLayerMask(int layer)
        {
            if (layer == LayerID.Player)
                return 1 << LayerID.Enemy;
            else if (layer == LayerID.Enemy)
                return 1 << LayerID.Player;
            else
                return 0;
        }
    }
}