using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player
{
	public int id;
	public bool isAI;
	public Color color;
	public PlayerHand hand;

	int numSettlements;
	int numCities;
	public int longestRoad;
	public int largestArmy;
	
	public Player(int id, bool isAI)
	{
		this.id = id;
		this.isAI = isAI;
		switch (id)
		{
			case 1: color = Color.blue; break;
			case 2: color = Color.red; break;
			case 3: color = Color.green; break;
			case 4: color = Color.yellow; break;
			default: color = Color.black; break;
		}

		hand = new PlayerHand();
	}

	public int CalculateVictoryPoints()
	{
		return (1 * numSettlements) + (2 * numCities) + hand.victoryPoints + GetLargestArmyModifier() + GetLongestRoadModifier();
	}

	public bool HasWon()
	{
		return CalculateVictoryPoints() >= 10;
	}

	// Dummy method to include largest army; will relocate to GameState at a future point
	public int GetLargestArmyModifier()
	{
		return 0;
	}

	// Dummy method to include longest road; will relocate to GameState at a future point
	public int GetLongestRoadModifier()
	{
		return 0;
	}
}
