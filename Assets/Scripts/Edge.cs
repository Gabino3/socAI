﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge  {

	Vector3 geoLocation;
	float angle;
	Vector2 position;
	List<Node> neighbors;
	public Player owner;
	public bool occupied;
	public GameObject visual;
	
	public Edge(Vector3 geoLocation, float angle, Player player, Node n1, Node n2, bool occupied)
	{
		this.geoLocation = geoLocation;
		this.angle = angle;
		neighbors = new List<Node> {n1, n2};
		owner = player;
		this.occupied = occupied;
		visual = null;
	}
	
	public void addNeighbor(Node node){
		neighbors.Add(node);
	}

	public List<Node> getNeighbors(){
		return neighbors;
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
