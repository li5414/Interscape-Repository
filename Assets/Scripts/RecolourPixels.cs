using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolourPixels : MonoBehaviour {
    Material material;
    public Color32[] originalColours;
    public Color32[] newColours;
    private MaterialPropertyBlock propBlock;

    void Start() {
        material = new Material(Shader.Find("Unlit/ClothesRecolour"));
        gameObject.GetComponent<SpriteRenderer>().material = material;
        updateMaterialTexture();
    }

    void updateMaterialTexture() {
        Texture2D texture = new Texture2D(256, 1, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color[] colors = new Color[texture.width * texture.height];
        texture.SetPixels(colors);

        for (int i = 0; i < originalColours.Length; i++) {
            int rValue = originalColours[i].r;
            texture.SetPixel(rValue, 0, newColours[i]);
        }
        texture.Apply();
        gameObject.GetComponent<SpriteRenderer>().material.SetTexture("_SwapTex", texture);
    }
}