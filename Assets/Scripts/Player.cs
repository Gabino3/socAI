﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player
{
	public int id;
	public List<int> cards;
	public Color color;
	PlayerHand hand;

	int numSettlements;
	int numCities;
	public int longestRoad;
	public int largestArmy;
	
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

		cards = new List<int>{ 0, 0, 0, 0, 0 };

		hand = new PlayerHand();
	}

	public int calculateVictoryPoints()
	{
		return (1 * numSettlements) + (2 * numCities) + hand.victoryPoints + largestArmyModifier() + longestRoadModifier();
	}

	public bool hasWon()
	{
		return calculateVictoryPoints >= 10;
	}

	// Dummy method to include largest army; will relocate to GameState at a future point
	public int largestArmyModifier()
	{
		return 0;
	}

	// Dummy method to include longest road; will relocate to GameState at a future point
	public int longestRoadModifier()
	{
		return 0;
	}
}
