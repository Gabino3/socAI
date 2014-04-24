using UnityEngine;
using System.Collections;

public class TradeOffer : MonoBehaviour
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

	public int[] giveResources;
	public int[] getResources;

	public TradeOffer (Player tradeHost, int currentTurn, int[] giveResources, int[] getResources)
	{
		this.tradeHost = tradeHost;
		this.turnOffered = currentTurn;

		this.giveResources = giveResources;
		this.getResources = getResources;

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
}
