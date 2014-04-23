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
	
	public PlayerHand()
	{
		brick = 0;
		ore = 0;
		wood = 0;
		grain = 0;
		sheep = 0;
		victoryPoints = 0;
		//knights = 0;
		//roadBuilding = 0;
		//yearOfPlenty = 0;
		//monopoly = 0;
		
		//randomizeHand();
	}

	public int getResourceAmount(int index){
		switch (index) {
		case 0: return brick;
		case 1: return ore;
		case 2: return wood;
		case 3: return grain;
		case 4: return sheep;
		default: return -1;
		}
	}
	
	public void randomize()
	{
		System.Random rand = new System.Random();
		
		brick = rand.Next(4);
		ore = rand.Next(4);
		wood = rand.Next(4);
		grain = rand.Next(4);
		sheep = rand.Next(4);
	}

	public int totalResources()
	{
		return brick + ore + wood + grain + sheep;
	}

}
