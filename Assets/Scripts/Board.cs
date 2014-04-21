using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Board : MonoBehaviour
{
	//Debug variables
	bool showHitboxes = false;

	//Board elements
	public List<Edge> roads;
	public List<Tile> tiles;
	public List<Node> vertices;
	public Dictionary<Transform, GameObject> settlements;
	public Dictionary<Transform, GameObject> roadHitboxes;
	public Dictionary<Transform, GameObject> settlementHitboxes;

	//Board metrics
	readonly int boardWidth = 7;
	readonly int boardHeight = 7;
	readonly float hexSize = 1.3f;

	//Board layout
	readonly List<int> tilesToDraw = new List<int>{
			 (21),(31),(41),
		   (22),(32),(42),(52),
		(13),(23),(33),(43),(53),
		   (24),(34),(44),(54),
		     (25),(35),(45)
	};
	readonly List<string> tileDeck = new List<string>{
		"tile_grain", "tile_sheep", "tile_ore","tile_lumber","tile_lumber", "tile_brick", "tile_lumber", "tile_lumber",
		"tile_ore","tile_desert","tile_brick","tile_grain","tile_grain","tile_grain","tile_sheep","tile_sheep","tile_sheep",
		"tile_brick",
		"tile_ore"
	};
	readonly List<char> chitDeck = new List<char>{
		'G','D','A','I','O','K','J','C','M','Q','E','F','H','P','L','B','N','R'
	};

	//Chit values
	readonly Dictionary<char, int> chitConversions = new Dictionary<char, int> (){
		{'G',9},{'D',3},{'A',5},{'I',11},{'O',5},{'K',8},{'J',4},{'C',6},{'M',9},{'Q',3},{'E',8},{'F',10},{'H',12},{'P',6},
		{'L',10},{'B',2},{'N',4},{'R',11}
	};

	/*
	 * Instantiates preset game board.
	 */
	public Board ()
	{
		settlements = new Dictionary<Transform, GameObject> (54);
		roadHitboxes = new Dictionary<Transform, GameObject> (54);
		settlementHitboxes = new Dictionary<Transform, GameObject> (54);

		PlaceTiles ();
		PlaceHitboxes ();
	}

	/*
	 * Methods for determining valid object placement.
	 */
	public bool CanBuildSettlementHere(Transform hitbox, Player player)
	{
		for (int i = 0;i<vertices.Count;i++){
			if (vertices[i].visual.transform == hitbox && vertices[i].owner == null)
			{
				foreach (Node neighbor in vertices[i].getNeighbors())
				 {
					if (neighbor.owner != null) {
						GameEngine.print ("Cannot build a settlement at this location! Another Settlement/City is too close by.");
						return false;
					}
				}
				foreach (Edge neighbor in vertices[i].getRoads())
				{
					if (neighbor.owner == player) {
						return true;
					}
				}
				
			}
		}
		GameEngine.print ("Cannot build a settlement at this location! No nearby Road.");
		return false;
	}
	public bool CanBuildCityHere(Transform hitbox, Player player)
	{
		//check to see if a settlement that this player owns is already there
		for (int i = 0;i<vertices.Count;i++){
			if (vertices[i].visual.transform == hitbox && vertices[i].occupied == Node.Occupation.settlement 
			    && vertices[i].owner == player){
				return true;
			}
		}
		GameEngine.print ("Cannot build a city at this location! No settlement.");
		return false;
	}
	public bool CanBuildRoadHere(Transform hitbox, Player player)
	{
		for (int i = 0;i<roads.Count;i++){
			if (roads[i].visual.transform == hitbox && roads[i].owner == null)
			{
				foreach (Node neighbor in roads[i].getNeighbors())
				{
					if (neighbor.owner == player) {
						return true;
					}
					foreach (Edge nextRoad in neighbor.getRoads())
					{
						if (nextRoad.owner == player) {
							return true;
						}
					}
				}
			    

			}
		}
		GameEngine.print ("Cannot build a road at this location! No nearby road or Settlement/City.");
		return false;
	}

	/*
	 * Determines logical positions of each vertex.
	 */
	static List<Node> FindLogicalPos(List<Node> vertices)
	{
		float threshold = 0.2f;
		for (var i = 0; i<vertices.Count; i++) {
			// TODO
		}
		return vertices;
	}

	/*
	 * Finds and assigns neighbors of each vertex.
	 */
	static List<Node> FindNeighboringVertices(List<Node> vertices, float size)
	{
		float threshold = size + 0.2f;
		
		for (var i = 0; i<vertices.Count; i++) {
			for (var j = 0; j<vertices.Count;j++) {
				double d = Math.Sqrt( Math.Pow((vertices[i]).getLoc().x - (vertices[j]).getLoc().x, 2) + Math.Pow((vertices[i]).getLoc().y - (vertices[j]).getLoc().y, 2));
				if (d<threshold && i!=j) {
					vertices[i].addNeighbor(vertices[j]);
				}	
			}
		}

		return vertices;
	}

	/*
	 * Finds all neighboring tiles to each vertex.
	 */
	static List<Node> FindNeighboringTiles(List<Node> vertices, List<Tile> tiles, float size)
	{
		float threshold = size + 0.2f;
		
		for (var i = 0; i<vertices.Count; i++) {
			for (var j = 0; j<tiles.Count;j++){
				double d = Math.Sqrt( Math.Pow((vertices[i]).getLoc().x - (tiles[j]).GetLoc().x, 2) + Math.Pow((vertices[i]).getLoc().y - (tiles[j]).GetLoc().y, 2));
				if(d<threshold && i!=j){
					vertices[i].addTile(tiles[j]);
				}	
			}
		}
		
		return vertices;
	}

	/*
	 * Places and orients roads
	 */
	static List<Edge> FindRoadPos(List<Node> verticesIn, out List<Node> vertices)
	{
		vertices = verticesIn;
		List<Edge> roads = new List<Edge> ();
		List<Node> visited = new List<Node> (54);
		for (var i = 0; i<vertices.Count; i++) {
			visited.Add(vertices[i]);
			List<Node> neighbors = (vertices[i]).getNeighbors();
			for (var j = 0; j<neighbors.Count;j++) {
				if (!visited.Contains(neighbors[j])) {
					float x1 = (vertices[i]).getLoc().x;
					float x2 = (neighbors[j]).getLoc().x;
					float y1 = (vertices[i]).getLoc().y;
					float y2 = (neighbors[j]).getLoc().y;
					
					float x_mid = (x1 + x2)/2;
					float y_mid = (y1 + y2)/2;
					float angle = (float) Math.Atan((y1 - y2)/(x1 - x2)) * 57.2957795f; //converting from radians to degrees
					
					if(angle > 350 || (angle < 5 && angle > -5)){
						angle = 0;
					} 
					else if (angle > 55 && angle < 65){
						angle = 60;
					}
					else if (angle > -65 && angle < -55){
						angle = 300;
					}

					roads.Add(new Edge(new Vector3(x_mid, y_mid, 0), angle, null, vertices[i], neighbors[j], false));
					vertices[i].addRoad(roads[roads.Count-1]);
					vertices[vertices.IndexOf(neighbors[j])].addRoad(roads[roads.Count-1]);
				}
			}	
		}

		return roads;
	}

	/*
	 * Returns 6 corners of given hex.
	 */
	static List<Vector3> GetHexCorners(float x, float y, float z, float size)
	{
		List<Vector3> corners = new List<Vector3>(6);
		for (var i = 0; i < 6; i++){
			float angle = (float)(2 * Math.PI / 6 * i);
			float x_i = (float)(x + size * Math.Cos(angle));
			float y_i = (float)(y + size * Math.Sin(angle));
			corners.Add(new Vector3(x_i, y_i, 0));
		}
		return corners;
	}

	/*
	 * Gives even/odd hex offset coordinates.
	 */
	private static Vector3 GetWorldCoordinates(int x, int y, float z)
	{
		float YSpacing = 2.3f;
		float XSpacing = 2.0f;
		var yOffset = x % 2 == 1 ? 0 : -YSpacing / 2;
		return new Vector3(x * XSpacing, y * YSpacing + yOffset, -z);
	}

	/*
	 * Removes duplicate hex corners.
	 */
	static List<Vector3> MergeDuplicates(List<Vector3> tileCorners)
	{
		int threshold = 1;
		List<Vector3> corners = new List<Vector3> ();
		bool isDuplicate = false;
		for (var i = 0; i<tileCorners.Count; i++) {
			for (var j = 0; j<corners.Count;j++) {
				double d = Math.Sqrt( Math.Pow((tileCorners[i]).x - (corners[j]).x, 2) + Math.Pow((tileCorners[i]).y - (corners[j]).y, 2));
				if(d<threshold) {
					isDuplicate = true;
					break;
				}
			}
			if(!isDuplicate){
				corners.Add(tileCorners[i]);
			}
			isDuplicate = false;
			
		}
		return corners;
	}

	/*
	 * Places clickable hitboxes for roads and settlements/cities.
	 */
	private void PlaceHitboxes()
	{
		for (var i = 0; i < roads.Count; i++) {
			GameObject s = Instantiate(Resources.Load("r_placeholder"), roads[i].getLoc(), Quaternion.identity) as GameObject;
			s.transform.eulerAngles = new Vector3(0f,0f,(roads[i]).getAngle());
			s.renderer.enabled = showHitboxes;
			roads[i].visual = s;
			roadHitboxes.Add(s.transform, s);
		}
		for (var i = 0; i < vertices.Count; i++) {
			GameObject t = Instantiate(Resources.Load("v_placeholder"), vertices[i].getLoc(), Quaternion.identity) as GameObject;
			t.renderer.enabled = showHitboxes;
			vertices[i].visual = t;
			settlementHitboxes.Add(t.transform, t);	
		}
	}

	/*
	 * Places an AI's settlement.
	 */
	public void PlaceSettlement(Node node, Player player)
	{
		//TODO

	}

	/*
	 * Places a human player's settlement.
	 */
	public void PlaceSettlement(Transform hitbox, Player player)
	{
		GameObject s = Instantiate (Resources.Load ("settlement"), hitbox.position, Quaternion.identity) as GameObject;
		s.renderer.material.color = player.color;
		settlements.Add (s.transform, s);
		Destroy (settlementHitboxes [hitbox]);
		settlementHitboxes.Remove (hitbox);
		for (int i = 0; i<vertices.Count; i++) {
				if (vertices [i].visual.transform == hitbox) {
					vertices [i].visual = s;
					vertices [i].owner = player;
					vertices[i].occupied = Node.Occupation.settlement;
					player.AddStructure(vertices[i]);
				}
		}
		
	}

	/*
	 * Places a human player's road.
	 */
	public void PlaceRoad(Transform hitbox, Player player)
	{
		GameObject s = Instantiate(Resources.Load("road"), hitbox.position , Quaternion.identity) as GameObject;
		s.transform.eulerAngles = hitbox.eulerAngles;
		s.renderer.material.color = player.color;
		Destroy(roadHitboxes[hitbox]);
		roadHitboxes.Remove(hitbox);	
		for (int i = 0;i<roads.Count;i++){
			if (roads[i].visual.transform == hitbox){
				roads[i].visual = s;
				roads[i].owner = player;
				player.AddRoad(roads[i]);
			}
		}
	}


	/*
	 * Places a human player's city.
	 */
	public void PlaceCity(Transform hitbox, Player player)
	{

		GameObject s = Instantiate (Resources.Load ("city"), hitbox.position, Quaternion.identity) as GameObject;
		s.renderer.material.color = player.color;
		Destroy (settlements [hitbox]);
		settlements.Remove (hitbox);
		for (int i = 0; i < vertices.Count; i++){
			if (vertices[i].visual.transform == hitbox){
				vertices [i].visual = s;
				vertices [i].owner = player;
				vertices[i].occupied = Node.Occupation.city;
			}
		}

	}


	/*
	 * Assigns and displays tiles.
	 */
	private void PlaceTiles()
	{
		List<Vector3> tilePos = new List<Vector3>(19);
		List<Vector3> tileCorners = new List<Vector3>(114);
		List<Tile> tiles = new List<Tile>(19);
		int counter = 0;
		int chitCounter = 0;

		for (var x = 0; x < boardWidth; x++)
		{
			for (var y = 0; y < boardHeight; y++)
			{
				if(tilesToDraw.Contains(y*10+x)){
					tilePos.Add(GetWorldCoordinates(x,y,0));
					Instantiate(Resources.Load(tileDeck[counter]), tilePos[counter], Quaternion.identity);
					switch(tileDeck[counter]){
					case "tile_lumber"	: tiles.Add(new Tile(Tile.Resource.wood, tilePos[counter], new Vector2(x,y),chitConversions[chitDeck[chitCounter]])); break;
					case "tile_ore"		: tiles.Add(new Tile(Tile.Resource.ore, tilePos[counter], new Vector2(x,y),chitConversions[chitDeck[chitCounter]])); break;
					case "tile_sheep"	: tiles.Add(new Tile(Tile.Resource.sheep, tilePos[counter], new Vector2(x,y),chitConversions[chitDeck[chitCounter]])); break;
					case "tile_grain"	: tiles.Add(new Tile(Tile.Resource.grain, tilePos[counter], new Vector2(x,y),chitConversions[chitDeck[chitCounter]])); break;
					case "tile_brick"	: tiles.Add(new Tile(Tile.Resource.brick, tilePos[counter], new Vector2(x,y),chitConversions[chitDeck[chitCounter]])); break;
					default				: tiles.Add(new Tile(Tile.Resource.none, tilePos[counter], new Vector2(x,y),chitConversions[chitDeck[chitCounter]])); break;
					}
					tileCorners.AddRange(GetHexCorners((tilePos[counter]).x,(tilePos[counter]).y, 0.0f, hexSize ));
					if (tileDeck[counter] != "tile_desert"){
						Instantiate(Resources.Load("catanChits_"+chitDeck[chitCounter]), new Vector3((tilePos[counter]).x, (tilePos[counter]).y, -1), Quaternion.identity);
						chitCounter++;
					}
					counter++;
				}
			}
		}

		tileCorners = MergeDuplicates (tileCorners);

		this.vertices = VecToNodes (tileCorners, tiles, hexSize);
		this.roads = FindRoadPos (vertices, out vertices);
		this.tiles = tiles;
	}

	/*
	 * Compiles vector information into nodes
	 */
	static List<Node> VecToNodes(List<Vector3> corners, List<Tile> tiles, float size){
		List<Node> vertices = new List<Node> (54);
		for (var i = 0; i<corners.Count; i++) {
			vertices.Add (new Node (corners [i]));
		}
		
		vertices = FindNeighboringVertices (vertices, size);
		vertices = FindNeighboringTiles (vertices, tiles, size);
		vertices = FindLogicalPos (vertices);
		
		return vertices;
	}

}
