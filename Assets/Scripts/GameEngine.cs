using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameEngine : MonoBehaviour
{
	Board board;
	GameState gamestate;
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

	void Start () {
		board = new Board (); // instantiates and draws
		gamestate = new GameState (4, board);
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
		endTurnButton = Instantiate (Resources.Load ("backing"), new Vector3(6f,0f,1f), Quaternion.identity) as GameObject;
		endTurnButton.transform.localScale = new Vector3(3f,1f,1f);
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

	private void OfferTrade() {
		print ("TODO: Trade has been offered!");
	}

	/*
	* Handles interactions with game FSM and player input.
	*/
	void Update ()
	{
		//AI Interaction
		if (gamestate.GetCurrentTurnPlayer().isAI) {
			//TODO replace logic here with AIEngine calls

			bool structurePlaced = false;
			System.Random rand = new System.Random();

			if (curState == GameState.State.placeSettlement) {
				while (!structurePlaced) {
					int location = rand.Next (54);
					if (board.CanBuildSettlementHere(board.vertices[location].visual.transform, gamestate.GetCurrentTurnPlayer(), true)) {
						lastStructurePlaced = board.PlaceSettlement(board.vertices[location].visual.transform, gamestate.GetCurrentTurnPlayer());
						structurePlaced = true;
					}
				}
				IncrementState ();
			}
			else if (curState == GameState.State.placeRoad) {
				//Only allow roads placed from previous settlement
				foreach (Edge road in lastStructurePlaced.getRoads()) {
					if (board.CanBuildRoadHere(road.visual.transform, gamestate.GetCurrentTurnPlayer())) {
						lastRoadPlaced = board.PlaceRoad(road.visual.transform, gamestate.GetCurrentTurnPlayer());
						break;
					}
				}
				IncrementState ();
			}
			else if (curState == GameState.State.roll ){

				if(gamestate.debug){
					if (Input.GetMouseButtonDown (0)){
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						RaycastHit hit;
						print ("mouse press");
						if (Physics.Raycast(ray, out hit)) {
							if (hit.transform == dice.transform){
								IncrementState ();
								updateDice();
							}
						}
					}
				}else{
					IncrementState ();
					updateDice();
				}
				
			}


		}
		//Human Interaction
		else {
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				print ("mouse press");
				if (Physics.Raycast(ray, out hit)) {



					//Need to place something
					if (curState == GameState.State.placeSettlement || curState == GameState.State.placeRoad || curState == GameState.State.place || curState == GameState.State.robber) {

						bool isSetup = (curState == GameState.State.placeSettlement || curState == GameState.State.placeRoad);

						//Choose what to build or use preselected
						if (curState == GameState.State.place) {
							if( hit.transform == roadSelector.transform && gamestate.GetCurrentTurnPlayer().CanBuildRoad()){
								objectToBuild = hit.transform;
								print ("on a building");
							}
							else if (hit.transform == citySelector.transform && gamestate.GetCurrentTurnPlayer().CanBuildCity()){
								objectToBuild = hit.transform;
								print ("on a building");
							}
							else if (hit.transform == settlementSelector.transform && gamestate.GetCurrentTurnPlayer().CanBuildSettlement()){
								objectToBuild = hit.transform;
								print ("on a building");
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
								if(board.CanBuildRoadHere(hit.transform, gamestate.GetCurrentTurnPlayer(), lastStructurePlaced)) {
									lastRoadPlaced = board.PlaceRoad(hit.transform, gamestate.GetCurrentTurnPlayer());
									objectToBuild = null;
									print ("road built!");
									//TODO add color based on player and save the road in an list/array/dict
									if (curState == GameState.State.placeRoad) {
										IncrementState ();
									}
								}
							}
							else if(objectToBuild == settlementSelector.transform && board.settlementHitboxes.ContainsKey(hit.transform)) {
								if(board.CanBuildSettlementHere(hit.transform, gamestate.GetCurrentTurnPlayer(), isSetup)){
									lastStructurePlaced = board.PlaceSettlement(hit.transform, gamestate.GetCurrentTurnPlayer());
									objectToBuild = null;
									print ("settlement built!");
									if (curState == GameState.State.placeSettlement) {
										IncrementState ();
									}
								}
							}
							else if (objectToBuild == citySelector.transform && board.settlements.ContainsKey(hit.transform)) {
								if(board.CanBuildCityHere(hit.transform, gamestate.GetCurrentTurnPlayer())){
									lastStructurePlaced = board.PlaceCity(hit.transform, gamestate.GetCurrentTurnPlayer());
									objectToBuild = null;
									print ("city built!");
									//TODO add color based on player and save the city in an list/array/dict
								}
							}
						}
					}
					else if (curState == GameState.State.trade) {
						UpdateTradePanel (hit.transform);
						
						if (hit.transform == tradeButton.transform) {
							OfferTrade();
							IncrementState ();
						}

					}
					//listen for click on dice
					else if (curState == GameState.State.roll ){
						print (hit.transform == dice.transform);
						if (hit.transform == dice.transform){
							IncrementState ();
							updateDice();
						}

					}
					else {
						print ("should not be here");
					}
				}
			}
		}
	}

	public void updateDice() {
		diceNum.GetComponent<TextMesh> ().text = ""+gamestate.getRoll() ;
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
		print ("This is trade");
		for (int i = 0; i<5; i++) {
			if (tradeThis[i].transform == hitbox){
				tradeThisText[i].GetComponent<TextMesh>().text = "" + 
					((Convert.ToInt32(tradeThisText[i].GetComponent<TextMesh>().text)+1) % 
					 (Convert.ToInt32(humanCardCounts[i].GetComponent<TextMesh> ().text)+1) );
			}
			else if(forThis[i].transform == hitbox){
				forThisText[i].GetComponent<TextMesh>().text = "" + 
					((Convert.ToInt32(forThisText[i].GetComponent<TextMesh>().text)+1) % 11);
			}
		}
	}

}
