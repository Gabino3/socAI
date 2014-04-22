using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState
{
	//Debug variables
	public bool debug = true;

	Player[] playersArray;
	int numPlayers;
	int turnCounter;
	int currentPlayerTurn;
	Board board;
	int roll;

	public enum State {
		unstarted, placeSettlement, placeRoad, roll, robber, trade, place, end, failure
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

		roll = 0;
		SetState (State.unstarted);
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
			playersArray[i].CollectResourcesFromRoll(diceRoll);
		}
	}

	public void CollectSecondPlacementResources()
	{
		for (int i = 0; i < numPlayers; i++) {
			playersArray[i].CollectResourcesFromStructure (1); // second structure
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

	public int getRoll(){
		return roll;
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
		turnCounter++;
		SetCurrentPlayerTurn (turnCounter % numPlayers);
	}

	/*
	 * Main game logic.
	 */
	public State IncrementState()
	{
		if (curState == State.unstarted) {
			turnCounter = 0;
			return SetState (State.placeSettlement);
		}

		//Setup
		if (turnCounter < numPlayers*2) {
			if (curState == State.placeSettlement) {
				return SetState(State.placeRoad);
			}
			else if (curState == State.placeRoad) {
				turnCounter++;

				if (turnCounter < numPlayers) {
					SetCurrentPlayerTurn (turnCounter);
					return SetState(State.placeSettlement);
				}
				else if (turnCounter >= numPlayers && turnCounter < numPlayers*2) {
					SetCurrentPlayerTurn ((numPlayers - 1) - turnCounter % numPlayers);
					return SetState(State.placeSettlement);
				}
				else {
					SetCurrentPlayerTurn (0);
					CollectSecondPlacementResources();
					return SetState(State.roll);
				}
			}
		}
		//Main game
		else {
			if (curState == State.roll) {
				roll = RollDice ();
				if (roll == 7) {
					return SetState(State.robber);
				}
				else {
					CollectPlayerResources (roll);
					return SetState(State.trade);
				}
			}
			else if (curState == State.robber) {
				return SetState(State.trade);
			}
			else if (curState == State.trade) {
				return SetState(State.place);
			}
			else if (curState == State.place) {
				DetermineObjectivesOwnership();
				if (IsGameOver ()) {
					return SetState(State.end);
				}
				IncrementPlayer ();
				return SetState(State.roll);
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
		GameEngine.print ("[" + timestamp + "] DICE ROLL: " + roll);

		return roll;
	}

	private void SetCurrentPlayerTurn(int currentPlayerTurn)
	{
		if (debug) {
			string timestamp = System.DateTime.Now.ToString ("yyyy/MM/dd HH:mm:ss:ffff");
			GameEngine.print ("[" + timestamp + "] CURRENT PLAYER TURN: " + currentPlayerTurn);
		}

		this.currentPlayerTurn = currentPlayerTurn;
	}

	private State SetState(State state)
	{
		if (debug) {
			string stateName = "";
			string timestamp = System.DateTime.Now.ToString ("yyyy/MM/dd HH:mm:ss:ffff");
			//TODO print new state here
			switch (state) {
			case State.end: 			stateName = "end"; break;
			case State.place: 			stateName = "place"; break;
			case State.placeRoad: 		stateName = "placeRoad"; break;
			case State.placeSettlement: stateName = "placeSettlement"; break;
			case State.robber: 			stateName = "robber"; break;
			case State.roll: 			stateName = "roll"; break;
			case State.trade: 			stateName = "trade"; break;
			case State.unstarted:		stateName = "unstarted"; break;
			default:					stateName = "failure/unknown"; break;
			}
			GameEngine.print ("[" + timestamp + "] STATE CHANGE: " + stateName);
		}

		curState = state;
		return curState;
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
