// author: Andrew Lawson
// date: 10.25.13 

// !USAGE!
// Animates a spritesheet from a given image and uv map from the user. User can control time in-between
// sprite frames.

using UnityEngine;
using System.Collections;
using System.IO;
using System.Globalization;

public class SpritesheetAnimator : MonoBehaviour {
	
	// Private Instance Variables
	
	private Mesh
		objectMesh;
	private Rect[]
		textureUV;
	private Vector2[]
		originalUV;
	private int
		counter = 0;
	
	// Public Instance Variables
	
	public Material
		material;
	public TextAsset
		uvMap;
	public float
		framesPerSecond;
	public bool
		pause = false;
	
	void Start() {
		
		// Intitializations
		
		objectMesh = GetComponent<MeshFilter>().mesh;
		originalUV = new Vector2[objectMesh.uv.Length];
		objectMesh.uv.CopyTo(originalUV, 0);
		
		// Read uv maps from text file
		
		textureUV = readUVMap();
		gameObject.renderer.material = material;
		
		StartCoroutine("CoUpdate");
	}
	
	// Coroutine that runs the frame animation.
	
	IEnumerator FrameAnimation() {
		
		// Reset the UV map
		
		Vector2[] transformUV = new Vector2[originalUV.Length];
		
		// For each texture in the atlas, transform the mesh UV map to the map of the texture.

		originalUV.CopyTo(transformUV, 0);
			
		for (int j = 0; j < objectMesh.uv.Length; j++) {
				
			transformUV[j].x = (transformUV[j].x * textureUV[counter].width) + textureUV[counter].x;
			transformUV[j].y = (transformUV[j].y * textureUV[counter].height) + textureUV[counter].y;
		}
		
		// Increase counter and set the object's mesh to the transformation.
		
		
		if (!pause) {
			
			counter++;
		}
		
		
		objectMesh.uv = transformUV;
		
		yield return null;
	}
	
	// Replaces Update() to allow for proper co-routine execution at runtime. Starts the animation co-routine.
	
	IEnumerator CoUpdate() {
		
		while (true) {
		
			if (counter >= textureUV.Length) {
				
				counter = 0;
			}
			
			StartCoroutine("FrameAnimation");
			
			// Allows control of time in-between frame animations.
			
			yield return new WaitForSeconds((float)(1 / framesPerSecond));
		}
	}
	
	Rect[] readUVMap() {
		
		// Intializes a reader and retrieves length of UV entries
		
		StringReader reader = new StringReader(uvMap.text);
		int length = int.Parse(reader.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
		
		Rect[] uvArray = new Rect[length];
		
		// Stores entries from text file into array and returns it
		
		for (int i = 0; i < length; i++) {
			
			float x = float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
			float y = float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
			float width = float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
			float height = float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
			
			uvArray[i] = new Rect(x, y, width, height);
		}
		
		return uvArray;
	}
} 