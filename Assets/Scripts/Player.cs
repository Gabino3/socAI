using UnityEngine;
using System.Collections;

public class Player
{
	public int id;
	public Color color;
	PlayerHand hand;

	int numSettlements;
	int numCities;
	int longestRoad;
	//int numKnights;
	
	public Player(int id)
	{
		this.id = id;
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

	private int calculateVictoryPoints()
	{
		return (1 * numSettlements) + (2 * numCities) + hand.victoryPoints + largestArmyModifier() + longestRoadModifier();
	}

	// Dummy method to include largest army; will relocate to GameState at a future point
	private int largestArmyModifier()
	{
		return 0;
	}

	// Dummy method to include longest road; will relocate to GameState at a future point
	private int longestRoadModifier()
	{
		return 0;
	}
}
