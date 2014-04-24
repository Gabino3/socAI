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

		ExecuteTradeOffer ();
	}

	private void ExecuteTradeOffer()
	{
		// Alert Human Player
		if(tradeHost.isAI)
		{
			if(proposeTradeToPlayer(gamestate.GetPlayerAtIndex(0)))
			{
				// Alerts Human Player to Trade
			}
		}




	}

	private bool proposeTradeToPlayer(Player Player)
	{




		return true;
	}
}
