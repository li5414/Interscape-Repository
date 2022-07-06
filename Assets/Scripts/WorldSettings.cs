using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettings : MonoBehaviour, IDataPersistence {
    // seed to help with world generation
    public int SEED = 130;

    // pseudo random number generator will help generate the same random numbers each run
    public System.Random PRNG;

    // render distance in terms of chunks
    public int RENDER_DIST = 5;

    // TODO not sure if need
    void Awake() {
        PRNG = new System.Random(SEED);
    }

    public void LoadData(GameData data) {
        SEED = data.worldData.seed;
        PRNG = new System.Random(SEED);
    }

    public void SaveData(GameData data) {
        data.worldData.seed = SEED;
    }
}
