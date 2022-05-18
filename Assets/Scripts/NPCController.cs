using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {
    Animator a;
    NPCState state = NPCState.STANDING;
    Facing facing = Facing.BotRight;

    void Start() {
        a = gameObject.GetComponent<Animator>();
        StartCoroutine(Stand());
    }

    IEnumerator Stand() {
        while (true) {
            facing = GetRandomEnum<Facing>();
            state = NPCState.STANDING;
            idleAnimation();
            yield return new WaitForSeconds(Random.Range(3, 6));
        }
    }

    private void idleAnimation() {
        switch (facing) {
            case Facing.Up:
                a.Play("BackIdle");
                break;
            case Facing.TopRight:
            case Facing.TopLeft:
                a.Play("BackAngledIdle");
                break;
            case Facing.Right:
            case Facing.Left:
                a.Play("SideIdle");
                break;
            case Facing.BotLeft:
            case Facing.BotRight:
                a.Play("FrontAngledIdle");
                break;
            case Facing.Down:
                a.Play("FrontIdle");
                break;
            default:
                Debug.LogError("NPC idle animation could not be found");
                break;
        }
    }

    private void walkAnimation() {
        switch (facing) {
            case Facing.Up:
                a.Play("BackWalk");
                break;
            case Facing.TopRight:
            case Facing.TopLeft:
                a.Play("BackAngledWalk");
                break;
            case Facing.Right:
            case Facing.Left:
                a.Play("SideWalk");
                break;
            case Facing.BotLeft:
            case Facing.BotRight:
                a.Play("FrontAngledWalk");
                break;
            case Facing.Down:
                a.Play("FrontWalk");
                break;
            default:
                Debug.LogError("NPC walk animation could not be found");
                break;
        }
    }

    private void runAnimation() {
        switch (facing) {
            case Facing.Up:
                a.Play("BackRun");
                break;
            case Facing.TopRight:
            case Facing.TopLeft:
                a.Play("BackAngledRun");
                break;
            case Facing.Right:
            case Facing.Left:
                a.Play("SideRun");
                break;
            case Facing.BotLeft:
            case Facing.BotRight:
                a.Play("FrontAngledRun");
                break;
            case Facing.Down:
                a.Play("FrontRun");
                break;
            default:
                Debug.LogError("NPC run animation could not be found");
                break;
        }
    }

    static T GetRandomEnum<T>() {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }
}

public enum NPCState {
    STANDING,
    WALKING,
    RUNNING,
}


