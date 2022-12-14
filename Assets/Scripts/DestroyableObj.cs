using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyableObj : MonoBehaviour {
    [SerializeField] float health = 100;
    [SerializeField] DamageType damageType;
    public string dropItemName;
    public GameObject parentObj;

    [Space]
    public AnimationClip damageClip;
    public ScriptAnimation damageAnimation;
    public AnimationClip deathClip;

    [Space]
    public bool isUsingRuleTiles;
    private Tilemap tilemap;
    private Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
        if (parentObj == null) {
            parentObj = this.gameObject;
        }
        if (isUsingRuleTiles) {
            tilemap = this.gameObject.transform.parent.gameObject.GetComponent<Tilemap>();
            if (!tilemap)
                Debug.LogError("Could not find parent tilemap", this);
            if (tilemap.gameObject.GetComponent<DestroyableTilemap>() != null) {
                dropItemName = tilemap.gameObject.GetComponent<DestroyableTilemap>().itemDropName;
            }
        }
    }

    public void damage(Tool tool) {
        health -= tool.getDamage();

        // check health reaches zero
        if (health < 0) {
            if (deathClip != null) {
                StartCoroutine(Die(deathClip.length));
                anim.Play(deathClip.name);
            } else {
                actuallyDie();
            }
            return;
        }

        // else play damage animation
        if (damageClip != null)
            anim.Play(damageClip.name);
        else if (damageAnimation != null)
            damageAnimation.Animate();
    }

    public DamageType getDamageType() {
        return damageType;
    }

    IEnumerator Die(float delay) {
        // play your sound
        yield return new WaitForSeconds(delay);
        actuallyDie();
    }

    private void actuallyDie() {
        if (dropItemName != "") {
            // TODO fix quantity
            ItemDrop.DropItemsAt(ItemRes.MakeItemCopy(dropItemName), 1, this.transform.position);
        }
        if (tilemap != null) {
            Vector3Int cellPos = tilemap.WorldToCell(this.transform.position);
            tilemap.SetTile(cellPos, null);
        }
        Destroy(parentObj);
    }
}


