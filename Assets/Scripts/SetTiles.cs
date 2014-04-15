using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SetTiles : MonoBehaviour {


	// Use this for initialization
	void Start () {
		int boardWidth = 7;
		int boardHeight = 7;
		//List of tile positions
		List<int> tilesToDraw = new List<int>{
			    (21),(31),(41),
			  (22),(32),(42),(52),
			(13),(23),(33),(43),(53),
			  (24),(34),(44),(54),
			    (25),(35),(45)
		};

		float hexSize = 1.3f;

		//List of tiles (19 total; 3 ore and wool; 4 grain, wool and lumber; 1 desert)
		var tileDeck = new List<string>{
			"tile_grain", "tile_wool", "tile_ore","tile_lumber","tile_lumber", "tile_brick", "tile_lumber", "tile_lumber",
			"tile_ore","tile_desert","tile_brick","tile_grain","tile_grain","tile_grain","tile_wool","tile_wool","tile_wool",
			"tile_brick",
			"tile_ore"

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
					case "tile_lumber"	: tiles.Add(new Tile(Tile.Resource.lumber, tilePos[counter], new Vector2(x,y))); break;
					case "tile_ore"		: tiles.Add(new Tile(Tile.Resource.ore, tilePos[counter], new Vector2(x,y))); break;
					case "tile_wool"	: tiles.Add(new Tile(Tile.Resource.wool, tilePos[counter], new Vector2(x,y))); break;
					case "tile_grain"	: tiles.Add(new Tile(Tile.Resource.grain, tilePos[counter], new Vector2(x,y))); break;
					case "tile_brick"	: tiles.Add(new Tile(Tile.Resource.brick, tilePos[counter], new Vector2(x,y))); break;
					default				: tiles.Add(new Tile(Tile.Resource.none, tilePos[counter], new Vector2(x,y))); break;
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

		List<Node> vertecies = VecToNodes (tileCorners, tiles, hexSize);
		List<Edge> roads = FindRoadPos (vertecies);
		print (roads.Count);
		//List<>
		 
		for (var i = 0; i<roads.Count; i++) {
			GameObject s = Instantiate(Resources.Load("road"), roads[i].getLoc(), Quaternion.identity) as GameObject;
			s.renderer.material.color = Color.magenta;
			s.transform.eulerAngles = new Vector3(0f,0f,(roads[i]).getAngle());

		}
		for (var i = 0; i<vertecies.Count; i++) {
			GameObject t = Instantiate(Resources.Load("city"), vertecies[i].getLoc(), Quaternion.identity) as GameObject;
			t.renderer.material.color = Color.red;

			
		}
		Instantiate(Resources.Load("backing"), new Vector3(-3.598929f,1f,1f), Quaternion.identity);

		GameObject city = Instantiate(Resources.Load("city"), new Vector3(-5f,2f,-1f), Quaternion.identity) as GameObject;
		city.renderer.material.color = Color.red;
		city.transform.localScale += city.transform.localScale;
		GameObject road = Instantiate(Resources.Load("road"), new Vector3(-2.5f,0f,-1f), Quaternion.identity) as GameObject;
		road.renderer.material.color = Color.red;
		road.transform.localScale += road.transform.localScale;
		GameObject settlement = Instantiate(Resources.Load("settlement"), new Vector3(-2.5f,2f,-1f), Quaternion.identity) as GameObject;
		settlement.renderer.material.color = Color.red;
		settlement.transform.localScale += settlement.transform.localScale;

		GameObject text = Instantiate(Resources.Load("text"), new Vector3(-2.5f,-2f,-1f), Quaternion.identity) as GameObject;
		text.guiText.text = "01110011";
		//text.transform.localScale += settlement.transform.localScale;

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
		List<Node> vertecies = new List<Node> (54);
		for (var i = 0; i<corners.Count; i++) {
			vertecies.Add (new Node (corners [i]));
		}

		vertecies = FindNeighbors (vertecies, size);

		vertecies = FindResources (vertecies, tiles, size);

		vertecies = FindLogicalPos (vertecies);

		return vertecies;
	}

	static List<Node> FindNeighbors(List<Node> vertecies, float size)
	{
		float threshold = size + 0.2f;

		for (var i = 0; i<vertecies.Count; i++) {
			for (var j = 0; j<vertecies.Count;j++){
				double d = Math.Sqrt( Math.Pow((vertecies[i]).getLoc().x - (vertecies[j]).getLoc().x, 2) + Math.Pow((vertecies[i]).getLoc().y - (vertecies[j]).getLoc().y, 2));
				if(d<threshold && i!=j){
					(vertecies[i]).addNeighbor(vertecies[j]);
				}	
			}
		}



		return vertecies;
	}

	static List<Node> FindResources(List<Node> vertecies, List<Tile> tiles, float size)
	{
		float threshold = size + 0.2f;
		
		for (var i = 0; i<vertecies.Count; i++) {
			for (var j = 0; j<tiles.Count;j++){
				double d = Math.Sqrt( Math.Pow((vertecies[i]).getLoc().x - (tiles[j]).getLoc().x, 2) + Math.Pow((vertecies[i]).getLoc().y - (tiles[j]).getLoc().y, 2));
				if(d<threshold && i!=j){
					(vertecies[i]).addTile(tiles[j]);
				}	
			}
		}

		return vertecies;
	}

	static List<Node> FindLogicalPos(List<Node> vertecies)
	{
		float threshold = 0.2f;
		
		for (var i = 0; i<vertecies.Count; i++) {

		}
		
		return vertecies;
	}

	static List<Edge> FindRoadPos(List<Node> vertecies){

		List<Edge> roads = new List<Edge> ();
		List<Node> visited = new List<Node> (54);
		for (var i = 0; i<vertecies.Count; i++) {
			visited.Add(vertecies[i]);
			List<Node> neighbors = (vertecies[i]).getNeighbors();
			for (var j = 0; j<neighbors.Count;j++) {
				if(!visited.Contains(neighbors[j])){

					float x1 = (vertecies[i]).getLoc().x;
					float x2 = (neighbors[j]).getLoc().x;
					float y1 = (vertecies[i]).getLoc().y;
					float y2 = (neighbors[j]).getLoc().y;

					float x_mid = (x1 + x2)/2;
					float y_mid = (y1 + y2)/2;
					float angle = (float)Math.Atan((y1 - y2)/(x1 - x2))*57.2957795f; //converting from radians to degrees
					print (angle);
					roads.Add(new Edge(new Vector3(x_mid, y_mid, 0), angle) );


				}
			}

		}


		return roads;

	}





	// Update is called once per frame
	void Update () {
	
	}
}
