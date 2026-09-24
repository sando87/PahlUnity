using UnityEngine;
using UnityEngine.Tilemaps;

namespace BulletRush
{
    [CreateAssetMenu]
    public class MyRuleTile : RuleTile<MyRuleTile.Neighbor>
    {
        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            public const int Custom = 3;
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            if (neighbor == Neighbor.Custom)
            {
                return tile is MyRuleTile;
            }
            else if (neighbor == Neighbor.NotThis)
            {
                return !(tile is MyRuleTile);
            }
            return base.RuleMatch(neighbor, tile);
        }
    }
}