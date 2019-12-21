using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public float renderDist = 3;
	public GameObject chunkGO;
	public GameObject player;

	Dictionary<Vector2, Chunk> chunkMap;

	private void Awake ()
	{
		chunkMap = new Dictionary<Vector2, Chunk> ();
	}


	void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		FindChunksToLoad ();
		DeleteChunks ();
	}

	void FindChunksToLoad () {

		int xPos = (int)player.transform.position.x;
		int yPos = (int)player.transform.position.y;

		for (int i = xPos - Chunk.size; i < xPos + 2*Chunk.size; i+= Chunk.size) {
			for (int j = yPos - Chunk.size; j < yPos + 2*Chunk.size; j += Chunk.size) {
				MakeChunkAt (i, j);
			}
		}
	}

	void MakeChunkAt (int x, int y)
	{
		x = Mathf.FloorToInt (x / (float)Chunk.size) * Chunk.size;
		y = Mathf.FloorToInt (y / (float)Chunk.size) * Chunk.size;

		if (chunkMap.ContainsKey(new Vector2(x, y)) == false) {
			GameObject go = Instantiate (chunkGO, new Vector3 (x, y, 0f), Quaternion.identity);
			chunkMap.Add (new Vector2 (x, y), go.GetComponent<Chunk> ());
		}
	}

	void DeleteChunks ()
	{
		List<Chunk> deleteChunks = new List<Chunk> (chunkMap.Values);
		Queue<Chunk> deleteQueue = new Queue<Chunk> ();

		for (int i = 0; i < deleteChunks.Count; i++) {

			float distance = Vector3.Distance (transform.position,
				deleteChunks [i].transform.position);

			if (distance > renderDist * Chunk.size) {
				deleteQueue.Enqueue (deleteChunks [i]);
			}
		}

		while (deleteQueue.Count > 0) {

			Chunk chunk = deleteQueue.Dequeue ();
			chunkMap.Remove (chunk.transform.position);
			Destroy (chunk.gameObject);
			
		}
	}
}

