using UnityEngine;
using System.Collections;

public class Tile {

	public enum Resource {

		ore,sheep,lumber,grain,brick,none
	};
	public Resource resource;
	public Vector3 geoLocation;
	public Vector2 position;

	public Tile(Resource r, Vector3 gl, Vector2 p){
		resource = r;
		geoLocation = gl;
		position = p;
	}

	public Vector3 getLoc(){
		return geoLocation;
	}

	public Resource getResource(){
		return resource;
	}

}
