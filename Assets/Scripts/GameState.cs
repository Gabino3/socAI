using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
	Player[] playersArray;
	int numPlayers;
	int turnCounter;
	int currentPlayerTurn;
	Board board;

	public enum State {
		placeSettlement, placeRoad, roll, robber, trade, place, end, failure
	};
	public State curState;

	private static System.Random rand = new System.Random();
	private static Player longestRoadPlayer;
	private static Player largestArmyPlayer;

	public GameState(int numPlayers, Board board)
	{
		this.numPlayers = numPlayers;
		this.board = board;
		bool start = false;

		playersArray = new Player[numPlayers];
		for (int i = 0; i < numPlayers; i++) {
			playersArray[i] = new Player(i, i!=0);
		}

		curState = State.placeSettlement;
		turnCounter = -1;
		currentPlayerTurn = 0;
		longestRoadPlayer = null;	// Requires 5+ consecutive roads to first attain 'Longest Road'
		largestArmyPlayer = null;	// Requires 3+ total knights to first attain 'Largest Army'
	}

	/*
	 * Has each player collect resources from owned tiles based on dice roll.
	 */
	public void CollectPlayerResources(int diceRoll)
	{
		for (int i = 0; i < numPlayers; i++) {
			playersArray[i].CollectResources(diceRoll);
		}
	}

	private void DetermineObjectivesOwnership()
	{
		if(GetCurrentTurnPlayer().longestRoad >= 5 && (longestRoadPlayer == null || GetCurrentTurnPlayer().longestRoad > longestRoadPlayer.longestRoad)) {
			longestRoadPlayer = GetCurrentTurnPlayer();
		}

		if(GetCurrentTurnPlayer().largestArmy >= 3 && (largestArmyPlayer == null || GetCurrentTurnPlayer().largestArmy > largestArmyPlayer.largestArmy)) {
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

	public void IncrementPlayer()
	{
		currentPlayerTurn = turnCounter % numPlayers;
	}

	/*
	 * Main game logic.
	 */
	public State IncrementState()
	{
		turnCounter++;

		//Setup
		if (turnCounter < numPlayers*2) {

			if (curState == State.placeSettlement) {
				return State.placeRoad;

			} else if (curState == State.placeRoad) {
				// 0, 1, 2, 3
				if (turnCounter < numPlayers) {
					currentPlayerTurn = turnCounter;
					return State.placeSettlement;
				
				// 4, 5, 6
				} else if (turnCounter >= numPlayers && turnCounter < numPlayers*2 - 1) {
					currentPlayerTurn = (numPlayers - 1) - turnCounter % numPlayers;
					return State.placeSettlement;
				
				// 7
				} else {
					currentPlayerTurn = 0;
					return State.roll;
				}
			}

		//Main game
		} else {

			if (curState == State.roll) {
				int roll = RollDice ();

				if (roll == 7) {
					return State.robber;
				} else {
					return State.trade;
				}
			
			} else if (curState == State.robber) {
				return State.trade;

			} else if (curState == State.trade) {
				return State.place;

			} else if (curState == State.place) {
				DetermineObjectivesOwnership();
				if (IsGameOver ()) {
					return State.end;
				}
				IncrementPlayer ();
				return State.roll;

			}
		}

		return State.failure;
	}

	private bool IsGameOver()
	{
		bool gameOver = false;
		
		if(GetCurrentTurnPlayer().HasWon()) {
			//Game Over; player 'getCurrentTurnPlayer' has won
			gameOver = true;
		}

		return gameOver;
	}
	
	public int RollDice()
	{
		int roll = rand.Next (6) + rand.Next (6) + 2;
		string timestamp = System.DateTime.Now.ToString ("yyyy/MM/dd HH:mm:ss:ffff");

		print ("[" + timestamp + "] DICE ROLL: " + roll);

		return roll;
	}

//	private void UpdateGameState()
//	{
//		turnCounter++;
//		UpdatePlayerTurn();
//	}

//	private void UpdatePlayerTurn()
//	{
//		currentPlayerTurn = turnCounter % numPlayers;
//	}
}
