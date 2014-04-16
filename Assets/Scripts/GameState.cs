using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{
	Player[] playersArray;
	int numPlayers;
	int turnCounter;
	int currentPlayerTurn;

	private int globalLongestRoad;
	private int globalLargestArmy;

	public GameState(int playerCount)
	{
		numPlayers = playerCount;

		playersArray = new Player[numPlayers];
		for (int i = 0; i < playerCount; i++)
		{
			playersArray[i] = new Player(i, i!=0);
		}

		turnCounter = 0;
		currentPlayerTurn = 0;
		globalLongestRoad = 4;	// Requires 5+ consecutive roads to first attain 'Longest Road'
		globalLargestArmy = 2;	// Requires 3+ total knights to first attain 'Largest Army'

		do
		{
			// Play Game



		}
		while(!isGameOver);
	}

	public Player getCurrentTurnPlayer()
	{
		return playersArray[currentPlayerTurn];
	}

	public void endTurn()
	{
		if(getCurrentTurnPlayer().longestRoad > globalLongestRoad)
		{
			globalLongestRoad = getCurrentTurnPlayer().longestRoad;
		}
		
		if(getCurrentTurnPlayer().largestArmy > globalLargestArmy)
		{
			globalLargestArmy = getCurrentTurnPlayer().largestArmy;
		}
	}

	private void updateGameState()
	{


		turnCounter++;
		updatePlayerTurn();
	}

	private void updatePlayerTurn()
	{
		currentPlayerTurn = turnCounter % numPlayers;
	}

	private bool isGameOver()
	{
		if(getCurrentTurnPlayer().hasWon)
		{
			// Game Over; player 'getCurrentTurnPlayer' has won
		}
		else
		{

		}
	}
}
