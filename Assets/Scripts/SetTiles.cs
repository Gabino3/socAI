using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SetTiles : MonoBehaviour {


	GameObject city;
	GameObject settlement;
	GameObject road;

	static GameState gamestate;


	// Use this for initialization
	void Start () {
		int boardWidth = 7;
		int boardHeight = 7;

		settlements = new Dictionary<Transform, GameObject> (54);
		roadPlaceholders = new Dictionary<Transform, GameObject> (54);
		settlementPlaceholders = new Dictionary<Transform, GameObject> (54);

		//List of tile positions
		List<int> tilesToDraw = new List<int>{
			    (21),(31),(41),
			  (22),(32),(42),(52),
			(13),(23),(33),(43),(53),
			  (24),(34),(44),(54),
			    (25),(35),(45)
		};

		float hexSize = 1.3f;

		//List of tiles (19 total; 3 ore and sheep; 4 grain, sheep and lumber; 1 desert)
		var tileDeck = new List<string>{
			"tile_grain", "tile_sheep", "tile_ore","tile_lumber","tile_lumber", "tile_brick", "tile_lumber", "tile_lumber",
			"tile_ore","tile_desert","tile_brick","tile_grain","tile_grain","tile_grain","tile_sheep","tile_sheep","tile_sheep",
			"tile_brick",
			"tile_ore"

		};

		Dictionary<char, int> chitConversions = new Dictionary<char, int> (){

			{'G',9},{'D',3},{'A',5},{'I',11},{'O',5},{'K',8},{'J',4},{'C',6},{'M',9},{'Q',3},{'E',8},{'F',10},{'H',12},{'P',6},{'L',10},{'B',2},{'N',4},{'R',11}

		};

		var chitDeck = new List<char>{
			'G','D','A','I','O','K','J','C','M','Q','E','F','H','P','L','B','N','R'
		};

		List<Vector3> tilePos = new List<Vector3>(19);
		List<Vector3> tileCorners = new List<Vector3>(114);

		var tiles = new List<Tile>(19);

		//Place tiles
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
					case "tile_lumber"	: tiles.Add(new Tile(Tile.Resource.lumber, tilePos[counter], new Vector2(x,y),chitConversions[chitDeck[chitCounter]])); break;
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

		Instantiate(Resources.Load("PlayerOrder"), new Vector3(-3, 12, 0), Quaternion.identity);

		tileCorners = MergeDuplicates (tileCorners);

		List<Node> vertices = VecToNodes (tileCorners, tiles, hexSize);
		List<Edge> roads = FindRoadPos ( vertices, out vertices);
		print (roads.Count);
		//List<>
		 
		for (var i = 0; i<roads.Count; i++) {
			GameObject s = Instantiate(Resources.Load("r_placeholder"), roads[i].getLoc(), Quaternion.identity) as GameObject;
			s.transform.eulerAngles = new Vector3(0f,0f,(roads[i]).getAngle());
			s.renderer.enabled = false;
			roadPlaceholders.Add(s.transform, s);

		}
		for (var i = 0; i<vertices.Count; i++) {
			GameObject t = Instantiate(Resources.Load("v_placeholder"), vertices[i].getLoc(), Quaternion.identity) as GameObject;
			t.renderer.enabled = false;
			settlementPlaceholders.Add(t.transform, t);
			
		}
		Instantiate(Resources.Load("backing"), new Vector3(-3.598929f,1f,1f), Quaternion.identity);

		city = Instantiate(Resources.Load("city"), new Vector3(-5f,2f,-1f), Quaternion.identity) as GameObject;
		city.renderer.material.color = Color.red;
		city.transform.localScale += city.transform.localScale;
		road = Instantiate(Resources.Load("road"), new Vector3(-2.5f,0f,-1f), Quaternion.identity) as GameObject;
		road.renderer.material.color = Color.red;
		road.transform.localScale += road.transform.localScale;
		settlement = Instantiate(Resources.Load("settlement"), new Vector3(-2.5f,2f,-1f), Quaternion.identity) as GameObject;
		settlement.renderer.material.color = Color.red;
		settlement.transform.localScale += settlement.transform.localScale;

		thingToBuild = null;

		//Create gamestate handler
		gamestate = new GameState(4, vertices, roads);

		//Instantiate player hand (bottom right)
		Instantiate (Resources.Load ("card_brick"), new Vector3 (12.5844f, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_ore"), new Vector3 (14.1115f, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_wood"), new Vector3 (15.6386f, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_grain"), new Vector3 (17.1657f, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_sheep"), new Vector3 (18.6928f, 0.1522f, 1), Quaternion.identity);
		DisplayPlayerCardCount ();
		
		//Instantiate dice roll indicator
		Instantiate (Resources.Load ("dice"), new Vector3 (14.24463f, 7.134943f, 1), Quaternion.identity);
	}

	static void DisplayPlayerCardCount()
	{
		GameObject brickCount = Instantiate (Resources.Load ("text"), new Vector3 (12.4493f, 1.908892f, -4f), Quaternion.identity) as GameObject;
		GameObject oreCount = Instantiate (Resources.Load ("text"), new Vector3 (13.9764f, 1.908892f, -4f), Quaternion.identity) as GameObject;
		GameObject woodCount = Instantiate (Resources.Load ("text"), new Vector3 (15.5035f, 1.908892f, -4f), Quaternion.identity) as GameObject;
		GameObject grainCount = Instantiate (Resources.Load ("text"), new Vector3 (17.0306f, 1.908892f, -4f), Quaternion.identity) as GameObject;
		GameObject sheepCount = Instantiate (Resources.Load ("text"), new Vector3 (18.5577f, 1.908892f, -4f), Quaternion.identity) as GameObject;
		brickCount.GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().brick;
		oreCount.GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().ore;
		woodCount.GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().wood;
		grainCount.GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().grain;
		sheepCount.GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().sheep;
	}

	static Vector3 GetWorldCoordinates(int x, int y, float z)
	{
		float YSpacing = 2.3f;
		float XSpacing = 2.0f;
		var yOffset = x % 2 == 1 ? 0 : -YSpacing / 2;
		return new Vector3(x * XSpacing, y * YSpacing + yOffset, -z);
	}

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

	static List<Vector3> MergeDuplicates(List<Vector3> tileCorners)
	{
		int threshold = 1;
		List<Vector3> corners = new List<Vector3> ();
		bool isDuplicate = false;
		for (var i = 0; i<tileCorners.Count; i++) {
			for (var j = 0; j<corners.Count;j++){
				double d = Math.Sqrt( Math.Pow((tileCorners[i]).x - (corners[j]).x, 2) + Math.Pow((tileCorners[i]).y - (corners[j]).y, 2));
				if(d<threshold){
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

	static List<string> Shuffle(List<string> deck, int times)
	{
		//Shuffle tiles 8 times
		System.Random rnd = new System.Random();
		for (var q = 0; q<times;q++){
			for (var i =deck.Count-1; i>=0 ;i-- )
			{
				var j = rnd.Next(0, i+1);
				var tempTile = deck[j];
				deck[j] = deck[i];
				deck[i] = tempTile;
			}
		}
		return deck;
	}


	static List<Node> VecToNodes(List<Vector3> corners, List<Tile> tiles, float size){
		List<Node> vertices = new List<Node> (54);
		for (var i = 0; i<corners.Count; i++) {
			vertices.Add (new Node (corners [i]));
		}

		vertices = FindNeighbors (vertices, size);

		vertices = FindResources (vertices, tiles, size);

		vertices = FindLogicalPos (vertices);

		return vertices;
	}

	static List<Node> FindNeighbors(List<Node> vertices, float size)
	{
		float threshold = size + 0.2f;

		for (var i = 0; i<vertices.Count; i++) {
			for (var j = 0; j<vertices.Count;j++){
				double d = Math.Sqrt( Math.Pow((vertices[i]).getLoc().x - (vertices[j]).getLoc().x, 2) + Math.Pow((vertices[i]).getLoc().y - (vertices[j]).getLoc().y, 2));
				if(d<threshold && i!=j){
					(vertices[i]).addNeighbor(vertices[j]);
				}	
			}
		}



		return vertices;
	}

	static List<Node> FindResources(List<Node> vertices, List<Tile> tiles, float size)
	{
		float threshold = size + 0.2f;
		
		for (var i = 0; i<vertices.Count; i++) {
			for (var j = 0; j<tiles.Count;j++){
				double d = Math.Sqrt( Math.Pow((vertices[i]).getLoc().x - (tiles[j]).getLoc().x, 2) + Math.Pow((vertices[i]).getLoc().y - (tiles[j]).getLoc().y, 2));
				if(d<threshold && i!=j){
					(vertices[i]).addTile(tiles[j]);
				}	
			}
		}

		return vertices;
	}

	static List<Node> FindLogicalPos(List<Node> vertices)
	{
		float threshold = 0.2f;
		
		for (var i = 0; i<vertices.Count; i++) {

		}
		
		return vertices;
	}

	static List<Edge> FindRoadPos(List<Node> verticesIn, out List<Node> vertices)
	{
		vertices = verticesIn;
		List<Edge> roads = new List<Edge> ();
		List<Node> visited = new List<Node> (54);
		for (var i = 0; i<vertices.Count; i++) {
			visited.Add(vertices[i]);
			List<Node> neighbors = (vertices[i]).getNeighbors();
			for (var j = 0; j<neighbors.Count;j++) {
				if(!visited.Contains(neighbors[j])){

					float x1 = (vertices[i]).getLoc().x;
					float x2 = (neighbors[j]).getLoc().x;
					float y1 = (vertices[i]).getLoc().y;
					float y2 = (neighbors[j]).getLoc().y;

					float x_mid = (x1 + x2)/2;
					float y_mid = (y1 + y2)/2;
					float angle = (float)Math.Atan((y1 - y2)/(x1 - x2))*57.2957795f; //converting from radians to degrees
					print (angle);
					roads.Add(new Edge(new Vector3(x_mid, y_mid, 0), angle, null, vertices[i], neighbors[j], false));
					vertices[i].addRoad(roads[roads.Count-1]);
					vertices[vertices.IndexOf(neighbors[j])].addRoad(roads[roads.Count-1]);

				}
			}

		}


		return roads;

	}



	Transform thingToBuild;
	Dictionary<Transform, GameObject> settlementPlaceholders;
	Dictionary<Transform, GameObject> roadPlaceholders;
	Dictionary<Transform, GameObject> settlements;


	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown(0)){ // if left button pressed...
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			print ("mouse press");
			if (Physics.Raycast(ray, out hit)){
				if(hit.transform == city.transform || hit.transform == settlement.transform || hit.transform == road.transform){
					thingToBuild = hit.transform;
					print ("on a building");
				}
				else if(thingToBuild!=null ){
					if (thingToBuild == road.transform && roadPlaceholders.ContainsKey(hit.transform)) {
						GameObject s = Instantiate(Resources.Load("road"), hit.transform.position , Quaternion.identity) as GameObject;
						s.transform.eulerAngles = hit.transform.eulerAngles;
						s.renderer.material.color = Color.red;
						Destroy(roadPlaceholders[hit.transform]);
						roadPlaceholders.Remove(hit.transform);
						thingToBuild = null;
						print ("road built!");
						//TODO add color based on player and save the road in an list/array/dict
					}
					else if(thingToBuild == settlement.transform && settlementPlaceholders.ContainsKey(hit.transform)){
						GameObject s = Instantiate(Resources.Load("settlement"), hit.transform.position , Quaternion.identity) as GameObject;
						s.renderer.material.color = Color.red;
						settlements.Add(s.transform, s);
						Destroy(settlementPlaceholders[hit.transform]);
						settlementPlaceholders.Remove(hit.transform);
						thingToBuild = null;
						print ("settlement built!");
					}
					else if (thingToBuild == city.transform && settlements.ContainsKey(hit.transform)) {
						GameObject s = Instantiate(Resources.Load("city"), hit.transform.position , Quaternion.identity) as GameObject;
						s.renderer.material.color = Color.red;
						Destroy(settlements[hit.transform]);
						settlements.Remove(hit.transform);
						thingToBuild = null;
						print ("city built!");
						//TODO add color based on player and save the city in an list/array/dict
					}
				}
			}
		}
	}
}
