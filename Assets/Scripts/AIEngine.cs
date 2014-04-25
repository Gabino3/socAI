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
			GameEngine.print(scoredEdge.edge.occupied);
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
	private static double TileScore(Tile tile)
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

	/*
	 * Score vertex by surrounding tile values.
	 */
	private static double VertexScore(Node vertex)
	{
		double score = 0;

		foreach (Tile tile in vertex.getTiles()) {
			score += TileScore(tile);
		}

		return score;
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

}
