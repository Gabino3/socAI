using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{
	static Player[] playersArray;
	static int numPlayers;
	static int turnCounter;
	static int currentPlayerTurn;

	private static Player longestRoadPlayer;
	private static Player largestArmyPlayer;

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
		longestRoadPlayer = null;	// Requires 5+ consecutive roads to first attain 'Longest Road'
		largestArmyPlayer = null;	// Requires 3+ total knights to first attain 'Largest Army'

		do
		{
			// Play Game




			EndTurn();
		}
		while(!IsGameOver());
	}

	public static Player GetCurrentTurnPlayer()
	{
		return playersArray[currentPlayerTurn];
	}

	public void EndTurn()
	{
		if(GetCurrentTurnPlayer().longestRoad >= 5 && (longestRoadPlayer == null || GetCurrentTurnPlayer().longestRoad > longestRoadPlayer.longestRoad))
		{
			longestRoadPlayer = GetCurrentTurnPlayer();
		}

		if(GetCurrentTurnPlayer().largestArmy >= 3 && (largestArmyPlayer == null || GetCurrentTurnPlayer().largestArmy > largestArmyPlayer.largestArmy))
		{
			largestArmyPlayer = GetCurrentTurnPlayer();
		}
	}

	private bool IsGameOver()
	{
		bool gameOver = false;

		if(GetCurrentTurnPlayer().HasWon())
		{
			// Game Over; player 'getCurrentTurnPlayer' has won
			gameOver = true;
		}
		else
		{
			UpdateGameState();
		}

		return true;
		//return gameOver;
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

	public static bool doesPlayerHaveLongestRoad(Player player)
	{
		return player == longestRoadPlayer;
	}

	public static bool doesPlayerHaveLargestArmy(Player player)
	{
		return player == largestArmyPlayer;
	}
}
