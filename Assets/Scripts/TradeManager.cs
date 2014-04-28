using UnityEngine;
using System.Collections;

public class TradeManager
{
	GameState gamestate;
	GameEngine gameengine;
	int numProposedTrades;
	int numSuccessfulTrades;

	bool debugMessages = false;

	public TradeManager (GameState gamestate, GameEngine ge)
	{
		this.gamestate = gamestate;
		gameengine = ge;
		this.numProposedTrades = 0;
		this.numSuccessfulTrades = 0;
	}

	public bool ExecuteTradeOfferNotification(TradeOffer offer)
	{
		numProposedTrades++;

		Player tradeHost = offer.tradeHost;

		bool tradeIsAccepted = false;
		Player tradeWithPlayer = null;

		for(int i = 0; i < gamestate.numPlayers && !tradeIsAccepted; i++)
		{
			tradeWithPlayer = gamestate.GetPlayerAtIndex(i);
			if(tradeHost != tradeWithPlayer)
			{
				if(!tradeWithPlayer.isAI)
				{
					// Alert Human Player to Trade via GUI
					gameengine.ShowHideTrade(false, offer);
					tradeIsAccepted = proposeTradeToPlayer(offer, tradeWithPlayer);
					gameengine.ShowHideTrade(false, offer);
				}
				else
				{
					tradeIsAccepted = proposeTradeToPlayer(offer, tradeWithPlayer);
				}
			}
		}

		if(tradeIsAccepted)
		{
			numSuccessfulTrades++;
			ExecuteTradeOffer(offer, tradeHost, tradeWithPlayer);

			if(debugMessages)
			{
				GameEngine.print("PLAYER " + tradeHost.id + " TRADES WITH PLAYER " + tradeWithPlayer.id);
				GameEngine.print("PLAYER " + tradeHost.id + " TRADES:\n" +
				                 offer.giveBrick + " Brick\n" +
				                 offer.giveOre + " Ore\n" +
				                 offer.giveWood + " Wood\n" +
				                 offer.giveGrain + " Grain\n" +
				                 offer.giveSheep + " Sheep");
				GameEngine.print ("FOR:\n" +
				                  offer.getBrick + " Brick\n" +
				                  offer.getOre + " Ore\n" +
				                  offer.getWood + " Wood\n" +
				                  offer.getGrain + " Grain\n" +
				                  offer.getSheep + " Sheep");
			}
		}
		else
		{
			// Nobody Accepts Trade.  Alert Human via GUI
		}

		if (debugMessages)
		{
			GameEngine.print ("CUMULATIVE PROPOSED TRADES: " + numProposedTrades +
			                  "\nTOTAL SUCCESSFUL TRADES: " + numSuccessfulTrades);
		}

		return tradeIsAccepted;
	}

	private bool proposeTradeToPlayer(TradeOffer offer, Player tradeWithPlayer)
	{
		return tradeWithPlayer.processTradeRequest(gamestate, offer);
	}

	private void ExecuteTradeOffer(TradeOffer offer, Player tradeHost, Player tradeWith)
	{
		PlayerHand hostHand = tradeHost.GetPlayerHand();
		PlayerHand withHand = tradeWith.GetPlayerHand();

		if (debugMessages)
		{
			GameEngine.print ("HOST TRADES:\n" +
			                  offer.giveBrick + "|" + offer.giveOre + "|" + offer.giveWood + "|" + offer.giveGrain + "|" + offer.giveSheep); 
			GameEngine.print ("FOR:\n" +
			                  offer.getBrick + "|" + offer.getOre + "|" + offer.getWood + "|" + offer.getGrain + "|" + offer.getSheep);
		}

		if(debugMessages)
		{
			GameEngine.print ("HOSTHAND BEFORE:\n" +
			                  hostHand.brick + " BRICK\n" +
			                  hostHand.ore + " ORE\n" +
			                  hostHand.wood + " WOOD\n" +
			                  hostHand.grain + " GRAIN\n" +
			                  hostHand.sheep + " SHEEP");
			
			GameEngine.print ("SELLER BEFORE:\n" +
			                  withHand.brick + " BRICK\n" +
			                  withHand.ore + " ORE\n" +
			                  withHand.wood + " WOOD\n" +
			                  withHand.grain + " GRAIN\n" +
			                  withHand.sheep + " SHEEP");
		}

		performResourceExchangeBetweenPlayers(hostHand, withHand, offer);

		if(debugMessages)
		{
			GameEngine.print ("HOSTHAND AFTER:\n" +
			                  hostHand.brick + " BRICK\n" +
			                  hostHand.ore + " ORE\n" +
			                  hostHand.wood + " WOOD\n" +
			                  hostHand.grain + " GRAIN\n" +
			                  hostHand.sheep + " SHEEP");
			
			GameEngine.print ("SELLER AFTER:\n" +
			                  withHand.brick + " BRICK\n" +
			                  withHand.ore + " ORE\n" +
			                  withHand.wood + " WOOD\n" +
			                  withHand.grain + " GRAIN\n" +
			                  withHand.sheep + " SHEEP");
		}
	}

