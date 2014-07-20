/*
-----------------------------------------------------------------------------------------
- Usage: 	Attach to an object and link the source images.
- Descrp: 	Generates a 2D spritesheet from a source directory of individual frames.
- Author: 	Andrew Lawson
- Date: 	10.25.13 
-----------------------------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using System.IO;
using System.Globalization;

public class SpritesheetGenerator : MonoBehaviour {
	private Texture2D[] textureArray;
	private Mesh objectMesh;
	private Rect[] uvArray, textureUV;
	private Vector2[] originalUV;
	public Texture2D spriteTexture;
	public string filePath, spriteName, destDirPath;
	public Material spriteMaterial;
	// Initialization code...
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
		// Pack individual textures into a texture atlas. Set upper bound of the image to 8192.
		float maxSqrt = Mathf.Ceil(Mathf.Sqrt(textureArray[0].width * textureArray[0].height * textureArray.Length));
		if (maxSqrt > 8192) {
			maxSqrt = 8192;
		}
		// Create sprite sheet and corresponding UV mappings.
		Texture2D textureAtlas = new Texture2D((int)maxSqrt, (int)maxSqrt);
		textureUV = textureAtlas.PackTextures(textureArray, 0, (int)maxSqrt);
		// Write PNG version of spritesheet to file
		System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/" + destDirPath);
		byte[] pngBytes = textureAtlas.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/Resources/" + destDirPath + spriteName + "_sheet" + ".png", pngBytes);
		// Output uv map to text file
		createUVMap();
	}
	// Looping code...
	void Update() {
		// empty
	}
	void createUVMap() { 
		// Intialize output writer to file with the same name as the sprites.
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
		// Write the UV mappings to the text file.
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
