using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/
public class PathFinder : MonoBehaviour {
    private static int MAX_LOOK_X = 30;
    private static int MAX_LOOK_Y = 30;

    public static List<PathFinderTile> FindPath(Vector2Int startPos, Vector2Int finishPos) {
        var start = new PathFinderTile();
        start.X = startPos.x;
        start.Y = startPos.y;
        var finish = new PathFinderTile();
        finish.X = finishPos.x;
        finish.Y = finishPos.y;

        start.SetDistance(finish.X, finish.Y);
        var activeTiles = new List<PathFinderTile>();
        activeTiles.Add(start);
        var visitedTiles = new List<PathFinderTile>();
        // Debug.Log("Looking for a path");

        while (activeTiles.Any()) {
            var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();
            if (checkTile.X == finish.X && checkTile.Y == finish.Y) {
                // Debug.Log("Found a path!");
                return retraceSteps(checkTile);
            }
            visitedTiles.Add(checkTile);
            activeTiles.Remove(checkTile);
            var walkableTiles = getWalkableTiles(start, checkTile, finish);
            foreach (var walkableTile in walkableTiles) {
                //We have already visited this tile so we don't need to do so again!
                if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    continue;
                //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y)) {
                    var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                    if (existingTile.CostDistance > checkTile.CostDistance) {
                        activeTiles.Remove(existingTile);
                        activeTiles.Add(walkableTile);
                    }
                } else {
                    //We've never seen this tile before so add it to the list. 
                    activeTiles.Add(walkableTile);
                }
            }
        }
        // Debug.Log("No Path Found! :(");
        return null;
    }

    private static List<PathFinderTile> retraceSteps(PathFinderTile tile) {
        List<PathFinderTile> tiles = new List<PathFinderTile>();
        while (tile != null) {
            tiles.Add(tile);
            tile = tile.Parent;
        }
        tiles.Reverse();
        // Remove last element cuz that would be the starting tile
        // if (tiles.Count > 0) {
        //     tiles.RemoveAt(tiles.Count - 1);
        // }
        return tiles;
    }

    private static List<PathFinderTile> getWalkableTiles(PathFinderTile startTile, PathFinderTile currentTile, PathFinderTile targetTile) {
        var possibleTiles = new List<PathFinderTile>() {
            new PathFinderTile {
                X = currentTile.X,
                Y = currentTile.Y - 1,
                Parent = currentTile,
                Cost = currentTile.Cost + 1
            },
            new PathFinderTile {
                X = currentTile.X,
                Y = currentTile.Y + 1,
                Parent = currentTile,
                Cost = currentTile.Cost + 1
            },
            new PathFinderTile {
                X = currentTile.X - 1,
                Y = currentTile.Y,
                Parent = currentTile,
                Cost = currentTile.Cost + 1
            },
            new PathFinderTile {
                X = currentTile.X + 1,
                Y = currentTile.Y,
                Parent = currentTile,
                Cost = currentTile.Cost + 1
            },
        };

        // add corner tiles
        if (CanCutCorner(currentTile.X - 1, currentTile.Y - 1, currentTile)) {
            possibleTiles.Add(new PathFinderTile {
                X = currentTile.X - 1,
                Y = currentTile.Y - 1,
                Parent = currentTile,
                Cost = currentTile.Cost + 1.41f
            });
        }
        if (CanCutCorner(currentTile.X + 1, currentTile.Y + 1, currentTile)) {
            possibleTiles.Add(new PathFinderTile {
                X = currentTile.X + 1,
                Y = currentTile.Y + 1,
                Parent = currentTile,
                Cost = currentTile.Cost + 1.41f
            });
        }
        if (CanCutCorner(currentTile.X - 1, currentTile.Y + 1, currentTile)) {
            possibleTiles.Add(new PathFinderTile {
                X = currentTile.X - 1,
                Y = currentTile.Y + 1,
                Parent = currentTile,
                Cost = currentTile.Cost + 1.41f
            });
        }
        if (CanCutCorner(currentTile.X + 1, currentTile.Y - 1, currentTile)) {
            possibleTiles.Add(new PathFinderTile {
                X = currentTile.X + 1,
                Y = currentTile.Y - 1,
                Parent = currentTile,
                Cost = currentTile.Cost + 1.41f
            });
        }

        possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));
        return possibleTiles
                .Where(tile => tile.X >= startTile.X - MAX_LOOK_X &&
                            tile.X <= startTile.X + MAX_LOOK_X)
                .Where(tile => tile.Y >= startTile.Y - MAX_LOOK_Y &&
                            tile.Y <= startTile.Y + MAX_LOOK_Y)
                .Where(tile => IsWalkable(tile.X, tile.Y))
                .ToList();
    }
    private static bool CanCutCorner(int x, int y, PathFinderTile currentTile) {
        Vector2Int[] cornerTileNeighbours = getNeighbouringCoords(new Vector2Int(x, y));
        Vector2Int[] currentTileNeighbours = getNeighbouringCoords(new Vector2Int(currentTile.X, currentTile.Y));

        foreach (Vector2Int vector1 in cornerTileNeighbours) {
            foreach (Vector2Int vector2 in currentTileNeighbours) {
                if (vector1.Equals(vector2)) {
                    if (!IsWalkable(vector1.x, vector1.y)) {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private static Vector2Int[] getNeighbouringCoords(Vector2Int pos) {
        Vector2Int[] result = new Vector2Int[4];
        result[0] = new Vector2Int(pos.x + 1, pos.y);
        result[1] = new Vector2Int(pos.x, pos.y + 1);
        result[2] = new Vector2Int(pos.x - 1, pos.y);
        result[3] = new Vector2Int(pos.x, pos.y - 1);
        return result;
    }
    public static bool IsWalkable(int x, int y) {
        // TODO figure out if z is important here
        Vector3 pos = new Vector3(x + 0.5f, y + 0.5f, 200);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, -Vector2.zero);

        for (int i = 0; i < hits.Length; i++) {
            // Debug.Log(hits[i].transform.gameObject.name);
            if (hits[i].transform != null) {
                Collider2D collider = hits[i].transform.gameObject.GetComponent<Collider2D>();
                if (collider != null && !collider.isTrigger && collider.gameObject.tag != "Door") {
                    // Debug.Log("Not walkable");
                    return false;
                }
            }
        }
        return true;
    }
}

public class PathFinderTile {
    public int X { get; set; }
    public int Y { get; set; }
    public float Cost { get; set; }
    public int Distance { get; set; }
    public float CostDistance => Cost + Distance;

    // the tile we came from to get here
    public PathFinderTile Parent { get; set; }

    // distance is the estimated distance ignoring walls to our target
    public void SetDistance(int targetX, int targetY) {
        this.Distance = Mathf.Abs(targetX - X) + Mathf.Abs(targetY - Y);
    }
}
