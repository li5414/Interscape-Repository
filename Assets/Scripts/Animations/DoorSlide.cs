using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlide : MonoBehaviour
{
    Animator anim;
    Transform player;
    public float distanceToOpen;

    private bool isMoving;
    private bool isOpen = false;

    public AnimationClip doorSlideOpenClip;
    public AnimationClip doorSlideCloseClip;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.position, transform.parent.transform.position) < distanceToOpen) {
            if (!isMoving && !isOpen)
                StartCoroutine("doorSlideOpen");
        } else {
            if (!isMoving && isOpen)
                StartCoroutine("doorSlideClose");
        }
    }

    private IEnumerator doorSlideOpen() {
        anim.Play("DoorSlideOpen");
        isMoving = true;
        yield return new WaitForSeconds(doorSlideOpenClip.length);
        isMoving = false;
        isOpen = true;
    }
    private IEnumerator doorSlideClose() {
        anim.Play("DoorSlideClose");
        isMoving = true;
        yield return new WaitForSeconds(doorSlideCloseClip.length);
        isMoving = false;
        isOpen = false;
        this.transform.position = new Vector3(0, 0, 0); // reset position
    }
}
