// author: Andrew Lawson
// date: 10.25.13 

// !USAGE!
// Generates a spritesheet and uv map from a set of given images.

using UnityEngine;
using System.Collections;
using System.IO;
using System.Globalization;

public class SpritesheetGenerator : MonoBehaviour {
	
	// Private Instance Variables
	
	private Texture2D[]
		textureArray;
	private Mesh
		objectMesh;
	private Rect[]
		uvArray,
		textureUV;
	private Vector2[]
		originalUV;
	
	// Public Instance Variables
	
	public Texture2D
		spriteTexture;
	public string
		filePath;
	public string
		spriteName,
		destDirPath;
	public Material
		spriteMaterial;
	
	void Start() {
		
		// Load all source images from 'Resources/"filePath"'
		
		Object[] resourceArray = Resources.LoadAll(filePath);		
		textureArray = new Texture2D[resourceArray.Length];
		
		for (int i = 0; i < resourceArray.Length; i++) {
			
			textureArray[i] = (Texture2D) resourceArray[i];
		}
		
		resourceArray = null;
		
		// Instantiate a variable holding the UV map of the mesh.
		
		objectMesh = GetComponent<MeshFilter>().mesh;
		originalUV = new Vector2[objectMesh.uv.Length];
		objectMesh.uv.CopyTo(originalUV, 0);
		
		// Pack individual textures into a texture atlas.
		
		float maxSqrt = Mathf.Ceil(Mathf.Sqrt(textureArray[0].width * textureArray[0].height * textureArray.Length));
			
		if (maxSqrt > 8192) {
			
			maxSqrt = 8192;
		}
		
		// Create sprite sheet
			
		Texture2D textureAtlas = new Texture2D((int)maxSqrt,
			(int)maxSqrt);
		
		textureUV = textureAtlas.PackTextures(textureArray, 0, 
			(int)maxSqrt);

		// Write PNG spritesheet to file
		
		System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/" + destDirPath);
		
		byte[] pngBytes = textureAtlas.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/Resources/" + destDirPath + spriteName + "_sheet" + ".png", pngBytes);
		
		//spriteMaterial = new Material(Shader.Find ("Transparent/Diffuse"));
		//spriteTexture = (Texture2D) Resources.Load(destDirPath + spriteName + "_sheet");
		//spriteMaterial.SetTexture("_MainTex", spriteTexture);
		//renderer.material = spriteMaterial;
		
		// Output uv map to text file
		
		createUVMap();
	}

	void Update() {
		
		// nothing
	}
	
	void createUVMap() { 
		
		// Intialize output writer
		
		StreamWriter writer; 
		FileInfo t = new FileInfo(Application.dataPath + "/Resources/" + destDirPath + spriteName + ".txt"); 
		
		if (!t.Exists) {
		
		 	writer = t.CreateText(); 
		}
	
		else {
		
			t.Delete(); 
			writer = t.CreateText(); 
		} 
		
		writer.Write(textureUV.Length);
		writer.WriteLine();
		
		// Write uv map from textureUV to text file.
		
		for (int i = 0; i < textureUV.Length; i++) {
			
			writer.Write(textureUV[i].x);
			writer.WriteLine();
			writer.Write(textureUV[i].y);
			writer.WriteLine();
			writer.Write(textureUV[i].width);
			writer.WriteLine();
			writer.Write(textureUV[i].height);
			writer.WriteLine();
		} 
		
		writer.Close(); 
	} 
} 