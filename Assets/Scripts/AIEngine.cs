using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; //sorting

public class AIEngine
{
	private static readonly double ROAD_VALUE = 2/5;
	private static readonly double SETTLEMENT_VALUE = 1;
	private static readonly double CITY_VALUE = 1;

	private static readonly double[] CHIT_PROBABILITIES = { 1, 0, .0278, .0556, .0833, .1111, .1389, .1667, .1389, .1111, .0833, .0556, .0278 };
	private static readonly double[] DEPTH_MODIFIERS = { 0, 1, 1.2, 0.7, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 }; //can be modified

	/*
	 * Returns a sorted list of new expansions from a set of roads from most to least favorable.
	 */
	public static List<Edge> GetFavorableRoadExpansions(Player player, Board board, Node parent)
	{
		List<ScoredEdge> scoredExpansions = new List<ScoredEdge> ();

		foreach (Edge possibleExpansion in parent.getRoads ()) {
			if (!possibleExpansion.occupied) {
				List<Edge> visitedEdges = new List<Edge>();
				List<Node> visitedNodes = new List<Node>();
				visitedNodes.Add (parent); //avoid muddling results with other edges
				double score = ScoreEdge (player, board, possibleExpansion, visitedEdges, visitedNodes, 1, 4);
				scoredExpansions.Add(new ScoredEdge(possibleExpansion, score));
			}
		}

		// Sort and return actions based on score
		List<Edge> favorableEdges = new List<Edge>();
		scoredExpansions = scoredExpansions.OrderByDescending (e => e.score).ToList ();
		foreach (ScoredEdge scoredEdge in scoredExpansions) {
			favorableEdges.Add (scoredEdge.edge);
		}
		
		return favorableEdges;
	}

	/*
	 * Returns a sorted list of starting locations from most to least favorable.
	 */
	public static List<Node> GetFavorableStartingLocations(Board board)
	{
		List<Node> vertices = board.vertices;
		List<ScoredNode> scoredVertices = new List<ScoredNode> ();

		//retrieve unoccupied nodes and score
		foreach (Node vertex in vertices) {
			if (vertex.owner == null && vertex.getTiles().Count > 1) {
				scoredVertices.Add (new ScoredNode(vertex, VertexScore(vertex)));
			}
		}

		//sort scored nodes
		scoredVertices = scoredVertices.OrderByDescending(v=>v.score).ToList();

		//return unscored nodes in sorted order
		vertices = new List<Node> ();
		foreach (ScoredNode scoredVertex in scoredVertices) {
			vertices.Add (scoredVertex.node);
		}

		return vertices;
	}

	public static List<Tile> GetListOfRobberPlacements(Player currentTurnPlayer, Player competitorPlayer, Board board)
	{
		//Get list of current player's tiles
		List<Tile> myTiles = new List<Tile>();
		foreach (Node structure in currentTurnPlayer.structures) {
			foreach (Tile tile in structure.getTiles ()) {
				myTiles.Add (tile);
			}
		}

		List<ScoredTile> scoredCompetitorTiles = new List<ScoredTile> ();
		
		//Get scored lists of top competitors tiles that don't neighbor current player
		foreach (Node structure in competitorPlayer.structures) {
			foreach (Tile tile in structure.getTiles ()) {

				//Doesn't neighbor current player
				if (!myTiles.Contains (tile)) {
					int cityModifier = (structure.occupied == Node.Occupation.city) ? 2 : 1;

					double score = AIEngine.TileScore(tile) * cityModifier;
					bool updated = false;

					//Append score if already discovered
					foreach (ScoredTile scoredTile in scoredCompetitorTiles) {
						if (scoredTile.tile == tile) {
							scoredTile.score += score;
							updated = true;
						}
					}

					//Add to list of discovered if not already
					if (!updated) {
						scoredCompetitorTiles.Add (new ScoredTile(tile, score));
					}
				}
			}
		}

		List<Tile> competitorTiles = new List<Tile>();

		//Return sorted list of tile options if available
		if (scoredCompetitorTiles.Count > 0) {
			scoredCompetitorTiles = scoredCompetitorTiles.OrderByDescending(t => t.score).ToList ();

			foreach (ScoredTile scoredTile in scoredCompetitorTiles) {
				competitorTiles.Add (scoredTile.tile);
			}
		}
		//If all else fails, just return the desert tile
		else {
			Tile desert = null;

			foreach (Tile tile in board.tiles) {
				if (tile.GetResource () == Tile.Resource.none) {
					desert = tile;
					break;
				}
			}

			competitorTiles.Add (desert);
		}

		return competitorTiles;
	}

