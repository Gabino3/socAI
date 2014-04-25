using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player
{
	//Debug variables
	private bool debug = false;

	public int id;
	public bool isAI;
	public Color color;
	private PlayerHand hand;
	
	public List<Edge> roads;
	public List<Node> structures;

	Dictionary <String, int> recentTradeRequests = new Dictionary<String, int> ();

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
		roads = new List<Edge> (72);
		structures = new List<Node> (54);
		hand = new PlayerHand ();//(100); //TODO just testing...
	}

	public PlayerHand GetPlayerHand()
	{
		return hand;
	}

	public void AddStructure(Node structure)
	{
		structures.Add (structure);
	}

	public void AddRoad(Edge road)
	{
		roads.Add (road);
	}

	public void BuyCity()
	{
		hand.grain += -2;
		hand.ore += -3;
	}

	public void BuyRoad()
	{
		hand.brick += -1;
		hand.wood += -1;
	}

	public void BuySettlement()
	{
		hand.brick += -1;
		hand.wood += -1;
		hand.grain += -1;
		hand.sheep += -1;
	}

	public bool CanBuildCity()
	{
		if ((hand.ore >= 3) && (hand.grain >= 2))
		{
			return true;
		}
		GameEngine.print ("Not Enough Resources!");
		return false;
	}

	public bool CanBuildRoad()
	{
		if ((hand.brick >= 1) && (hand.wood >= 1))
		{
			return true;
		}
		
		GameEngine.print ("Not Enough Resources!");
		return false;

	}

	public bool CanBuildSettlement()
	{
		if ((hand.brick >= 1) && (hand.wood >= 1) && (hand.grain >= 1) && (hand.sheep >= 1))
		{
			return true;
		}
			
		GameEngine.print ("Not Enough Resources!");
		return false;
	}

	/*
	 * Collects a player's resources at the dice-rolling portion of the turn based on their settlements.
	 */
	public void CollectResourcesFromRoll(Board board, int diceRoll)
	{
		structures.ForEach (delegate(Node structure) {
			CollectResourcesFromStructure (board, structure, diceRoll);
		});
	}

	public void CollectResourcesFromStructure(Board board, int index)
	{
		CollectResourcesFromStructure (board, structures [index]);
	}

	private void CollectResourcesFromStructure(Board board, Node structure, int diceRoll = -1)
	{
		int resourceModifier = 0;

		//Give 1 resource for settlement, 2 for city
		if (structure.occupied == Node.Occupation.settlement) {
			resourceModifier = 1;
		} else if (structure.occupied == Node.Occupation.city) {
			resourceModifier = 2;
		}

		structure.getTiles().ForEach (delegate(Tile tile) {
			if (tile.GetChitValue () == diceRoll || diceRoll == -1) {
				if (tile != board.robberOwner) {
					string resource = "";
					
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
						GameEngine.print ("Gave player #" + id + ": " + resourceModifier + " " + resource);
						GameEngine.print ("Player #" + id + " Hand: b > " + hand.brick + ", o > " + hand.ore + ", w > " + hand.wood + ", g > " + hand.grain + ", s > " + hand.sheep);
					}
				}
				else {
					if (debug) {
						GameEngine.print ("Player #" + id + " didn't receive some of their resources due to robber.");
					}
				}
			}
		});
	}

	public PlayerHand Hand()
	{
		return hand;
	}

	public bool HasWon(bool hasLargestArmy, bool hasLongestRoad)
	{
		return VictoryPointsCount(hasLargestArmy, hasLongestRoad) >= 10;
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

	public bool RemoveRoad(Edge road)
	{
		return roads.Remove (road);
	}

	public int VictoryPointsCount(bool hasLargestArmy, bool hasLongestRoad)
	{
		int largestArmyPoints = (hasLargestArmy) ? 2 : 0;
		int longestRoadPoints = (hasLongestRoad) ? 2 : 0;
		return (1 * NumSettlements()) + (2 * NumCities()) + hand.victoryPoints + largestArmyPoints + longestRoadPoints;
	}

	public bool acceptTradeRequest(TradeOffer trade)
	{
		bool doesAcceptTrade = false;

		// Verify that trade is permissible based on player hand
		if(hand.isViableTradeRequest(trade))
		{
			// if player trade evaluation returns true, accept trade request
			doesAcceptTrade = evaluateTradeRequest(trade);
		}

		return doesAcceptTrade;
	}

	private bool evaluateTradeRequest(TradeOffer trade)
	{
		if(!isAI)
		{
			// Allow player to accept / reject / modify trade
		}
		else
		{
			// AI logic to determine whether or not to accept trade
		}

		return false;
	}

	public TradeOffer generateHumanTradeRequest(int currentTurn, int[] giveResources, int[] getResources)
	{
		return new TradeOffer(this, currentTurn, giveResources, getResources);
	}

	// Returns a TradeOffer if the trade is valid (an identical request has not recently been made); If this is not true, returns null
	public TradeOffer generateAITradeRequest(int currentTurn, int[] giveResources, int[] getResources)
	{
		String key = generateKeyForTrade (getResources);

		int lastRequestTurn = 0;
		if (recentTradeRequests.TryGetValue (key, out lastRequestTurn))
		{
		}

		if(lastRequestTurn < currentTurn)
		{
			TradeOffer newTrade = new TradeOffer(this, currentTurn, giveResources, getResources);

			if(lastRequestTurn != 0)
			{
				recentTradeRequests.Remove(key);
			}

			recentTradeRequests.Add(key, currentTurn);

			return newTrade;
		}

		return null;
	}

	private String generateKeyForTrade(int[] getResources)
	{
		return getResources[0].ToString() + "_" + getResources[1].ToString() + " " + getResources[2].ToString() + "_" + getResources[3].ToString() + "_" + getResources[4].ToString();
	}

	public void gotRobbed()
	{
		int handCount = hand.GetHandSize();
		if (handCount > 7) {
			int numDiscards = handCount / 2;
			for (int i = 0; i < numDiscards; i++) {
				hand.discard();
			}
		}
	}
}
