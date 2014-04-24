using UnityEngine;
using System.Collections;

public class TradeManager : MonoBehaviour
{
	Player tradeHost;
	GameState gamestate;

	public TradeManager (TradeOffer offer, GameState gamestate)
	{
		tradeHost = offer.tradeHost;
		gamestate = gamestate;

		ExecuteTradeOffer (offer);
	}

	private void ExecuteTradeOffer(TradeOffer offer)
	{
		bool tradeIsAccepted = false;
		Player tradeWithPlayer = gamestate.GetPlayerAtIndex(0);

		for(int i = 0; i < gamestate.numPlayers && !tradeIsAccepted; i++)
		{
			tradeWithPlayer = gamestate.GetPlayerAtIndex(i);
			if(tradeHost != tradeWithPlayer)
			{
				if(!tradeHost.isAI)
				{
					// Alert Human Player to Trade via GUI
					tradeIsAccepted = proposeTradeToPlayer(offer, tradeWithPlayer);
				}
				else
				{
					tradeIsAccepted = proposeTradeToPlayer(offer, tradeWithPlayer);
				}
			}
		}

		if(tradeIsAccepted)
		{
			ExecuteTradeOffer(offer, tradeHost, tradeWithPlayer);
		}
		else
		{
			// Prevent repeat trades
		}
	}

	private bool proposeTradeToPlayer(TradeOffer offer, Player tradeWithPlayer)
	{
		return tradeWithPlayer.acceptTradeRequest(offer);
	}

	public void ExecuteTradeOffer(TradeOffer offer, Player tradeHost, Player tradeWith)
	{
		PlayerHand hostHand = tradeHost.GetPlayerHand();
		PlayerHand withHand = tradeWith.GetPlayerHand();

		exchangeResources(hostHand.brick, withHand.brick, offer.getBrick, offer.giveBrick);
		exchangeResources(hostHand.ore, withHand.ore, offer.getOre, offer.giveOre);
		exchangeResources(hostHand.wood, withHand.wood, offer.getWood, offer.giveWood);
		exchangeResources(hostHand.grain, withHand.grain, offer.getGrain, offer.giveGrain);
		exchangeResources(hostHand.sheep, withHand.sheep, offer.getSheep, offer.giveSheep);
	}

	public void exchangeResources(int hostResource, int withResource, int getResource, int giveResource)
	{
		hostResource += getResource;
		hostResource -= giveResource;

		withResource += giveResource;
		withResource -= getResource;
	}
}
