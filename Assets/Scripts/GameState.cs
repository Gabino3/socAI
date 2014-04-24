using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState
{
	//Debug variables
	private bool diceDebug = false;
	private bool stateDebug = true;
	private bool objectiveDebug = true;

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

	private int largestArmy;
	private int longestRoad;
	private Player largestArmyPlayer;
	private Player longestRoadPlayer;

	public GameState(int numPlayers, Board board)
	{
		this.numPlayers = numPlayers;
		this.board = board;
		bool start = false;

		playersArray = new Player[numPlayers];
		for (int i = 0; i < numPlayers; i++)
		{
			playersArray[i] = new Player(i, i!=0);
		}

		roll = 0;
		SetState (State.unstarted);
		turnCounter = -1;
		currentPlayerTurn = 0;
		largestArmy = 2; //Requires 3+ total knights to first attain 'Largest Army'
		longestRoad = 4; //Requires 5+ consecutive roads to first attain 'Longest Road'
		largestArmyPlayer = null;
		longestRoadPlayer = null;
	}

	/*
	 * Has each player collect resources from owned tiles based on dice roll.
	 */
	public void CollectPlayerResources(int diceRoll)
	{
		for (int i = 0; i < numPlayers; i++) {
			playersArray[i].CollectResourcesFromRoll(board, diceRoll);
		}
	}

	public void CollectSecondPlacementResources()
	{
		for (int i = 0; i < numPlayers; i++) {
			playersArray[i].CollectResourcesFromStructure(board, 1); // second structure
		}
	}

	private void DetermineCurrentPlayerObjectives()
	{
		int playerLongestRoad = Board.LongestRoadOfPlayer (GetCurrentTurnPlayer());

		//Assign longest road player
		if (playerLongestRoad > longestRoad) {
			longestRoad = playerLongestRoad;
			longestRoadPlayer = GetCurrentTurnPlayer();
		}

		if (objectiveDebug) {
			GameEngine.print ("PLAYER " + GetCurrentTurnPlayer ().id + " HAS ROAD OF " + playerLongestRoad + " LENGTH.");
			if (longestRoadPlayer != null) {
				GameEngine.print ("LONGEST ROAD PLAYER: " + longestRoadPlayer.id);
			} else {
				GameEngine.print ("LONGEST ROAD PLAYER: null");
			}
		}

		//TODO largest army?
	}

	public Player GetCurrentTurnPlayer()
	{
		return playersArray[currentPlayerTurn];
	}

	public int GetRoll()
	{
		return roll;
	}

	public Player GetPlayerAtIndex(int index)
	{
		return playersArray[index];
	}

	public bool HasLargestArmy(Player player)
	{
		return player == largestArmyPlayer;
	}

	public bool HasLongestRoad(Player player)
	{
		return player == longestRoadPlayer;
	}

	public void IncrementPlayer()
	{
		turnCounter++;
		SetCurrentPlayerTurn (turnCounter % numPlayers);
	}

	/*
	 * Handles gameplay FSM.
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
				DetermineCurrentPlayerObjectives();
				if (IsGameOver ()) {
					GameEngine.print ("HOLY SHIT WE FINISHED A GAME!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
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
		bool hasLargestArmy = (GetCurrentTurnPlayer() == largestArmyPlayer);
		bool hasLongestRoad = (GetCurrentTurnPlayer() == longestRoadPlayer);

		if(GetCurrentTurnPlayer().HasWon(hasLargestArmy, hasLongestRoad))
		{
			//Game Over; player 'getCurrentTurnPlayer' has won
			gameOver = true;
		}

		return gameOver;
	}

	public int RollDice()
	{
		int roll = rand.Next (6) + rand.Next (6) + 2;

		if (diceDebug) {
			string timestamp = System.DateTime.Now.ToString ("yyyy/MM/dd HH:mm:ss:ffff");
			GameEngine.print ("[" + timestamp + "] DICE ROLL: " + roll);
		}

		return roll;
	}

	private void SetCurrentPlayerTurn(int currentPlayerTurn)
	{
		if (stateDebug)
		{
			string timestamp = System.DateTime.Now.ToString ("yyyy/MM/dd HH:mm:ss:ffff");
			GameEngine.print ("[" + timestamp + "] CURRENT PLAYER TURN: " + currentPlayerTurn);
		}

		this.currentPlayerTurn = currentPlayerTurn;
	}

	private State SetState(State state)
	{
		if (stateDebug)
		{
			string stateName = "";
			string timestamp = System.DateTime.Now.ToString ("yyyy/MM/dd HH:mm:ss:ffff");
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
			GameEngine.print ("[" + timestamp + "] PLAYER " + GetCurrentTurnPlayer().id + " -> STATE CHANGE: " + stateName);
		}

		curState = state;
		return curState;
	}
}
