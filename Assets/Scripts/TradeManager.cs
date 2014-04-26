using UnityEngine;
using System.Collections;

public class TradeManager : MonoBehaviour
{
	GameState gamestate;

	public TradeManager (GameState gamestate)
	{
		gamestate = gamestate;
	}

	public void ExecuteTradeOfferNotification(TradeOffer offer)
	{
		Player tradeHost = offer.tradeHost;

		bool tradeIsAccepted = false;
		Player tradeWithPlayer = null;

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
			// Nobody Accepts Trade.  Alert Human via GUI
		}
	}

	private bool proposeTradeToPlayer(TradeOffer offer, Player tradeWithPlayer)
	{
		return tradeWithPlayer.processTradeRequest(gamestate, offer);
	}

	private void ExecuteTradeOffer(TradeOffer offer, Player tradeHost, Player tradeWith)
	{
		PlayerHand hostHand = tradeHost.GetPlayerHand();
		PlayerHand withHand = tradeWith.GetPlayerHand();

		exchangeResources(hostHand.brick, withHand.brick, offer.getBrick, offer.giveBrick);
		exchangeResources(hostHand.ore, withHand.ore, offer.getOre, offer.giveOre);
		exchangeResources(hostHand.wood, withHand.wood, offer.getWood, offer.giveWood);
		exchangeResources(hostHand.grain, withHand.grain, offer.getGrain, offer.giveGrain);
		exchangeResources(hostHand.sheep, withHand.sheep, offer.getSheep, offer.giveSheep);
	}

	private void ExecuteTradeWithBank(TradeOffer offer, Player tradeHost)
	{
		PlayerHand hostHand = tradeHost.GetPlayerHand ();
		PlayerHand bankHand = new PlayerHand ();

		exchangeResources(hostHand.brick, bankHand.brick, offer.getBrick, offer.giveBrick);
		exchangeResources(hostHand.ore, bankHand.ore, offer.getOre, offer.giveOre);
		exchangeResources(hostHand.wood, bankHand.wood, offer.getWood, offer.giveWood);
		exchangeResources(hostHand.grain, bankHand.grain, offer.getGrain, offer.giveGrain);
		exchangeResources(hostHand.sheep, bankHand.sheep, offer.getSheep, offer.giveSheep);
	}

	private void exchangeResources(int hostResource, int withResource, int getResource, int giveResource)
	{
		hostResource += getResource;
		hostResource -= giveResource;

		withResource += giveResource;
		withResource -= getResource;
	}
}
