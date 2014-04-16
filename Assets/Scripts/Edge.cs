using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge  {

	Vector3 geoLocation;
	float angle;
	Vector2 position;
	List<Node> neighbors;
	Player owner;
	
	public Edge(Vector3 geoLocation, float angle, Player player)
	{
		this.geoLocation = geoLocation;
		this.angle = angle;
		neighbors = new List<Node> (2);
		owner = player;

	}
	
	public void addNeighbor(Node node){
		neighbors.Add(node);
	}
	
	public Vector3 getLoc(){
		return geoLocation;
	}

	public float getAngle(){
		return angle;
	}
	
	public Vector2 getPos(){
		return position;
	}
	
	public void setPos(Vector2 pos){
		position = pos;
	}
	
	public override string ToString(){
		return (neighbors.Count).ToString();
	}
}
