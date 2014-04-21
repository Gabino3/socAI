using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player
{
	public int id;
	public bool isAI;
	public Color color;
	PlayerHand hand;
	
	List<Edge> roads;
	List<Node> structures;
	public int longestRoad;
	public int largestArmy;
	
	public Player(int id, bool isAI)
	{
		this.id = id;
		this.isAI = isAI;

		switch (id)
		{
			case 0: color = Color.blue; break;
			case 1: color = Color.red; break;
			case 2: color = Color.green; break;
			case 3: color = Color.yellow; break;
			default: color = Color.black; break;
		}

		hand = new PlayerHand();
	}

	public void AddStructure()
	{
		//TODO
	}

	public bool CanBuildCity()
	{
		return (hand.ore >= 3) && (hand.grain >= 2);
	}

	public bool CanBuildRoad()
	{
		return (hand.brick >= 1) && (hand.wood >= 1);
	}

	public bool CanBuildSettlement()
	{
		return (hand.brick >= 1) && (hand.wood >= 1) && (hand.grain >= 1) && (hand.sheep >= 1);
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
		return 1;//return (1 * NumSettlements()) + (2 * NumCities()) + hand.victoryPoints + GetLargestArmyModifier() + GetLongestRoadModifier();
	}
}
