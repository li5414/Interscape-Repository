using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class RuleTileSibling : RuleTile<RuleTileSibling.Neighbor> {
    public int siblingGroup;
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Sibling = 1;
    }
    public override bool RuleMatch(int neighbor, TileBase tile) {
        RuleTileSibling myTile = tile as RuleTileSibling;
        switch (neighbor) {
            case Neighbor.Sibling:
                if (myTile && (myTile.siblingGroup == 1 || siblingGroup == 1)) // group 1 is special, do the opposite of the other groups (used for doors)
                    return myTile && myTile.siblingGroup != siblingGroup;
                return myTile && myTile.siblingGroup == siblingGroup;
        }
        return base.RuleMatch(neighbor, tile);
    }
}