using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Generation : MonoBehaviour {

	public Camera cam;
	public Chunk ChunkPrefab;
	public static int width = 16;
	public static int height = 32;
	public static int seed = 0;
    public static float smooth = 10f;

    public float chunksWide;

	void Start () {
        chunksWide = 4;
	}
	
	void OnGUI ()
	{
		if(GUI.Button( new Rect( 10, 10, 100, 30 ), "Generate" ))
		{
			Generate ();
		}
        GUI.Label(new Rect(15, 45, 100, 30), "Chunks wide: " + chunksWide);
        chunksWide = Mathf.Round(GUI.HorizontalSlider(new Rect(10, 70, 100, 30), chunksWide, 1.0F, 10.0F));

        GUI.Label(new Rect(15, 80, 175, 30), "Seed: " + seed);
        seed = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(10, 105, 100, 30), seed, 0f, int.MaxValue));
        seed = Mathf.Max(0, seed);

        GUI.Label(new Rect(15, 115, 175, 30), "Smooth: " + smooth);
        smooth = GUI.HorizontalSlider(new Rect(10, 135, 100, 30), smooth, 0.1f, 100f);
        smooth = Mathf.Max(0, smooth);
	}
	
	void Generate(){
		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Chunk");
		
		for (var i = 0; i < gameObjects.Length; i++)
            Destroy(gameObjects [i]);

        for (int x = 0; x < chunksWide; x++)
        {
            for(int z = 0; z < chunksWide; z++){
                Instantiate(ChunkPrefab, new Vector3(x * width, 0, z * width), Quaternion.identity);
            }
        }
        cam.transform.position = new Vector3((chunksWide * width) / 2, 80, -32);
    }
}
