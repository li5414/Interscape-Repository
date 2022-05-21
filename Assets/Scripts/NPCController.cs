using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCController : MonoBehaviour {

    static float WALK_SPEED = 2f;
    static int WANDER_RANGE = 10;
    static int WANDER_BOUNDARY_RANGE = 20;
    public GameObject showPathObject;
    Animator a;
    NPCState state = NPCState.STANDING;
    Facing facing = Facing.BotRight;
    Vector3 wanderAnchorPoint;


    void Start() {
        a = gameObject.GetComponent<Animator>();
        StartCoroutine(Stand());
        wanderAnchorPoint = transform.position;
    }

    IEnumerator Stand() {
        while (true) {
            facing = GetRandomEnum<Facing>();
            state = NPCState.STANDING;
            idleAnimation();
            yield return new WaitForSeconds(Random.Range(3, 6));

            if (Random.value < 0.3f) {
                StartCoroutine(Walk());
                yield break;
            }
        }
    }

    IEnumerator Walk() {
        Vector2Int? nullableTarget = pickRandomWanderTarget();
        if (!nullableTarget.HasValue) {
            StartCoroutine(Stand());
            yield break;
        }

        Vector2Int target = nullableTarget.Value;
        state = NPCState.WALKING;
        walkAnimation();

        List<PathFinderTile> path = PathFinder.FindPath(new Vector2Int((int)transform.position.x, (int)transform.position.y), target);
        // Debug.Log("Count:" + path.Count);
        // Debug.Log(path.First().X + ", " + path.First().Y);
        GameObject pathParent = showPath(path);

        while (path != null && path.Count != 0 && !isOnTileCenter(target)) {
            // update the path
            if (isOnTileCenter(new Vector2Int(path.First().X, path.First().Y))) {
                path.RemoveAt(0);
                Destroy(pathParent.transform.GetChild(0).gameObject);
                // Debug.Log("updated the path");
                // Debug.Log(path.First().X + ", " + path.First().Y);
            }

            // get current position and intermediate target
            Vector2 currentPos = new Vector2(
                transform.position.x,
                transform.position.y);
            Vector2 intermediateTarget = new Vector2(
                path.First().X + 0.5f,
                path.First().Y + 0.5f);

            // get direction it needs to travel
            Vector2 direction = (intermediateTarget - currentPos).normalized;

            // face the correct direction
            Facing newFacing = facingFromVector(direction);
            if (newFacing != facing) {
                facing = newFacing;
                walkAnimation();
            }
            this.transform.Translate(direction * WALK_SPEED * Time.deltaTime);
            yield return null;
        }
        Destroy(pathParent);
        StartCoroutine(Stand());
        yield break;
    }

    private GameObject showPath(List<PathFinderTile> path) {
        GameObject parent = Instantiate(new GameObject("Path"));

        if (path != null) {
            foreach (PathFinderTile tile in path) {
                GameObject obj = Instantiate(showPathObject, new Vector3(tile.X + 0.5f, tile.Y + 0.5f, 150), Quaternion.identity);
                obj.transform.SetParent(parent.transform, true);
            }
        }
        return parent;
    }

    private bool isOnTileCenter(Vector2Int tile) {
        bool isOnTileCenter = Vector2.Distance(transform.position, new Vector2(tile.x + 0.5f, tile.y + 0.5f)) < 0.1;
        // if (isOnTileCenter)
        //     Debug.Log("I'm on the tile center");
        return isOnTileCenter;
    }

    Vector2Int? pickRandomWanderTarget() {
        int x = Random.Range((int)transform.position.x - WANDER_RANGE, (int)transform.position.x + WANDER_RANGE);
        x = Mathf.Clamp(x, (int)wanderAnchorPoint.x - WANDER_BOUNDARY_RANGE, (int)wanderAnchorPoint.x + WANDER_BOUNDARY_RANGE);

        int y = Random.Range((int)transform.position.y - WANDER_RANGE, (int)transform.position.y + WANDER_RANGE);
        y = Mathf.Clamp(y, (int)wanderAnchorPoint.y - WANDER_BOUNDARY_RANGE, (int)wanderAnchorPoint.y + WANDER_BOUNDARY_RANGE);

        if (PathFinder.IsWalkable(x, y))
            return new Vector2Int(x, y);
        return null;
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

    static Facing facingFromVector(Vector2 vector) {
        int direction = (((int)Mathf.Round(Mathf.Atan2(vector.y, vector.x) / (2 * 3.14159f / 8))) + 8) % 8;
        switch (direction) {
            case 0:
                return Facing.Right;
            case 1:
                return Facing.TopRight;
            case 2:
                return Facing.Up;
            case 3:
                return Facing.TopLeft;
            case 4:
                return Facing.Left;
            case 5:
                return Facing.BotLeft;
            case 6:
                return Facing.Down;
            case 7:
                return Facing.BotRight;
            default:
                Debug.LogError("Yikes");
                return Facing.BotRight;
        }
    }
}

public enum NPCState {
    STANDING,
    WALKING,
    RUNNING,
}


