using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshCollider))]
[RequireComponent (typeof(MeshFilter))]

public class Chunk : MonoBehaviour {
	
	public int width, height, seed;
	
	private byte[,,] map;
	public Mesh visualMesh;
	protected MeshRenderer meshRenderer;
	protected MeshCollider meshCollider;
	protected MeshFilter meshFilter;
	
	void Start () {
		width = Generation.width;
		height = Generation.height;
		seed = Generation.seed;
		meshRenderer = GetComponent<MeshRenderer> ();
		meshCollider = GetComponent<MeshCollider> ();
		meshFilter = GetComponent<MeshFilter> ();
		map = new byte[width,height,width];

		GenerateMap ();
		CreateVisualMesh ();
	}
	
	public float PerlinNoise(float x, float y){
		Random.seed = seed;
        float modifier = Random.Range(0.5f, 1.5f); 
		return Mathf.Abs (Mathf.PerlinNoise((x + (Random.value * 1000)) / (Generation.smooth * modifier),
                                 (y + (Random.value * 1000)) / (Generation.smooth * modifier)) * height);
	}
	
	public virtual void GenerateMap(){
		map = new byte[width, height, width];
		Random.seed = seed;
		for(int x = 0; x < width; x++)
		{
			for(int z = 0; z < width; z++)
			{
				byte brick = 1;
				float tempHeight = PerlinNoise(x + transform.position.x,    // Calculate the height of each
                                               z + transform.position.z);	// "column" of voxel blocks.
				int tempY = Mathf.RoundToInt(tempHeight);

				for(int y = 0; y < tempY; y++)
				{
					map[x, y, z] = brick;
				}
			}
		}
	}
	
	public virtual void CreateVisualMesh(){
		visualMesh = new Mesh ();
		List<Vector3> verts = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2> ();
		List<int> tris = new List<int> ();
		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				for(int z = 0; z < width; z++)
				{
					if(map[x, y, z] == 0) continue;
					
					byte brick = map[x,y,z];
					//Left wall
					if(IsTransparent(x-1, y, z))
						BuildFace (brick, new Vector3(x,y,z), Vector3.up, Vector3.forward, false, verts, uvs, tris);
					//Right wall
					if(IsTransparent(x+1, y, z))
						BuildFace (brick, new Vector3(x+1,y,z), Vector3.up, Vector3.forward, true, verts, uvs, tris);
					//Bottom wall
					if(IsTransparent(x, y-1, z))
						BuildFace (brick, new Vector3(x,y,z), Vector3.forward, Vector3.right, false, verts, uvs, tris);
					//Top wall
					if(IsTransparent(x, y+1, z))
						BuildFace (brick, new Vector3(x,y+1,z), Vector3.forward, Vector3.right, true, verts, uvs, tris);
					//Back wall
					if(IsTransparent (x, y, z-1))
						BuildFace (brick, new Vector3(x,y,z), Vector3.up, Vector3.right, true, verts, uvs, tris);
					//Front wall
					if(IsTransparent(x, y, z+1))
						BuildFace (brick, new Vector3(x,y,z+1), Vector3.up, Vector3.right, false, verts, uvs, tris);
				}
			}
		}
		visualMesh.vertices = verts.ToArray ();
		visualMesh.uv = uvs.ToArray ();
		visualMesh.triangles = tris.ToArray ();
		visualMesh.RecalculateBounds ();
		visualMesh.RecalculateNormals ();
		
		meshFilter.mesh = visualMesh;
		meshCollider.sharedMesh = visualMesh;
	}
	
	public virtual void BuildFace(byte brick, Vector3 corner, Vector3 up, Vector3 right, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
	{
		int index = verts.Count;
		
		verts.Add (corner);
		verts.Add (corner + up);
		verts.Add (corner + up + right);
		verts.Add (corner + right);
		
		Vector2 uvWidth = new Vector2(0.0625f, 0.0625f);	// To add a terrain image
		Vector2 uvCorner = new Vector2(0.00f, 0.9375f);
		
		uvCorner.x += (float)(brick - 1) * 0.0625f;
		
		uvs.Add(uvCorner);
		uvs.Add(new Vector2(uvCorner.x, uvCorner.y + uvWidth.y));
		uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y + uvWidth.y));
		uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y));
		
		if (reversed)
		{
			tris.Add(index + 0);
			tris.Add(index + 1);
			tris.Add(index + 2);
			tris.Add(index + 2);
			tris.Add(index + 3);
			tris.Add(index + 0);
		}
		else
		{
			tris.Add(index + 1);
			tris.Add(index + 0);
			tris.Add(index + 2);
			tris.Add(index + 3);
			tris.Add(index + 2);
			tris.Add(index + 0);
		}
		
	}
	
	public virtual bool IsTransparent(int x, int y, int z){
		if (y < 0)
			return false;
		byte brick = GetByte(x, y, z);
		switch (brick)
		{
		default:
		case 0:
			return true;
			
		case 1: return false;
		}
	}
	
	public virtual byte GetByte(int x, int y, int z){
		if (x < 0 || x >= width)
            return 0;
        else if (y < 0 || y >= height)
            return 0;
        else if (z < 0 || z >= width)
            return 0;
		else
            return map [x, y, z];
	}
}