	/*
	 * Returns a sorted list of recommended objectives for the AI to pursue
	 */
	public static List<Objective> GetObjectives(Player player, Board board, GameState gamestate)
	{
		/* ********************
		 | Constructions:     |
		 |   settlement		  |
		 |   city			  |
		 |   road+settlement  |
		 |   2road+settlement |
		 **********************/

		List<Node> playerNodes = new List<Node> ();
		List<Edge> playerEdges = new List<Edge> ();
		List<Objective> objectives = new List<Objective> ();
		List<Objective> prunedObjectives = new List<Objective> ();

		//Get list of all nodes touching player roads
		foreach(Edge road in player.roads) {
			playerEdges.Add (road);
			foreach(Node node in road.getNeighbors()) {
				if (!playerNodes.Contains (node)) {
					playerNodes.Add (node);
				}
			}
		}

		//Attempt constructions from each node
		foreach (Node playerNode in playerNodes) {
			List<Edge> visitedEdges = new List<Edge>();
			List<Node> visitedNodes = new List<Node>();

			//Keep track of player nodes/edges so we don't backtrack
			foreach(Edge visitedEdge in playerEdges) {
				visitedEdges.Add (visitedEdge);
			}
			foreach(Node visitedNode in playerNodes) {
				visitedNodes.Add (visitedNode);
			}

			Objective objective = new Objective(player, gamestate);
			GetObjectivesHelper (player, board, playerNode, objective, objectives, visitedEdges, visitedNodes, 0, 2); //TODO try 3 for maxdepth
		}

		//Prune unwanted objectives
		foreach (Objective objective in objectives) {
			objective.RecalculateForLongestRoad ();
			if (objective.TotalCardsNeeded () <= 3 && objective.Score() > 0) { //TODO prune scoring
				prunedObjectives.Add (objective);
			}
		}

		return prunedObjectives.OrderByDescending (o => o.Score ()).ToList ();
	}

	/*
	 * Helper class that traverses nodes and collects additional objectives given constraints.
	 */
	private static void GetObjectivesHelper(Player player, Board board, Node node, Objective objective, List<Objective> objectives, List<Edge> visitedEdges, List<Node> visitedNodes, int depth, int maxDepth)
	{
		visitedNodes.Add (node);

		//Why not add a settlement/upgrade?
		if (node.occupied == Node.Occupation.none) {
			if (board.CanBuildSettlementHere(node.visual.transform, player, true)) {
				Objective newObjective = objective.Clone();
				newObjective.AddSettlement (node);
				objectives.Add (newObjective);
			}
		}
		else if (node.occupied == Node.Occupation.settlement && node.owner == player) {
			Objective newObjective = objective.Clone();
			newObjective.AddCity (node);
			objectives.Add (newObjective);
		}

		//End of the line
		if (depth >= maxDepth) {
			return;
		}

		//Continue traversal
		foreach (Edge road in node.getRoads ()) {
			if (!visitedEdges.Contains (road) && !road.occupied) {
				visitedEdges.Add (road);
				foreach (Node neighbor in road.getNeighbors ()) {
					if (!visitedNodes.Contains (neighbor)) {
						Objective newObjective = objective.Clone();
						newObjective.AddRoad (road);
						objectives.Add (newObjective);
						GetObjectivesHelper (player, board, neighbor, newObjective, objectives, visitedEdges, visitedNodes, depth+1, maxDepth);
					}
				}
			}
		}
	}

	public static bool PerformObjective(Objective objective, Board board) {
		Player player = objective.GetPlayer ();
		bool partiallyPerformed = false;

		foreach (Edge road in objective.GetRoads ()) {
			if (player.CanBuildRoad () && board.CanBuildRoadHere (road.visual.transform, player)) {
				board.PlaceRoad (road.visual.transform, player);
				partiallyPerformed = true;
			}
		}

		foreach (Node settlement in objective.GetSettlements ()) {
			if (player.CanBuildSettlement () && board.CanBuildSettlementHere (settlement.visual.transform, player, false)) {
				board.PlaceSettlement (settlement.visual.transform, player);
				partiallyPerformed = true;
			}
		}

		foreach (Node city in objective.GetCities ()) {
			if (player.CanBuildCity () && board.CanBuildCityHere (city.visual.transform, player)) {
				board.PlaceCity (city.visual.transform, player);
				partiallyPerformed = true;
			}
		}

		return partiallyPerformed;
	}

