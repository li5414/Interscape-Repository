using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolourPixels : MonoBehaviour {
    public Texture2D texture;
    public Material material;
    public Color32[] originalColours;
    public Color32[] newColours;
    private MaterialPropertyBlock propBlock;

    void Start() {
        updateMaterialTexture();
    }

    void Update() {
        // updateMaterialTexture();
    }


    void updateMaterialTexture() {
        if (!texture || texture.width < 256)
            return;

        for (int i = 0; i < originalColours.Length; i++) {
            int rValue = originalColours[i].r;
            texture.SetPixel(rValue, 0, newColours[i]);
        }
        texture.Apply();
        material.SetTexture("_SwapTex", texture);
    }
}