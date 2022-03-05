using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingGeneration : MonoBehaviour
{
    public Tilemap tilemap;
    public RuleTile buildingReference;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRuinAt(0, 0, seed: Random.Range(0, 100));
    }

    public void GenerateRuinAt(int x, int y, int maxBuildings=10, int seed=0) {
        System.Random rng = new System.Random (seed);
        int boundaryRadius = rng.Next(10, 30);
        int maxBuildingSize = 8;
        int minBuildingSize = 3;


        for (int i = 0; i < maxBuildings; i++) {
            int buildingPosX = rng.Next(x - boundaryRadius, x + boundaryRadius - maxBuildingSize);
            int buildingPosY = rng.Next(y - boundaryRadius, y + boundaryRadius - maxBuildingSize);

            int buildingLenX = rng.Next(minBuildingSize, maxBuildingSize); // from bottom left corner
            int buildingLenY = rng.Next(minBuildingSize, maxBuildingSize);

            for (int pos = buildingPosX; pos <= buildingPosX + buildingLenX; pos++) {
                tilemap.SetTile(new Vector3Int(pos, buildingPosY, 0), buildingReference);
                tilemap.SetTile(new Vector3Int(pos, buildingPosY + buildingLenY, 0), buildingReference);
            }

            for (int pos = buildingPosY; pos <= buildingPosY + buildingLenY; pos++) {
                tilemap.SetTile(new Vector3Int(buildingPosX, pos, 0), buildingReference);
                tilemap.SetTile(new Vector3Int(buildingPosX + buildingLenX, pos, 0), buildingReference);
            }
        }
    }
}
