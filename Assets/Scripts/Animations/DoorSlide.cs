using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlide : MonoBehaviour {
    public Animator anim;

    private bool isMoving;
    private bool isOpen = false;

    public AnimationClip doorSlideOpenClip;
    public AnimationClip doorSlideCloseClip;

    private bool isSomeoneClose = false;
    private int count = 0;

    void FixedUpdate() {
        if (isSomeoneClose) {
            if (!isMoving && !isOpen)
                StartCoroutine("doorSlideOpen");
        } else {
            if (!isMoving && isOpen)
                StartCoroutine("doorSlideClose");
        }
    }

    private IEnumerator doorSlideOpen() {
        anim.Play(doorSlideOpenClip.name);
        isMoving = true;
        yield return new WaitForSeconds(doorSlideOpenClip.length);
        isMoving = false;
        isOpen = true;
    }
    private IEnumerator doorSlideClose() {
        anim.Play(doorSlideCloseClip.name);
        isMoving = true;
        yield return new WaitForSeconds(doorSlideCloseClip.length);
        isMoving = false;
        isOpen = false;
        // this.transform.position = new Vector3(0, 0, 0); // reset position
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "NPC") {
            isSomeoneClose = true;
            count++;
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "NPC") {
            count--;
            if (count <= 0) {
                isSomeoneClose = false;
            }
        }
    }
}
