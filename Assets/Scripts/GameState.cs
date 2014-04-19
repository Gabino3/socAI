using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{
	Player[] playersArray;
	int numPlayers;
	int turnCounter;
	int currentPlayerTurn;

	private static System.Random rand = new System.Random();
	private static Player longestRoadPlayer;
	private static Player largestArmyPlayer;

	public GameState(int numPlayers)
	{
		this.numPlayers = numPlayers;

		playersArray = new Player[numPlayers];
		for (int i = 0; i < numPlayers; i++)
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

			for (int i = 0; i < 20; i++) {
				RollDice ();
			}

			EndTurn();
		}
		while(!IsGameOver());
	}

	private void EndTurn()
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

	public Player GetCurrentTurnPlayer()
	{
		return playersArray[currentPlayerTurn];
	}

	public Player GetPlayerAtIndex(int index)
	{
		return playersArray[index];
	}

	public static bool HasLargestArmy(Player player)
	{
		return player == largestArmyPlayer;
	}

	public static bool HasLongestRoad(Player player)
	{
		return player == longestRoadPlayer;
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

	public int RollDice()
	{
		int roll = rand.Next (6) + rand.Next (6) + 2;
		print ("DICE ROLL: " + roll);

		return roll;
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
}