	/*
	 * Score an edge based on possible future expansions up to a crtain depth. Scoring changes with distance from start.
	 */
	public static double ScoreEdge(Player player, Board board, Edge edge, List<Edge> visitedEdges, List<Node> visitedNodes, int depth, int maxDepth) {
		
		if (depth > maxDepth) {
			return 0;
		}
		
		double score = 0;
		visitedEdges.Add (edge);
		
		foreach (Node neighbor in edge.getNeighbors ()) {
			if (neighbor.occupied == Node.Occupation.none && board.CanBuildSettlementHere(neighbor.visual.transform, player, false)) {
				foreach (Tile tile in neighbor.getTiles ()) {
					score += TileScore (tile) * DEPTH_MODIFIERS[depth];
				}
			}
			
			if (!visitedNodes.Contains (neighbor)) {
				visitedNodes.Add (neighbor);
				foreach(Edge neighborEdge in neighbor.getRoads ()) {
					if (!visitedEdges.Contains (neighborEdge)) {
						score += ScoreEdge (player, board, neighborEdge, visitedEdges, visitedNodes, depth+1, maxDepth);
					}
				}
			}
		}
		
		return score;
	}

	/*
	 * Score tile based on resource and chit value.
	 */
	public static double TileScore(Tile tile)
	{
		double probability = CHIT_PROBABILITIES [tile.GetChitValue ()];

		switch (tile.GetResource ()) {
		case Tile.Resource.brick: return ValueOfBrick()*probability; break;
		case Tile.Resource.ore: return ValueOfOre()*probability; break;
		case Tile.Resource.wood: return ValueOfWood()*probability; break;
		case Tile.Resource.grain: return ValueOfGrain()*probability; break;
		case Tile.Resource.sheep: return ValueOfSheep()*probability; break;
		default: return 0; break; // desert
		}
	}

	/*
	 * Resource values
	 */
	public static double ValueOfBrick()
	{
		return ROAD_VALUE/2 + SETTLEMENT_VALUE/4 + CITY_VALUE*0;
	}
	public static double ValueOfOre()
	{
		return ROAD_VALUE*0 + SETTLEMENT_VALUE*0 + (CITY_VALUE*3)/5;
	}
	public static double ValueOfWood()
	{
		return ROAD_VALUE/2 + SETTLEMENT_VALUE/4 + CITY_VALUE*0;
	}
	public static double ValueOfGrain()
	{
		return ROAD_VALUE*0 + SETTLEMENT_VALUE/4 + (CITY_VALUE*2)/5;
	}
	public static double ValueOfSheep()
	{
		return ROAD_VALUE*0 + SETTLEMENT_VALUE/4 + CITY_VALUE*0 + 0.1;
	}
	public static double AverageValue()
	{
		return (ValueOfBrick () + ValueOfOre () + ValueOfWood () + ValueOfGrain () + ValueOfSheep ()) / 5;
	}

	/*
	 * Score vertex by surrounding tile values.
	 */
	public static double VertexScore(Node vertex)
	{
		double score = 0;

		foreach (Tile tile in vertex.getTiles()) {
			score += TileScore(tile);
		}

		return score;
	}

	/* ======== *
	 * A building objective with an associated score value.
	 * ======== */
	public class Objective
	{
		/* ********************
		 | Constructions:     |
		 |   settlement		  |
		 |   city			  |
		 |   road+settlement  |
		 |   2road+settlement |
		 **********************/
		
		private Player player;
		private List<Edge> roads;
		private List<Node> settlements;
		private List<Node> cities; //WARNING! contains settlements to be upgraded, not actual cities
		private double score;
		private PlayerHand cardsNeeded;						// collection of cards required for objective
		private PlayerHand cardDifferentialForObjective;	// differential between cards needed and playerhand.  (-) if required, (+) if surplus
		private int curLongestRoad;
		private GameState gamestate;

		public Objective(Player player, GameState gamestate)
		{
			this.player = player;
			this.gamestate = gamestate;
			roads = new List<Edge>();
			settlements = new List<Node>();
			cities = new List<Node>();
			score = 0;
			cardsNeeded = new PlayerHand();
			curLongestRoad = Board.LongestRoadOfPlayer(player);
		}

		private Objective(Player player, GameState gamestate, List<Edge> roads, List<Node> settlements, List<Node> cities, double score, PlayerHand hand, int curLongestRoad)
		{
			List<Edge> roadsCopy = new List<Edge>();
			List<Node> settlementsCopy = new List<Node>();
			List<Node> citiesCopy = new List<Node>();

			foreach(Edge road in roads) {
				roadsCopy.Add (road);
			}
			foreach(Node settlement in settlements) {
				settlementsCopy.Add (settlement);
			}
			foreach(Node city in cities) {
				citiesCopy.Add (city);
			}

			this.player = player;
			this.gamestate = gamestate;
			this.roads = roadsCopy;
			this.settlements = settlementsCopy;
			this.cities = citiesCopy;
			this.score = score;
			this.cardsNeeded = hand;
			this.curLongestRoad = curLongestRoad;
		}

