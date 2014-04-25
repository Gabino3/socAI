using UnityEngine;
using System.Collections;

public class PlayerHand
{
	public int brick;
	public int ore;
	public int wood;
	public int grain;
	public int sheep;
	public int victoryPoints;
	//int knights;
	//int roadBuilding;
	//int yearOfPlenty;
	//int monopoly;

	public PlayerHand() : this(0)
	{
	}

	public PlayerHand(int amount)
	{
		brick = amount;
		ore = amount;
		wood = amount;
		grain = amount;
		sheep = amount;
		victoryPoints = 0;
		//knights = 0;
		//roadBuilding = 0;
		//yearOfPlenty = 0;
		//monopoly = 0;
		
		//randomizeHand();
	}

	public int GetResourceQuantity(int index)
	{
		switch (index)
		{
			case 0: return brick;
			case 1: return ore;
			case 2: return wood;
			case 3: return grain;
			case 4: return sheep;
			default: return -1;
		}
	}
	
	public void Randomize(int max)
	{
		System.Random rand = new System.Random();
		
		brick = rand.Next(max);
		ore = rand.Next(max);
		wood = rand.Next(max);
		grain = rand.Next(max);
		sheep = rand.Next(max);
	}

	public void SetResourceQuantity(int index, int value)
	{
		switch (index)
		{
		case 0: brick = value; break;
		case 1: ore = value; break;
		case 2: wood = value; break;
		case 3: grain = value; break;
		case 4: sheep = value; break;
		default: break;
		}
	}

	public int TotalResources()
	{
		return brick + ore + wood + grain + sheep;
	}

	public bool isViableTradeRequest(TradeOffer trade)
	{
		return brick >= trade.giveBrick && ore >= trade.giveOre && wood >= trade.giveWood && grain >= trade.giveGrain && sheep >= trade.giveSheep;
	}
}
