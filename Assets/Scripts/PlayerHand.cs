using UnityEngine;
using System.Collections;

public class PlayerHand : MonoBehaviour
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
		brick = 5;
		ore = 5;
		wood = 5;
		grain = 5;
		sheep = 5;
		victoryPoints = 0;
		//knights = 0;
		//roadBuilding = 0;
		//yearOfPlenty = 0;
		//monopoly = 0;
		
		//randomizeHand();
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