		public double GetObjectiveScore()
		{
			return score;
		}

		public void AddCity(Node city)
		{
			score += VertexScore (city);
			cities.Add (city);
			RecalculateHand ();
		}

		public void AddRoad(Edge newRoad)
		{
			roads.Add (newRoad);
			RecalculateHand ();
		}

		public void AddSettlement(Node settlement)
		{
			score += VertexScore (settlement);
			settlements.Add (settlement);
			RecalculateHand ();
		}

		public PlayerHand GetCardDifferential()
		{
			return cardDifferentialForObjective;
		}

		public PlayerHand GetCardsNeeded()
		{
			return cardsNeeded;
		}

		public int TotalCardsNeeded()
		{
			return cardsNeeded.GetHandSize();
		}

		public Objective Clone()
		{
			return new Objective(player, gamestate, roads, settlements, cities, score, cardsNeeded, curLongestRoad);
		}

		public List<Node> GetCities()
		{
			return cities;
		}

		public Player GetPlayer()
		{
			return player;
		}

		public List<Edge> GetRoads()
		{
			return roads;
		}

		public List<Node> GetSettlements()
		{
			return settlements;
		}

		public void RecalculateForLongestRoad()
		{
			int newLongest;
			
			//Add roads to player to simulate new longest road
			foreach (Edge road in roads) {
				player.AddRoad (road);
			}
			newLongest = Board.LongestRoadOfPlayer (player);
			
			//Remove all simulated edges
			foreach (Edge road in roads) {
				player.RemoveRoad (road);
			}
			
			//Add extra points if roads connect two previous ones
			if (newLongest > curLongestRoad) { //TODO correct logic?
				if (!gamestate.HasLongestRoad(player) && gamestate.WouldBeLongestRoad(newLongest)) {
					score += 1; //TODO adjust this
				} else {
					score += 0.00; //TODO adjust this
				}
			}
		}

		//TODO all card logic here is probably better suited in another class
		private void RecalculateHand()
		{
			// Justian Logic to calculate cards needed; the hand contains positive values for cards required to perform objective
			cardsNeeded = new PlayerHand ();

			PlayerHand neededCards = new PlayerHand();
			foreach (Edge road in roads) {
				neededCards.brick++;
				neededCards.wood++;
			}
			foreach (Node settlement in settlements) {
				neededCards.brick++;
				neededCards.wood++;
				neededCards.grain++;
				neededCards.sheep++;
			}
			foreach (Node city in cities) {
				neededCards.ore+=3;
				neededCards.grain+=2;
			}

			// Kevin Logic to calculate cards needed, a differential between cards-needed-to-build and current player's hand.  Negative
			// 		values indicate a lack of cards, positive values indicate a surplus
			
			cardDifferentialForObjective = player.GetPlayerHand ().MakeHandDifferentialFromRequiredResources (neededCards.brick, 
			                                                                                                  neededCards.ore, 
			                                                                                                  neededCards.wood, 
			                                                                                                  neededCards.grain, 
			                                                                                                  neededCards.sheep);

			// Sets the cardsneeded variable to included only those cards the Player does not yet have
			for (int i = 0; i < 5; i++)
			{
				neededCards.SetResourceQuantity(i, neededCards.GetResourceQuantity(i) - player.GetPlayerHand ().GetResourceQuantity(i));

				if (neededCards.GetResourceQuantity(i) > 0)
				{
					cardsNeeded.SetResourceQuantity(i, neededCards.GetResourceQuantity(i));
				}
			}
		}

		public double Score()
		{
			return score;
		}

		public override string ToString ()
		{
			return "Objective: (Player:"+player.id+" > Roads:"+roads.Count+", Settlements:"+settlements.Count+", Cities:"+cities.Count+", Cards:"+TotalCardsNeeded()+", Score:"+score+")";
		}
	}

	/* ======== *
	 * Node that has its own score. Used for determining most favorable options.
	 * ======== */
	private class ScoredNode
	{
		public Node node;
		public double score;

		public ScoredNode(Node node, double score)
		{
			this.node = node;
			this.score = score;
		}
	}

	/* ======== *
	 * Edge that has its own score. Used for determining most favorable options.
	 * ======== */
	private class ScoredEdge
	{
		public Edge edge;
		public double score;

		public ScoredEdge(Edge edge, double score)
		{
			this.edge = edge;
			this.score = score;
		}
	}

	/* ======== *
	 * Tile that has its own score. Used for determining most favorable options.
	 * ======== */
	private class ScoredTile
	{
		public Tile tile;
		public double score;

		public ScoredTile(Tile tile, double score)
		{
			this.tile = tile;
			this.score = score;
		}
	}

}
