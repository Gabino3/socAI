using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player
{
	//Debug variables
	private bool debug = true;

	public int id;
	public bool isAI;
	public Color color;
	private PlayerHand hand;
	
	private List<Edge> roads;
	private List<Node> structures;
	public int longestRoad;
	public int largestArmy;
	
	public Player(int id, bool isAI)
	{
		this.id = id;
		this.isAI = isAI;

		switch (id)
		{
			case 0: color = Color.red; break;
			case 1: color = Color.yellow; break;
			case 2: color = Color.green; break;
			case 3: color = Color.blue; break;
			default: color = Color.black; break;
		}
		structures = new List<Node> (54);
		hand = new PlayerHand();
	}

	public void AddStructure(Node structure)
	{
		structures.Add (structure);
	}

	public void AddRoad(Edge road)
	{
		roads.Add (road);
	}

	public bool CanBuildCity()
	{
		if ((hand.ore >= 3) && (hand.grain >= 2)) {
			return true;
		}
		GameEngine.print ("Not Enough Resources!");
		return false;
	}

	public bool CanBuildRoad()
	{
		if ((hand.brick >= 1) && (hand.wood >= 1)) {
			return true;
		}
		
		GameEngine.print ("Not Enough Resources!");
		return false;

	}

	public bool CanBuildSettlement()
	{
		if ((hand.brick >= 1) && (hand.wood >= 1) && (hand.grain >= 1) && (hand.sheep >= 1)) {
			return true;
		}
			
		GameEngine.print ("Not Enough Resources!");
		return false;
	}

	/*
	 * Collects a player's resources at the dice-rolling portion of the turn based on their settlements.
	 */
	public void CollectResources(int diceRoll)
	{
		//Search adjacent tiles of all structures
		structures.ForEach (delegate(Node structure) {
			structure.getTiles().ForEach (delegate(Tile tile) {
				if (tile.GetChitValue () == diceRoll) {

					int resourceModifier = 0;
					string resource = "";

					//Give 1 resource for settlement, 2 for city
					if (structure.occupied == Node.Occupation.settlement) {
						resourceModifier = 1;
					} else if (structure.occupied == Node.Occupation.city) {
						resourceModifier = 2;
					}

					//Give player appropriate resource
					if (tile.GetResource() == Tile.Resource.brick) {
						hand.brick += resourceModifier;
						resource = "brick";
					} else if (tile.GetResource() == Tile.Resource.ore) {
						hand.ore += resourceModifier;
						resource = "ore";
					} else if (tile.GetResource() == Tile.Resource.wood) {
						hand.wood += resourceModifier;
						resource = "wood";
					} else if (tile.GetResource() == Tile.Resource.grain) {
						hand.grain += resourceModifier;
						resource = "grain";
					} else if (tile.GetResource() == Tile.Resource.sheep) {
						hand.sheep += resourceModifier;
						resource = "sheep";
					}

					if (debug) {
						GameEngine.print ("Gave player " + resourceModifier + " " + resource);
						GameEngine.print ("b: " + hand.brick + ", o: " + hand.ore + ", w: " + hand.wood + ", g: " + hand.grain + ", s: " + hand.sheep);
					}
				}
			});
		});
	}

	// Dummy method to include largest army; will relocate to GameState at a future point
	public int GetLargestArmyModifier()
	{
		return GameState.HasLargestArmy (this) ? 2 : 0;
	}

	// Dummy method to include longest road; will relocate to GameState at a future point
	public int GetLongestRoadModifier()
	{
		return GameState.HasLongestRoad (this) ? 2 : 0;
	}

	public PlayerHand Hand()
	{
		return hand;
	}

	public bool HasWon()
	{
		return VictoryPoints() >= 10;
	}

	private int NumCities()
	{
		int numCities = 0;

		structures.ForEach (delegate(Node structure) {
			if (structure.occupied == Node.Occupation.city) {
				numCities++;
			}
		});

		return numCities;
	}

	private int NumSettlements()
	{
		int numSettlements = 0;
		
		structures.ForEach (delegate(Node structure) {
			if (structure.occupied == Node.Occupation.settlement) {
				numSettlements++;
			}
		});
		
		return numSettlements;
	}

	public int VictoryPoints()
	{
		return (1 * NumSettlements()) + (2 * NumCities()) + hand.victoryPoints + GetLargestArmyModifier() + GetLongestRoadModifier();
	}
}
