using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{
	public Player[] playersArray;
	public int numPlayers;
	public int turnCounter;
	public int currentPlayerTurn;

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
		while(!IsGameOver());
	}

	public Player GetCurrentTurnPlayer()
	{
		return playersArray[currentPlayerTurn];
	}

	public void EndTurn()
	{
		if(GetCurrentTurnPlayer().longestRoad > globalLongestRoad)
		{
			globalLongestRoad = GetCurrentTurnPlayer().longestRoad;
		}
		
		if(GetCurrentTurnPlayer().largestArmy > globalLargestArmy)
		{
			globalLargestArmy = GetCurrentTurnPlayer().largestArmy;
		}
	}

	private void UpdateGameState()
	{


		turnCounter++;
		UpdatePlayerTurn();
	}

	private void UpdatePlayerTurn()
	{
		currentPlayerTurn = turnCounter % numPlayers;
	}

	private bool IsGameOver()
	{
		if(GetCurrentTurnPlayer().HasWon())
		{
			// Game Over; player 'getCurrentTurnPlayer' has won
		}
		else
		{

		}

		return true;
	}
}