	public void ExecuteTradeWithBank(TradeOffer offer, Player tradeHost)
	{
		PlayerHand hostHand = tradeHost.GetPlayerHand ();
		PlayerHand bankHand = new PlayerHand ();

		if (debugMessages)
		{
			GameEngine.print ("HOST TRADES:\n" +
			                  offer.giveBrick + "|" + offer.giveOre + "|" + offer.giveWood + "|" + offer.giveGrain + "|" + offer.giveSheep); 
			GameEngine.print ("FOR:\n" +
			                  offer.getBrick + "|" + offer.getOre + "|" + offer.getWood + "|" + offer.getGrain + "|" + offer.getSheep);
		}

		if(debugMessages)
		{
			GameEngine.print ("HOSTHAND BEFORE:\n" +
			                  hostHand.brick + " BRICK\n" +
			                  hostHand.ore + " ORE\n" +
			                  hostHand.wood + " WOOD\n" +
			                  hostHand.grain + " GRAIN\n" +
			                  hostHand.sheep + " SHEEP");

			GameEngine.print ("BANK BEFORE:\n" +
			                  bankHand.brick + " BRICK\n" +
			                  bankHand.ore + " ORE\n" +
			                  bankHand.wood + " WOOD\n" +
			                  bankHand.grain + " GRAIN\n" +
			                  bankHand.sheep + " SHEEP");
		}

		performResourceExchangeBetweenPlayers(hostHand, bankHand, offer);

		if(debugMessages)
		{
			GameEngine.print ("HOSTHAND AFTER:\n" +
			                  hostHand.brick + " BRICK\n" +
			                  hostHand.ore + " ORE\n" +
			                  hostHand.wood + " WOOD\n" +
			                  hostHand.grain + " GRAIN\n" +
			                  hostHand.sheep + " SHEEP");
			
			GameEngine.print ("BANK AFTER:\n" +
			                  bankHand.brick + " BRICK\n" +
			                  bankHand.ore + " ORE\n" +
			                  bankHand.wood + " WOOD\n" +
			                  bankHand.grain + " GRAIN\n" +
			                  bankHand.sheep + " SHEEP");
		}
	}

	private void performResourceExchangeBetweenPlayers(PlayerHand hostHand, PlayerHand withHand, TradeOffer offer)
	{
		hostHand.brick = hostHand.brick + offer.getBrick - offer.giveBrick;
		withHand.brick = withHand.brick - offer.getBrick + offer.giveBrick;

		hostHand.ore = hostHand.ore + offer.getOre - offer.giveOre;
		withHand.ore = withHand.ore - offer.getOre + offer.giveOre;

		hostHand.wood = hostHand.wood + offer.getWood - offer.giveWood;
		withHand.wood = withHand.wood - offer.getWood + offer.giveWood;

		hostHand.grain = hostHand.grain + offer.getGrain - offer.giveGrain;
		withHand.grain = withHand.grain - offer.getGrain + offer.giveGrain;

		hostHand.sheep = hostHand.sheep + offer.getSheep - offer.giveSheep;
		withHand.sheep = withHand.sheep - offer.getSheep + offer.giveSheep;
	}
	
	private void exchangeResources(int hostResource, int withResource, int getResource, int giveResource)
	{
		hostResource += getResource;
		hostResource -= giveResource;

		withResource += giveResource;
		withResource -= getResource;
	}
}
