using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameEngine : MonoBehaviour
{
	//Debug variables
	private bool rollForAI = false;
	private bool interactDebug = false;

	Board board;
	GameState gamestate;
	TradeManager tradeManager;
	GameObject citySelector;
	GameObject settlementSelector;
	GameObject roadSelector;
	Transform objectToBuild = null;

	Node lastStructurePlaced = null;
	Edge lastRoadPlaced = null;

	GameState.State curState;

	GameObject[] tradeThisText;
	GameObject[] forThisText;
	GameObject[] tradeThis;
	GameObject[] forThis;

	GameObject tradeButton;
	GameObject endTurnButton;

	GameObject dice;
	GameObject diceNum;

	GameObject[] humanCardCounts;
	DateTime lastAIActionTime; //used to slow down AI players
	readonly DateTime EPOCH = new DateTime(2001, 1, 1);
	readonly float FORCED_TIME_BETWEEN_AI_ACTIONS = 0.0f;

	void Start () {
		board = new Board (); // instantiates and draws
		gamestate = new GameState (4, board);
		tradeManager = new TradeManager (gamestate);
		humanCardCounts = new GameObject[5];
		tradeThisText = new GameObject[5];
		forThisText = new GameObject[5];
		tradeThis = new GameObject[5];
		forThis = new GameObject[5];

		BuildPlayerTurnWindow ();
		BuildSelectionPanel ();
		BuildHumanPlayerHandDisplay ();
		BuildEndTurnButton ();
		BuildDiceRoller ();
		BuildTradePanel ();

		lastAIActionTime = EPOCH;
		curState = gamestate.IncrementState (); // initial increment
	}

	private void BuildDiceRoller() 
	{
		dice = Instantiate (Resources.Load ("dice"), new Vector3 (-4f, 6f, 1), Quaternion.identity) as GameObject;
		diceNum = Instantiate (Resources.Load ("text"), new Vector3 (-2f, 6f, 1f), Quaternion.identity) as GameObject;
		diceNum.GetComponent<TextMesh> ().text = "" ;
	}

	private void BuildEndTurnButton()
	{
		endTurnButton = Instantiate (Resources.Load ("backing"), new Vector3 (6f,0f,1f), Quaternion.identity) as GameObject;
		endTurnButton.transform.localScale = new Vector3 (3f,1f,1f);
		GameObject temp = Instantiate (Resources.Load ("text"), new Vector3 (5.2f,0.25f,-1f), Quaternion.identity) as GameObject;
		temp.GetComponent<TextMesh> ().text = "End Turn" ;
	}

	private void BuildHumanPlayerHandDisplay()
	{
		Instantiate (Resources.Load ("card_brick"), new Vector3 (12.5844f, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_ore"), new Vector3 (14.1115f, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_wood"), new Vector3 (15.6386f, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_grain"), new Vector3 (17.1657f, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_sheep"), new Vector3 (18.6928f, 0.1522f, 1), Quaternion.identity);

		humanCardCounts[0] = Instantiate (Resources.Load ("text"), new Vector3 (12.4493f, 1.908892f, -4f), Quaternion.identity) as GameObject;
		humanCardCounts[1] = Instantiate (Resources.Load ("text"), new Vector3 (13.9764f, 1.908892f, -4f), Quaternion.identity) as GameObject;
		humanCardCounts[2] = Instantiate (Resources.Load ("text"), new Vector3 (15.5035f, 1.908892f, -4f), Quaternion.identity) as GameObject;
		humanCardCounts[3] = Instantiate (Resources.Load ("text"), new Vector3 (17.0306f, 1.908892f, -4f), Quaternion.identity) as GameObject;
		humanCardCounts[4] = Instantiate (Resources.Load ("text"), new Vector3 (18.5577f, 1.908892f, -4f), Quaternion.identity) as GameObject;

		UpdateHumanCardCounts ();
	}

	private void BuildPlayerTurnWindow()
	{
		Instantiate(Resources.Load("PlayerOrder"), new Vector3(-3, 12, 0), Quaternion.identity);
	}

	private void BuildSelectionPanel()
	{
		Instantiate(Resources.Load("backing"), new Vector3(-3.598929f,1f,1f), Quaternion.identity);
		
		citySelector = Instantiate(Resources.Load("city"), new Vector3(-5f,2f,-1f), Quaternion.identity) as GameObject;
		GameObject temp = Instantiate (Resources.Load ("text"), new Vector3 (-5.4f,3f,-1f), Quaternion.identity) as GameObject;
		temp.GetComponent<TextMesh> ().text = "City" ;
		citySelector.renderer.material.color = Color.red;
		citySelector.transform.localScale += citySelector.transform.localScale;
		temp = Instantiate (Resources.Load ("text"), new Vector3 (-3f,.95f,-1f), Quaternion.identity) as GameObject;
		temp.GetComponent<TextMesh> ().text = "Road" ;
		roadSelector = Instantiate(Resources.Load("road"), new Vector3(-2.5f,0f,-1f), Quaternion.identity) as GameObject;
		roadSelector.renderer.material.color = Color.red;
		roadSelector.transform.localScale += roadSelector.transform.localScale;
		temp = Instantiate (Resources.Load ("text"), new Vector3 (-3.6f,3f,-1f), Quaternion.identity) as GameObject;
		temp.GetComponent<TextMesh> ().text = "Settlement" ;
		settlementSelector = Instantiate(Resources.Load("settlement"), new Vector3(-2.5f,2f,-1f), Quaternion.identity) as GameObject;
		settlementSelector.renderer.material.color = Color.red;
		settlementSelector.transform.localScale += settlementSelector.transform.localScale;
	}

	private void BuildTradePanel()
	{
		//Trading this (what you will give)
		tradeThis[0] =Instantiate (Resources.Load ("card_brick"), new Vector3 (12.5844f, 12f, 1), Quaternion.identity) as GameObject;
		tradeThis[1] =Instantiate (Resources.Load ("card_ore"), new Vector3 (14.1115f, 12f, 1), Quaternion.identity) as GameObject;
		tradeThis[2] =Instantiate (Resources.Load ("card_wood"), new Vector3 (15.6386f, 12f, 1), Quaternion.identity) as GameObject;
		tradeThis[3] =Instantiate (Resources.Load ("card_grain"), new Vector3 (17.1657f, 12f, 1), Quaternion.identity) as GameObject;
		tradeThis[4] =Instantiate (Resources.Load ("card_sheep"), new Vector3 (18.6928f, 12f, 1), Quaternion.identity) as GameObject;
		
		//For this (what you want)
		forThis[0] =Instantiate (Resources.Load ("card_brick"), new Vector3 (12.5844f, 7f, 1), Quaternion.identity) as GameObject;
		forThis[1] =Instantiate (Resources.Load ("card_ore"), new Vector3 (14.1115f, 7f, 1), Quaternion.identity) as GameObject;
		forThis[2] =Instantiate (Resources.Load ("card_wood"), new Vector3 (15.6386f, 7f, 1), Quaternion.identity) as GameObject;
		forThis[3] =Instantiate (Resources.Load ("card_grain"), new Vector3 (17.1657f, 7f, 1), Quaternion.identity) as GameObject;
		forThis[4] =Instantiate (Resources.Load ("card_sheep"), new Vector3 (18.6928f, 7f, 1), Quaternion.identity) as GameObject;
		
		tradeThisText[0] = Instantiate (Resources.Load ("text"), new Vector3 (12.4493f, 1.908892f+12f, -4f), Quaternion.identity) as GameObject;
		tradeThisText[1] = Instantiate (Resources.Load ("text"), new Vector3 (13.9764f, 1.908892f+12f, -4f), Quaternion.identity) as GameObject;
		tradeThisText[2] = Instantiate (Resources.Load ("text"), new Vector3 (15.5035f, 1.908892f+12f, -4f), Quaternion.identity) as GameObject;
		tradeThisText[3] = Instantiate (Resources.Load ("text"), new Vector3 (17.0306f, 1.908892f+12f, -4f), Quaternion.identity) as GameObject;
		tradeThisText[4] = Instantiate (Resources.Load ("text"), new Vector3 (18.5577f, 1.908892f+12f, -4f), Quaternion.identity) as GameObject;
		
		forThisText[0] = Instantiate (Resources.Load ("text"), new Vector3 (12.4493f, 1.908892f+7f, -4f), Quaternion.identity) as GameObject;
		forThisText[1] = Instantiate (Resources.Load ("text"), new Vector3 (13.9764f, 1.908892f+7f, -4f), Quaternion.identity) as GameObject;
		forThisText[2] = Instantiate (Resources.Load ("text"), new Vector3 (15.5035f, 1.908892f+7f, -4f), Quaternion.identity) as GameObject;
		forThisText[3] = Instantiate (Resources.Load ("text"), new Vector3 (17.0306f, 1.908892f+7f, -4f), Quaternion.identity) as GameObject;
		forThisText[4] = Instantiate (Resources.Load ("text"), new Vector3 (18.5577f, 1.908892f+7f, -4f), Quaternion.identity) as GameObject;
		
		for (int i = 0; i < 5; i++) {
			forThisText[i].GetComponent<TextMesh> ().text = "" + 0;
			tradeThisText[i].GetComponent<TextMesh> ().text = "" + 0;
		}
		
		GameObject temp = Instantiate (Resources.Load ("text"), new Vector3 (14.5f,14.7f,-1f), Quaternion.identity) as GameObject;
		temp.GetComponent<TextMesh> ().text = "I will trade this:" ;
		temp = Instantiate (Resources.Load ("text"), new Vector3 (15f,10f,-1f), Quaternion.identity) as GameObject;
		temp.GetComponent<TextMesh> ().text = "for this:" ;
		
		tradeButton = Instantiate (Resources.Load ("backing"), new Vector3(15.5f,3.75f,1f), Quaternion.identity) as GameObject;
		tradeButton.transform.localScale = new Vector3(3f,1f,1f);
		
		temp = Instantiate (Resources.Load ("text"), new Vector3(14.2f,4f,1f), Quaternion.identity) as GameObject;
		temp.GetComponent<TextMesh> ().text = "Offer Trade!" ;
	}
	
	private void IncrementState ()
	{
		curState = gamestate.IncrementState ();
	}

	private void OfferTrade()
	{
		print ("TODO: Trade has been offered!");

		Player humanPlayer = gamestate.GetPlayerAtIndex (0);
		
		int[] giveResources = new int[5]{Convert.ToInt32(tradeThisText[0].GetComponent<TextMesh>().text), 
			Convert.ToInt32(tradeThisText[1].GetComponent<TextMesh>().text), 
			Convert.ToInt32(tradeThisText[2].GetComponent<TextMesh>().text), 
			Convert.ToInt32(tradeThisText[3].GetComponent<TextMesh>().text), 
			Convert.ToInt32(tradeThisText[4].GetComponent<TextMesh>().text)};
		
		int[] getResources = new int[5]{Convert.ToInt32(forThisText[0].GetComponent<TextMesh>().text), 
			Convert.ToInt32(forThisText[1].GetComponent<TextMesh>().text), 
			Convert.ToInt32(forThisText[2].GetComponent<TextMesh>().text), 
			Convert.ToInt32(forThisText[3].GetComponent<TextMesh>().text), 
			Convert.ToInt32(forThisText[4].GetComponent<TextMesh>().text)};
		
		TradeOffer offer = humanPlayer.generateHumanTradeRequest(gamestate.getTurnCounter(), giveResources, getResources);
		
		tradeManager.ExecuteTradeOfferNotification (offer);
	}

	/*
	 * Handles interactions with game FSM and player input.
	 */
	void Update ()
	{
		Player currentTurnPlayer = gamestate.GetCurrentTurnPlayer ();
		UpdateHumanCardCounts ();

		long elapsedTicksSinceLastAIAction = DateTime.Now.Ticks - lastAIActionTime.Ticks;
		double secondsSinceLastAIAction = new TimeSpan(elapsedTicksSinceLastAIAction).TotalSeconds;


		//AI Interaction
		if (currentTurnPlayer.isAI) {
			if (secondsSinceLastAIAction >= FORCED_TIME_BETWEEN_AI_ACTIONS) {
				System.Random rand = new System.Random();

				//Initial settlement placement
				if (curState == GameState.State.placeSettlement) {
					List<Node> locationOptions = AIEngine.GetFavorableStartingLocations(board);

					//Attempt to place elements in decreasing score order
					for (int i = 0; i < locationOptions.Count; i++) {
						if (board.CanBuildSettlementHere(locationOptions[i].visual.transform, currentTurnPlayer, true)) {
							lastStructurePlaced = board.PlaceSettlement(locationOptions[i].visual.transform, currentTurnPlayer, false);
							break;
						}
					}

					IncrementState ();
				}
				//Initial road placement
				else if (curState == GameState.State.placeRoad) {
					List<Edge> favorableRoads = AIEngine.GetFavorableRoadExpansions(currentTurnPlayer, board, lastStructurePlaced);
					
					foreach (Edge road in favorableRoads) {
						if (board.CanBuildRoadHere(road.visual.transform, currentTurnPlayer)) {
							lastRoadPlaced = board.PlaceRoad(road.visual.transform, currentTurnPlayer, false);
							break;
						}
					}

					IncrementState ();
				}
				//Roll dice
				else if (curState == GameState.State.roll) {
					if (rollForAI) {
						if (Input.GetMouseButtonDown (0)) {
							Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
							RaycastHit hit;
							if (interactDebug) { print ("mouse press"); }
							if (Physics.Raycast (ray, out hit)) {
								if (hit.transform == dice.transform) {
									IncrementState ();
									updateDice();
								}
							}
						}
					}
					else {
						IncrementState ();
						updateDice();
					}
				}
				//Trade with players
				else if (curState == GameState.State.trade) {
					//TODO
					IncrementState();
				}
				//Building phase
				else if (curState == GameState.State.place) {
					//TODO
					List<AIEngine.Objective> objectives = AIEngine.GetObjectives(currentTurnPlayer, board);
					foreach(AIEngine.Objective objective in objectives) {
						print (objective);
					}
					foreach(AIEngine.Objective objective in objectives) {
						if (objective.Score() > 0 && objective.TotalCardsNeeded() == 0) {
							AIEngine.PerformObjective(objective, board);
						}
						break;
					}
					IncrementState();
				}
				//Place robber
				else if (curState == GameState.State.robber)
				{
					foreach(Player p in gamestate.GetAllPlayers())
					{
						p.gotRobbed();
					}

					Player competitorPlayer = gamestate.biggestCompetitorToPlayer(gamestate.GetCurrentTurnPlayer());

					// Place robber on hex belonging to competitionPlayer

					bool robberPlaced = false;

					while (!robberPlaced)
					{
						int tileIndex = rand.Next (board.tiles.Count);
						robberPlaced = board.PlaceRobber (board.tileHitboxes[tileIndex].transform);
					}

					IncrementState();
				}

				lastAIActionTime = DateTime.Now; //Prevent AI from acting too quickly
			}
		}
		//Human Interaction
		else {
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (interactDebug) { print ("mouse press"); }
				if (Physics.Raycast(ray, out hit)) {

					//Need to place something
					if (curState == GameState.State.placeSettlement || curState == GameState.State.placeRoad || curState == GameState.State.place) {

						bool isSetup = (curState == GameState.State.placeSettlement || curState == GameState.State.placeRoad);

						//Choose what to build or use preselected
						if (curState == GameState.State.place) {
							if( hit.transform == roadSelector.transform && currentTurnPlayer.CanBuildRoad()){
								objectToBuild = hit.transform;
								if (interactDebug) { print ("on a building"); }
							}
							else if (hit.transform == citySelector.transform && currentTurnPlayer.CanBuildCity()){
								objectToBuild = hit.transform;
								if (interactDebug) { print ("on a building"); }
							}
							else if (hit.transform == settlementSelector.transform && currentTurnPlayer.CanBuildSettlement()){
								objectToBuild = hit.transform;
								if (interactDebug) { print ("on a building"); }
							}
							else if (hit.transform == endTurnButton.transform){
								IncrementState();
							}
						}
						else if (curState == GameState.State.placeSettlement) {
							objectToBuild = settlementSelector.transform;
						}
						else if (curState == GameState.State.placeRoad) {
							objectToBuild = roadSelector.transform;
						}

						if(objectToBuild != null) {
							if (objectToBuild == roadSelector.transform && board.roadHitboxes.ContainsKey(hit.transform)) {
								Node structureToBuildNear = (curState == GameState.State.placeRoad) ? lastStructurePlaced : null;

								if(board.CanBuildRoadHere(hit.transform, currentTurnPlayer, structureToBuildNear, true)) {
									lastRoadPlaced = board.PlaceRoad(hit.transform, currentTurnPlayer, !isSetup);
									objectToBuild = null;
									if (interactDebug) { print ("road built!"); }
									if (curState == GameState.State.placeRoad) {
										IncrementState ();
									}
								}
							}
							else if(objectToBuild == settlementSelector.transform && board.settlementHitboxes.ContainsKey(hit.transform)) {
								if(board.CanBuildSettlementHere(hit.transform, currentTurnPlayer, isSetup, true)){
									lastStructurePlaced = board.PlaceSettlement(hit.transform, currentTurnPlayer, !isSetup);
									objectToBuild = null;
									if (interactDebug) { print ("settlement built!"); }
									if (curState == GameState.State.placeSettlement) {
										IncrementState ();
									}
								}
							}
							else if (objectToBuild == citySelector.transform && board.settlements.ContainsKey(hit.transform)) {
								if(board.CanBuildCityHere(hit.transform, currentTurnPlayer, true)){
									lastStructurePlaced = board.PlaceCity(hit.transform, currentTurnPlayer);
									objectToBuild = null;
									if (interactDebug) { print ("city built!"); }
								}
							}
						}
					}
					//Place robber on a chit
					else if (curState == GameState.State.robber) {
						if (board.PlaceRobber(hit.transform)) {
							IncrementState (); //increment if successfully placed
						}
					}
					//Request trades with other players
					else if (curState == GameState.State.trade) {
						UpdateTradePanel (hit.transform);
						
						if (hit.transform == tradeButton.transform) {
							OfferTrade ();
							IncrementState ();
						} else if (hit.transform == endTurnButton.transform){
							IncrementState ();
							IncrementState ();
						}
					}
					//listen for click on dice
					else if (curState == GameState.State.roll ) {
						if (interactDebug) { print (hit.transform == dice.transform); }
						if (hit.transform == dice.transform) {
							IncrementState ();
							updateDice ();
						}
					}
					else {
						if (interactDebug) { print ("should not be here"); }
					}
				}
			}
		}
	}

	public void updateDice() {
		diceNum.GetComponent<TextMesh> ().text = "" + gamestate.GetRoll();
	}

	private void UpdateHumanCardCounts()
	{
		humanCardCounts[0].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().brick;
		humanCardCounts[1].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().ore;
		humanCardCounts[2].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().wood;
		humanCardCounts[3].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().grain;
		humanCardCounts[4].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().sheep;
	}

	private void UpdateTradePanel(Transform hitbox)
	{
		if (interactDebug)
		{
			print ("This is trade");
		}

		for (int i = 0; i < 5; i++)
		{
			if (tradeThis[i].transform == hitbox)
			{
				if(Convert.ToInt32(forThisText[i].GetComponent<TextMesh>().text) > 0)
				{
					forThisText[i].GetComponent<TextMesh>().text = "" + 0;
				}

				tradeThisText[i].GetComponent<TextMesh>().text = "" + 
					((Convert.ToInt32(tradeThisText[i].GetComponent<TextMesh>().text)+1) % (gamestate.GetPlayerAtIndex(0).Hand().GetResourceQuantity(i)+1) );
			}
			else if(forThis[i].transform == hitbox)
			{
				if(Convert.ToInt32(tradeThisText[i].GetComponent<TextMesh>().text) > 0)
				{
					tradeThisText[i].GetComponent<TextMesh>().text = "" + 0;
				}

				forThisText[i].GetComponent<TextMesh>().text = "" + ((Convert.ToInt32(forThisText[i].GetComponent<TextMesh>().text)+1) % 11);
			}
		}
	}
}
