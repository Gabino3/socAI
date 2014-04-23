using UnityEngine;
using System.Collections;

public class Tile {

	public enum Resource {
		ore,sheep,wood,grain,brick,none
	};
	Resource resource;
	Vector3 geoLocation;
	Vector2 position;
	int chitValue;
	public GameObject robber {get;set;}

	public Tile(Resource r, Vector3 gl, Vector2 p, int chitValue)
	{
		resource = r;
		geoLocation = gl;
		position = p;
		this.chitValue = chitValue;
		robber = null;
	}

	public int GetChitValue()
	{
		return chitValue;
	}

	public Vector3 GetLoc()
	{
		return geoLocation;
	}

	public Resource GetResource()
	{
		return resource;
	}

}
