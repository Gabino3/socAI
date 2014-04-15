using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{
	Player[] playersArray;
	int numPlayers;
	int turnCounter;

	public GameState(int playerCount)
	{
		numPlayers = playerCount;
		turnCounter = 0;

		playersArray = new Player[numPlayers];
		for (int i = 0; i < playerCount; i++)
		{
			playersArray[i] = new Player(i);
		}
	}

	private int playerTurn()
	{
		return turnCounter % numPlayers;
	}

	public void endTurn()
	{
		turnCounter++;
	}
}
