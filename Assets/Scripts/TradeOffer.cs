using UnityEngine;
using System.Collections;

public class TradeOffer
{
	public Player tradeHost;

	public int turnOffered;

	public int giveBrick;
	public int giveOre;
	public int giveWood;
	public int giveGrain;
	public int giveSheep;

	public int getBrick;
	public int getOre;
	public int getWood;
	public int getGrain;
	public int getSheep;

	bool debugMessages = false;

	// Player Trade
	public TradeOffer (Player tradeHost, int currentTurn, int[] giveResources, int[] getResources)
	{
		this.tradeHost = tradeHost;
		this.turnOffered = currentTurn;

		for(int i = 0; i < 5; i++)
		{
			switch(i)
			{
				case 0:
					giveBrick = giveResources[i];
					getBrick = getResources[i];
					break;
				case 1:
					giveOre = giveResources[i];
					getOre = getResources[i];
					break;
				case 2:
					giveWood = giveResources[i];
					getWood = getResources[i];
					break;
				case 3:
					giveGrain = giveResources[i];
					getGrain = getResources[i];
					break;
				case 4:
					giveSheep = giveResources[i];
					getSheep = getResources[i];
					break;
			}
		}
	}

	// Bank Trade
	public TradeOffer(Player tradeHost, int[] giveResource, int[] getResource)
	{
		for(int i = 0; i < 5; i++)
		{
			switch(i)
			{
			case 0:
				giveBrick = giveResource[i];
				getBrick = getResource[i];
				break;
			case 1:
				giveOre = giveResource[i];
				getOre = getResource[i];
				break;
			case 2:
				giveWood = giveResource[i];
				getWood = getResource[i];
				break;
			case 3:
				giveGrain = giveResource[i];
				getGrain = getResource[i];
				break;
			case 4:
				giveSheep = giveResource[i];
				getSheep = getResource[i];
				break;
			}
		}

		if(debugMessages)
		{
			GameEngine.print ("GET RESOURCES: " + TotalGetResources ());
			GameEngine.print ("GIVE RESOURCES: " + TotalGiveResources ());
		}

		while((TotalGetResources() != TotalGiveResources() / 4))
		{
			if(debugMessages)
			{
				GameEngine.print ("TRADE RATIO: " + TotalGetResources() + ":" + TotalGiveResources());
			}

			if((TotalGetResources() > TotalGiveResources() / 4))
			{
				dropGetCard();
			}

			if((TotalGetResources() < TotalGiveResources() / 4))
			{
				dropGiveCard();
				dropGiveCard();
				dropGiveCard();
				dropGiveCard();
			}
		}

		if(debugMessages)
		{
			GameEngine.print ("TRADE RATIO: " + TotalGetResources() + ":" + TotalGiveResources());
		}
	}

	public int TotalGetResources()
	{
		return getBrick + getOre + getWood + getGrain + getSheep;
	}

	public int TotalGiveResources()
	{
		return giveBrick + giveOre + giveWood + giveGrain + giveSheep;
	}

	public bool IsFairTrade()
	{
		bool fairTrade = Mathf.Abs ((float)(ValueOfGetResources() - ValueOfGiveResources())) <= AIEngine.AverageValue ();

		if(debugMessages)
		{
			GameEngine.print ("GET VALUE: " + ValueOfGetResources());
			GameEngine.print ("GIVE VALUE: " + ValueOfGiveResources());
			GameEngine.print ("___avg val: " + AIEngine.AverageValue ());
			GameEngine.print ("IS FAIR TRADE?: " + fairTrade);
		}

		return fairTrade;
	}

	public bool IsFairTrade(PlayerHand a, PlayerHand b)
	{
		PlayerHand get = a;
		PlayerHand give = b;
		
		bool fairTrade = Mathf.Abs ((float)(get.ValueOfHand () - give.ValueOfHand ())) <= AIEngine.AverageValue ();
		return fairTrade;
	}

	// Equalizes the give and take components of the trade
	public void EqualizeTrade()
	{
		PlayerHand get = convertGetResourcesToPlayerHand ();
		PlayerHand give = convertGiveResourcesToPlayerHand ();

		while(!IsFairTrade(get, give))
		{
			if(get.ValueOfHand() > give.ValueOfHand())
			{
				get.discard();
			}
			else
			{
				give.discard();
			}
		}

		int[] getArray = get.ToArray ();
		int[] giveArray = give.ToArray ();

		getBrick = getArray [0];
		getOre = getArray [1];
		getWood = getArray [2];
		getGrain = getArray [3];
		getSheep = getArray [4];

		giveBrick = giveArray [0];
		giveOre = giveArray [1];
		giveWood = giveArray [2];
		giveGrain = giveArray [3];
		giveSheep = giveArray [4];
	}

	// Has a chance to randomly discard some component from the trade
	public void RandomUnequalizeTrade()
	{
		System.Random rand = new System.Random ();

		int dropGive = rand.Next (100);
		int dropGet = rand.Next (100);

		if(dropGive < 10)
		{
			if(debugMessages)
			{
				GameEngine.print ("DROPPED GIVE CARD");
			}

			dropGiveCard();
		}

		if(dropGet < 5)
		{
			if(debugMessages)
			{
				GameEngine.print ("DROPPED GET CARD");
			}

			dropGetCard();
		}
	}

	public void dropGiveCard()
	{
		PlayerHand give = convertGiveResourcesToPlayerHand();
		
		give.discard();
		
		int[] giveArray = give.ToArray ();
		
		giveBrick = giveArray [0];
		giveOre = giveArray [1];
		giveWood = giveArray [2];
		giveGrain = giveArray [3];
		giveSheep = giveArray [4];
	}

	public void dropGetCard()
	{
		PlayerHand get = convertGetResourcesToPlayerHand();
		
		get.discard();
		
		int[] getArray = get.ToArray ();
		
		getBrick = getArray [0];
		getOre = getArray [1];
		getWood = getArray [2];
		getGrain = getArray [3];
		getSheep = getArray [4];
	}

	public string GenerateTradeKey()
	{
		return getBrick.ToString () + "_" + getOre.ToString () + "_" + getWood.ToString () + "_" + getGrain.ToString () + "_" + getSheep.ToString ();
	}

	public PlayerHand convertGiveResourcesToPlayerHand()
	{
		return new PlayerHand (new int[5] {giveBrick, giveOre, giveWood, giveGrain, giveSheep});
	}

	public PlayerHand convertGetResourcesToPlayerHand()
	{
		return new PlayerHand (new int[5] {getBrick, getOre, getWood, getGrain, getSheep});
	}

	public double ValueOfGetResources()
	{
		return (getBrick * AIEngine.ValueOfBrick() +
		        getOre * AIEngine.ValueOfOre() +
		        getWood * AIEngine.ValueOfWood() +
		        getGrain * AIEngine.ValueOfGrain() +
		        getSheep * AIEngine.ValueOfSheep());
	}

	public double ValueOfGiveResources()
	{
		return (giveBrick * AIEngine.ValueOfBrick() +
		        giveOre * AIEngine.ValueOfOre() +
		        giveWood * AIEngine.ValueOfWood() +
		        giveGrain * AIEngine.ValueOfGrain() +
		        giveSheep * AIEngine.ValueOfSheep());
	}
}
