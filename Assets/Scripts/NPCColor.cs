using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCColor : MonoBehaviour {
    static Color32[] HAIR_COLOURS = {
        new Color32(112, 73, 56, 255),
        new Color32(195, 163, 117, 255),
        new Color32(188, 112, 81, 255),
        new Color32(65, 57, 53, 255),
        new Color32(181, 162, 162, 255),
        new Color32(195, 161, 127, 255),
        new Color32(48, 45, 44, 255),
        new Color32(101, 64, 51, 255),
        new Color32(164, 122, 96, 255)
    };
    static Color32[] EYE_COLOURS = {
        new Color32(67, 71, 85, 255),
        new Color32(113, 42, 42, 255),
        new Color32(139, 84, 31, 255),
        new Color32(74, 100, 62, 255),
        new Color32(109, 70, 53, 255),
        new Color32(52, 44, 40, 255)
    };
    static Color32[] SKIN_COLOURS = {
        new Color32(214, 169, 145, 255),
        new Color32(198, 146, 121, 255),
        new Color32(176, 118, 94, 255),
        new Color32(133, 83, 66, 255),
        new Color32(104, 59, 47, 255)
    };
    static Color32[] CLOTHES_COLOURS_1 = {
        new Color32(161, 148, 149, 255),
        new Color32(123, 105, 106, 255),
        new Color32(108, 112, 128, 255),
        new Color32(69, 64, 62, 255),
        new Color32(68, 63, 58, 255),
        new Color32(122, 79, 61, 255),
        new Color32(112, 73, 56, 255)
    };
    static Color32[] CLOTHES_COLOURS_2 = {
        new Color32(208, 200, 196, 255),
        new Color32(212, 204, 198, 255),
        new Color32(211, 202, 203, 255),
        new Color32(146, 130, 131, 255),
        new Color32(196, 194, 194, 255),
        new Color32(202, 192, 189, 255)
    };

    Color32 hairColour;
    Color32 eyeColour;
    Color32 skinColour;
    Color32 clothesColour1;
    Color32 clothesColour2;

    void Start() {
        hairColour = HAIR_COLOURS[Random.Range(0, HAIR_COLOURS.Length)];
        eyeColour = EYE_COLOURS[Random.Range(0, EYE_COLOURS.Length)];
        skinColour = SKIN_COLOURS[Random.Range(0, SKIN_COLOURS.Length)];
        clothesColour1 = CLOTHES_COLOURS_1[Random.Range(0, CLOTHES_COLOURS_1.Length)];
        clothesColour2 = CLOTHES_COLOURS_2[Random.Range(0, CLOTHES_COLOURS_2.Length)];
        applyColours();
    }

    public void applyColours() {
        gameObject.GetComponent<SpriteRenderer>().color = skinColour;

        gameObject.transform.Find("LongStraightHair").GetComponent<SpriteRenderer>().color = hairColour;

        gameObject.transform.Find("Eyes").GetComponent<SpriteRenderer>().color = eyeColour;

        // gameObject.transform.Find("LongOveralls");
    }
}
