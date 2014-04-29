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
	
	public List<Edge> roads;
	public List<Node> structures;

	bool debugMessages = false;

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
		hand = new PlayerHand (0);//(100); //TODO just testing...
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

		if(debugMessages)
		{
			GameEngine.print ("Not Enough Resources!");
		}

		return false;
	}

	public bool CanBuildRoad()
	{
		if ((hand.brick >= 1) && (hand.wood >= 1))
		{
			return true;
		}

		if(debugMessages)
		{
			GameEngine.print ("Not Enough Resources!");
		}

		return false;

	}

	public bool CanBuildSettlement()
	{
		if ((hand.brick >= 1) && (hand.wood >= 1) && (hand.grain >= 1) && (hand.sheep >= 1))
		{
			return true;
		}

		if(debugMessages)
		{
			GameEngine.print ("Not Enough Resources!");
		}

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

	public bool processTradeRequest(GameState gamestate, TradeOffer trade)
	{
		if(debugMessages)
		{
			GameEngine.print ("PLAYER " + trade.tradeHost.id + " ATTEMPTING TO BROKER TRADE WITH PLAYER " + this.id);
		}

		bool doesAcceptTrade = false;

		// Verify that trade is permissible based on player hand
		if(hand.IsViableTradeRequest(trade))
		{
			// if player trade evaluation returns true, accept trade request
			doesAcceptTrade = evaluateTradeRequest(gamestate, trade);
		}

		return doesAcceptTrade;
	}

	private bool evaluateTradeRequest(GameState gamestate, TradeOffer trade)
	{
		bool acceptTradeRequest = false;

		if(!isAI)
		{
			// Allow player to accept / reject / modify trade
		}
		else
		{
			List<AIEngine.Objective> objectives = AIEngine.GetObjectives(this, gamestate.getGamestateBoard(), gamestate);

			foreach(AIEngine.Objective objective in objectives)
			{
				PlayerHand thisPlayerNeedResources = objective.GetCardDifferential();

				PlayerHand thisPlayerGiveResources = trade.convertGetResourcesToPlayerHand();	// tradeHost gets the resources that this Player gives
				PlayerHand thisPlayerGetResources = trade.convertGiveResourcesToPlayerHand();	// tradeHost gives the resources that this Player gets

				if(debugMessages)
				{
					GameEngine.print (objective.GetObjectiveScore() + ": OBJECTIVE SCORE");
					GameEngine.print ("PLAYER " + this.id + " GIVES TO TRADEHOST:" +
					                  "GIVE " + trade.getBrick + " BRICK, " +
					                  "GIVE " + trade.getOre + " ORE, " +
					                  "GIVE " + trade.getWood + " WOOD, " +
					                  "GIVE " + trade.getGrain + " GRAIN, " +
					                  "GIVE " + trade.getSheep + " SHEEP");
					
					GameEngine.print ("FOR " + trade.giveBrick + " BRICK, " +
					                  "FOR " + trade.giveOre + " ORE, " +
					                  "FOR " + trade.giveWood + " WOOD, " +
					                  "FOR " + trade.giveGrain + " GRAIN, " +
					                  "FOR " + trade.giveSheep + " SHEEP");
					
					int[] x = thisPlayerNeedResources.ToArray();
					GameEngine.print ("PLAYER " + this.id + " (SELLER) NEEDS:" +
					                  x[0] + " BRICK, " +
					                  x[1] + " ORE, " +
					                  x[2] + " WOOD, " +
					                  x[3] + " GRAIN, " +
					                  x[4] + " SHEEP");
				}

				bool resourcesNeededOverlapsWithResourcesReceived = false;
				// Compare Cards Received with Cards Needed
				for(int i = 0; i < 5; i++)
				{
					if(thisPlayerNeedResources.GetResourceQuantity(i) < 0 && thisPlayerGetResources.GetResourceQuantity(i) > 0)
					{
						resourcesNeededOverlapsWithResourcesReceived = true;
					}
				}

				bool resourcesNeededOverlapsWithResourcesGiven = false;
				for(int i = 0; i < 5; i++)
				{
					if(thisPlayerNeedResources.GetResourceQuantity(i) >= 0 && thisPlayerNeedResources.GetResourceQuantity(i) - thisPlayerGiveResources.GetResourceQuantity(i) < 0)
					{
						resourcesNeededOverlapsWithResourcesGiven = true;
					}
				}

				if(debugMessages)
				{
					// If any resources needed and received overlap && any resources needed and given do not overlap
					GameEngine.print ("NEEDS OVERLAPS WITH GET?: " + resourcesNeededOverlapsWithResourcesReceived + "(-true-), " +
					                  "NEEDS OVERLAPS WITH GIVE?: " + resourcesNeededOverlapsWithResourcesGiven + " (-false-)");
				}
				
				if(resourcesNeededOverlapsWithResourcesReceived && !resourcesNeededOverlapsWithResourcesGiven)
				{
					if(debugMessages)
					{
						GameEngine.print ("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ GET NEEDED RESOURCES & ONLY TRADE SURPLUS RESOURCES @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
					}

					if(trade.IsFairTrade() || trade.isGoodTradeForSeller())
					{
						acceptTradeRequest = true;
					}
					else
					{
						System.Random rand = new System.Random();

						if(rand.Next (100) < 25)			// 25% Chance to accept an unfair trade (technically could be better for them, but sometimes worse for them)
						{
							acceptTradeRequest = true;
						}
					}
				}

				if(acceptTradeRequest)
				{
					if(debugMessages)
					{
						GameEngine.print ("# # # # # # # # # # # # # # # # # # # # # # # # # # # ACCEPT TRADE # # # # # # # # # # # # # # # # # # # # # # # # # # #");
					}

					break;
				}
			}
		}

		return acceptTradeRequest;
	}

	public TradeOffer generateHumanTradeRequest(int currentTurn, int[] giveResources, int[] getResources)
	{
		return new TradeOffer(this, currentTurn, giveResources, getResources);
	}

	// Returns a TradeOffer if the trade is valid; If the ratio is not 4:1 or player does not have enough resources, returns null
	public TradeOffer generateAITradeWithBank(AIEngine.Objective objective)
	{
		if(debugMessages)
		{
			GameEngine.print ("-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#- BANK TRADE REQUEST -#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-");
		}

		return BuildTradeWithBank (objective.GetCardDifferential ());
	}

	// Returns a TradeOffer if the trade is valid (an identical request has not recently been made); If this is not true, returns null
	public TradeOffer generateAITradeRequest(int currentTurn, AIEngine.Objective objective)
	{
		TradeOffer trade = BuildFairTrade (currentTurn, objective.GetCardDifferential ());

		if(trade.TotalGetResources() > 1 && trade.TotalGiveResources() > 1)
		{
			trade.RandomUnequalizeTrade ();	// Introduces an element of randomness to the trade algorithm
		}

		if(debugMessages)
		{
			GameEngine.print ("PLAYER HAND: \n" +
			                  hand.brick + " BRICK\n" +
			                  hand.ore + " ORE\n" +
			                  hand.wood + " WOOD\n" +
			                  hand.grain + " GRAIN\n" +
			                  hand.sheep + " SHEEP");
			
			GameEngine.print ("CARDS NEEDED: \n" +
			                  objective.GetCardsNeeded().brick + " BRICK\n" +
			                  objective.GetCardsNeeded ().ore + " ORE\n" +
			                  objective.GetCardsNeeded ().wood + " WOOD\n" +
			                  objective.GetCardsNeeded ().grain + " GRAIN\n" +
			                  objective.GetCardsNeeded ().sheep + " SHEEP");
			
			GameEngine.print ("CARD DIFFERENTIAL: \n" +
			                  objective.GetCardsNeeded().GetHandSize() + " CARDS NEEDED\n" +
			                  objective.GetCardDifferential ().brick + " BRICK\n" +
			                  objective.GetCardDifferential ().ore + " ORE\n" +
			                  objective.GetCardDifferential ().wood + " WOOD\n" +
			                  objective.GetCardDifferential ().grain + " GRAIN\n" +
			                  objective.GetCardDifferential ().sheep + " SHEEP");
		}

		if(GetPermissionToRetryTradeRequest (currentTurn, trade))
		{
			return trade;
		}
		
		return null;
	}
	
	private bool GetPermissionToRetryTradeRequest(int currentTurn, TradeOffer trade)
	{
		bool permission = false;
		String key = trade.GenerateTradeKey ();

		int lastRequestTurn = 0;
		recentTradeRequests.TryGetValue (key, out lastRequestTurn);		// replaces lastRequestTurn w/ value found in dictionary, untouched if key not found

		if(debugMessages)
		{
			GameEngine.print ("REQUEST KEY: " + key + "$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$_$");
			GameEngine.print ("TURN LAST REQUESTED: " + lastRequestTurn);
		}

		if(lastRequestTurn < currentTurn)								// request not yet made this turn
		{
			if(debugMessages)
			{
				GameEngine.print("OVERWRITE KEY; FORMERLY " + lastRequestTurn + ", NOW: " + currentTurn);
			}

			if(lastRequestTurn != 0)
			{
				recentTradeRequests.Remove(key);
			}

			recentTradeRequests.Add(key, currentTurn);
			
			permission = true;
		}
		else
		{
			if(trade.TotalGetResources() > 1)
			{
				trade.dropGetCard();
				trade.EqualizeTrade();

				if(debugMessages)
				{
					GameEngine.print ("{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{-- REATTEMPT WITH FEWER CARDS REQUESTED --}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}");
				}

				permission = GetPermissionToRetryTradeRequest(currentTurn, trade);
			}
		}


		return permission;
	}

	private TradeOffer BuildTradeWithBank(PlayerHand cardDifferential)
	{
		int[] resourceRequest = new int[5];
		int[] resourceSurplus = new int[5];

		bool needsSomeCards = false;
		bool canTradeWithBank = false;
		for(int i = 0; i < 5; i++)
		{
			int cards = cardDifferential.GetResourceQuantity(i);
			
			if(cards < 0)
			{
				resourceRequest[i] = - cards;
				needsSomeCards = true;
			}
			else
			{
				resourceSurplus[i] = (cards / 4) * 4;

				if(resourceSurplus[i] > 0)
				{
					canTradeWithBank = true;
				}
			}
		}

		if(canTradeWithBank && needsSomeCards)
		{
			// This TradeOffer constructor is specifically for bank trades; auto-equalizes to a 1:4 ratio
			return new TradeOffer(this, resourceSurplus, resourceRequest);
		}
		else
		{
			if(debugMessages)
			{
				GameEngine.print ("NOT ENOUGH RESOURCES TO TRADE WITH BANK");
			}

			return null;
		}
	}

	// Returns a fair trade proposal
	private TradeOffer BuildFairTrade(int currentTurn, PlayerHand cardDifferential)
	{
		int[] resourceRequest = new int[5];
		int[] resourceSurplus = new int[5];

		for(int i = 0; i < 5; i++)
		{
			int cards = cardDifferential.GetResourceQuantity(i);

			if(cards < 0)
			{
				resourceRequest[i] = - cards;
			}
			else
			{
				resourceSurplus[i] = cards;
			}
		}

		TradeOffer trade = new TradeOffer (this, currentTurn, resourceSurplus, resourceRequest);

		trade.EqualizeTrade ();

		return trade;
	}

	// Discards half of a player's hand if their card count is greater than 7
	public void gotRobbed()
	{
		int handCount = hand.GetHandSize();
		if(handCount > 7)
		{
			int numDiscards = handCount / 2;
			for(int i = 0; i < numDiscards; i++)
			{
				hand.discard();
			}
		}
	}
}
