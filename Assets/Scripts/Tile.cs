using UnityEngine;
using System.Collections;

public class Tile {

	public enum Resource {

		ore,sheep,lumber,grain,brick,none
	};
	Resource resource;
	Vector3 geoLocation;
	Vector2 position;
	int chitValue;

	public Tile(Resource r, Vector3 gl, Vector2 p, int chitValue){
		resource = r;
		geoLocation = gl;
		position = p;
		this.chitValue = chitValue;
	}

	public Vector3 getLoc(){
		return geoLocation;
	}

	public Resource getResource(){
		return resource;
	}

}
