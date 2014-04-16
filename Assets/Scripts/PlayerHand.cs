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
	
	public void randomizeHand()
	{
		System.Random generator = new System.Random();
		
		brick = generator.Next(generator.Next(4));
		ore = generator.Next(generator.Next(4));
		wood = generator.Next(generator.Next(4));
		grain = generator.Next(generator.Next(4));
		sheep = generator.Next(generator.Next(4));
	}
}
