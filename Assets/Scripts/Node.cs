using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {


	Vector3 geoLocation;
	Vector2 position;
	List<Node> neighbors;
	List<Edge> roads;
	List<Tile> adjTiles;

	public Node(Vector3 geoLocation){
		this.geoLocation = geoLocation;
		neighbors = new List<Node> (3);
		roads = new List<Edge> (3);
		adjTiles = new List<Tile> (3);
	}

	public void addNeighbor(Node node){
		neighbors.Add(node);
	}

	public List<Node> getNeighbors(){
		return neighbors;

	}

	public void addTile(Tile tile){
		adjTiles.Add(tile);
	}

	public Vector3 getLoc(){
		return geoLocation;
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
