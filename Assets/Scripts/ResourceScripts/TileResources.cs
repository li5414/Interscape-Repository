using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileResources : MonoBehaviour {

    public RuleTile tileSandRule;

    public Sprite[] grassDetailsChunk;

    public static int GetTilesBlockCount(Tilemap tilemap, Vector3Int min, Vector3Int max) {
        int count = 0;
        for (int i = 0; i < max.y - min.y; i++) {
            Vector3Int minRowPos = new Vector3Int(min.x, min.y + i, min.z);
            Vector3Int maxRowPos = new Vector3Int(max.x - 1, min.y + i, max.z);

            count += tilemap.GetTilesRangeCount(minRowPos, maxRowPos);
        }
        return count;
    }
}
