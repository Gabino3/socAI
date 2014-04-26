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

	public PlayerHand(int[] resources)
	{
		brick = resources[0];
		ore = resources[1];
		wood = resources[2];
		grain = resources[3];
		sheep = resources[4];
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

	public int GetHandSize()
	{
		return brick + ore + wood + grain + sheep;
	}

	public bool isViableTradeRequest(TradeOffer trade)
	{
		return brick >= trade.giveBrick && ore >= trade.giveOre && wood >= trade.giveWood && grain >= trade.giveGrain && sheep >= trade.giveSheep;
	}

	public PlayerHand MakeHandDifferentialFromRequiredResources(int needBrick, int needOre, int needWood, int needGrain, int needSheep)
	{
		return new PlayerHand (new int[5] {this.brick - needBrick, this.ore - needOre, this.wood - needWood, this.grain - needGrain, this.sheep - needSheep});
	}

	public void discard()
	{
		System.Random rand = new System.Random ();
		int randDiscard = rand.Next (GetHandSize());

		if(randDiscard - brick < 0)
		{
			brick--;
		}
		else if(randDiscard - brick - ore < 0)
		{
			ore--;
		}
		else if(randDiscard - brick - ore - wood < 0)
		{
			wood--;
		}
		else if(randDiscard - brick - ore - wood - grain < 0)
		{
			grain--;
		}
		else if(randDiscard - brick - ore - wood - grain - sheep < 0)
		{
			sheep--;
		}
	}

	public int[] GetArrayOfPlayerHand()
	{
		return new int[5] {brick, ore, wood, grain, sheep};
	}

	public double HandValueOfBrick()
	{
		return brick * AIEngine.ValueOfBrick ();
	}

	public double HandValueOfOre()
	{
		return ore * AIEngine.ValueOfOre ();
	}

	public double HandValueOfWood()
	{
		return wood * AIEngine.ValueOfWood ();
	}

	public double HandValueOfGrain()
	{
		return grain * AIEngine.ValueOfGrain ();
	}

	public double HandValueOfSheep()
	{
		return sheep * AIEngine.ValueOfSheep ();
	}

	public double ValueOfHand()
	{
		return HandValueOfBrick () + HandValueOfOre () + HandValueOfWood () + HandValueOfGrain () + HandValueOfSheep ();
	}
	
	public bool handsOverlap(PlayerHand hand)
	{
		for(int i = 0; i < 5; i++)
		{
			if(this.GetResourceQuantity(i) < 0 && hand.GetResourceQuantity(i) > 0)
			{
				return true;
			}
		}

		return false;
	}
}
