using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameEngine : MonoBehaviour
{
	//Debug variables
	private bool rollForAI = false;
	private bool interactDebug = false;
	bool debugMessages = false;

	Board board;
	GameState gamestate;
	TradeManager tradeManager;

	Transform objectToBuild = null;
	
	Node lastStructurePlaced = null;
	Edge lastRoadPlaced = null;
	AIEngine.Objective proposedObjective = null;

	GameState.State curState;

	GameObject[] turnInfoText;

	GameObject citySelector;
	GameObject citySelectorText;
	GameObject settlementSelector;
	GameObject settlementSelectorText;
	GameObject roadSelector;
	GameObject roadSelectorText;
	GameObject selectorBacking;

	GameObject tradeThisDiscription;
	GameObject forThisDiscription;
	GameObject[] tradeThisText;
	GameObject[] forThisText;
	GameObject[] tradeThis;
	GameObject[] forThis;

	GameObject offerTradeButton;
	GameObject offerTradeButtonText;
	GameObject acceptTradeButton;
	GameObject acceptTradeButtonText;
	GameObject declineTradeButton;
	GameObject declineTradeButtonText;
	GameObject endTradeButton;
	GameObject endTradeButtonText;


	GameObject endTurnButton;
	GameObject endTurnButtonText;

	GameObject dice;
	GameObject diceNum;

	GameObject[] humanCardCounts;


	DateTime lastAIActionTime; //used to slow down AI players
	readonly DateTime EPOCH = new DateTime(2001, 1, 1);
	private float FORCED_TIME_BETWEEN_AI_ACTIONS = 0.1f;

	void Start () {
		board = new Board (); // instantiates and draws
		gamestate = new GameState (4, board);
		tradeManager = new TradeManager (gamestate, this);
		humanCardCounts = new GameObject[5];
		tradeThisText = new GameObject[5];
		forThisText = new GameObject[5];
		tradeThis = new GameObject[5];
		forThis = new GameObject[5];
		turnInfoText = new GameObject[gamestate.numPlayers];

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
		dice = Instantiate (Resources.Load ("dice"), new Vector3 (15f, 6f, 1), Quaternion.identity) as GameObject;
		diceNum = Instantiate (Resources.Load ("text"), new Vector3 (-4f, 6f, 1f), Quaternion.identity) as GameObject;
		diceNum.GetComponent<TextMesh> ().text = "" ;
		ShowHideDice ();

	}

	private void BuildEndTurnButton()
	{
		endTurnButton = Instantiate (Resources.Load ("backing"), new Vector3 (6f,0f,1f), Quaternion.identity) as GameObject;
		endTurnButton.transform.localScale = new Vector3 (3f,1f,1f);
		endTurnButtonText = Instantiate (Resources.Load ("text"), new Vector3 (5.2f,0.25f,-1f), Quaternion.identity) as GameObject; 
		endTurnButtonText.GetComponent<TextMesh> ().text = "End Turn" ; 
		ShowHideEndTurnButton ();
	}

	private void BuildHumanPlayerHandDisplay()
	{
		float offset = -19f;

		Instantiate (Resources.Load ("card_brick"), new Vector3 (12.5844f+offset, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_ore"), new Vector3 (14.1115f+offset, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_wood"), new Vector3 (15.6386f+offset, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_grain"), new Vector3 (17.1657f+offset, 0.1522f, 1), Quaternion.identity);
		Instantiate (Resources.Load ("card_sheep"), new Vector3 (18.6928f+offset, 0.1522f, 1), Quaternion.identity);

		humanCardCounts[0] = Instantiate (Resources.Load ("text"), new Vector3 (12.4493f+offset, 1.908892f, -4f), Quaternion.identity) as GameObject;
		humanCardCounts[1] = Instantiate (Resources.Load ("text"), new Vector3 (13.9764f+offset, 1.908892f, -4f), Quaternion.identity) as GameObject;
		humanCardCounts[2] = Instantiate (Resources.Load ("text"), new Vector3 (15.5035f+offset, 1.908892f, -4f), Quaternion.identity) as GameObject;
		humanCardCounts[3] = Instantiate (Resources.Load ("text"), new Vector3 (17.0306f+offset, 1.908892f, -4f), Quaternion.identity) as GameObject;
		humanCardCounts[4] = Instantiate (Resources.Load ("text"), new Vector3 (18.5577f+offset, 1.908892f, -4f), Quaternion.identity) as GameObject;

		UpdateHumanCardCounts ();
	}

	private void BuildPlayerTurnWindow()
	{

		Instantiate(Resources.Load("PlayerOrder"), new Vector3(-3, 12, 0), Quaternion.identity);


		for (int i =0;i<turnInfoText.Length;i++){
			turnInfoText[i] = Instantiate (Resources.Load ("text"), new Vector3 (-4f, 13.7f-i, -4f), Quaternion.identity) as GameObject;
			turnInfoText[i].GetComponent<TextMesh>().color = gamestate.GetAllPlayers()[i].color;
			turnInfoText[i].GetComponent<TextMesh> ().text = "";
			turnInfoText[i].GetComponent<TextMesh>().characterSize = .07f;
		}
		turnInfoText[0].GetComponent<TextMesh> ().text = "Placing Initial Settlement" ;
	}


	private void BuildSelectionPanel()
	{

		float offset = 19f;

		selectorBacking = Instantiate(Resources.Load("backing"), new Vector3(-3.598929f+offset,1f,1f), Quaternion.identity) as GameObject;
		
		citySelector = Instantiate(Resources.Load("city"), new Vector3(-5f+offset,2f,-1f), Quaternion.identity) as GameObject;
		citySelectorText = Instantiate (Resources.Load ("text"), new Vector3 (-5.4f+offset,3f,-1f), Quaternion.identity) as GameObject;
		citySelectorText.GetComponent<TextMesh> ().text = "City" ;
		citySelector.renderer.material.color = Color.red;
		citySelector.transform.localScale += citySelector.transform.localScale;
		roadSelectorText = Instantiate (Resources.Load ("text"), new Vector3 (-3f+offset,.95f,-1f), Quaternion.identity) as GameObject;
		roadSelectorText.GetComponent<TextMesh> ().text = "Road" ;
		roadSelector = Instantiate(Resources.Load("road"), new Vector3(-2.5f+offset,0f,-1f), Quaternion.identity) as GameObject;
		roadSelector.renderer.material.color = Color.red;
		roadSelector.transform.localScale += roadSelector.transform.localScale;
		settlementSelectorText = Instantiate (Resources.Load ("text"), new Vector3 (-3.6f+offset,3f,-1f), Quaternion.identity) as GameObject;
		settlementSelectorText.GetComponent<TextMesh> ().text = "Settlement" ;
		settlementSelector = Instantiate(Resources.Load("settlement"), new Vector3(-2.5f+offset,2f,-1f), Quaternion.identity) as GameObject;
		settlementSelector.renderer.material.color = Color.red;
		settlementSelector.transform.localScale += settlementSelector.transform.localScale;

		ShowHideSelector ();
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
		
		tradeThisDiscription = Instantiate (Resources.Load ("text"), new Vector3 (14.5f,14.7f,-1f), Quaternion.identity) as GameObject;
		tradeThisDiscription.GetComponent<TextMesh> ().text = "I will trade this:" ;
		forThisDiscription = Instantiate (Resources.Load ("text"), new Vector3 (15f,10f,-1f), Quaternion.identity) as GameObject;
		forThisDiscription.GetComponent<TextMesh> ().text = "for this:" ;
		
		offerTradeButton = Instantiate (Resources.Load ("backing"), new Vector3(14f,3.75f,1f), Quaternion.identity) as GameObject;
		offerTradeButton.transform.localScale = new Vector3(3f,1f,1f);
		
		offerTradeButtonText = Instantiate (Resources.Load ("text"), new Vector3(12.7f,4f,1f), Quaternion.identity) as GameObject;
		offerTradeButtonText.GetComponent<TextMesh> ().text = "Offer Trade!" ;


		endTradeButton = Instantiate (Resources.Load ("backing"), new Vector3(17.5f,3.75f,1f), Quaternion.identity) as GameObject;
		endTradeButton.transform.localScale = new Vector3(3f,1f,1f);
		
		endTradeButtonText = Instantiate (Resources.Load ("text"), new Vector3(16.2f,4f,1f), Quaternion.identity) as GameObject;
		endTradeButtonText.GetComponent<TextMesh> ().text = "End Trading" ;

		acceptTradeButton = offerTradeButton;
		acceptTradeButtonText = offerTradeButtonText;

		declineTradeButton = endTradeButton;
		acceptTradeButtonText = offerTradeButtonText;

		ShowHideTrade ();


	}
	
	private void IncrementState ()
	{
		//Things to do before incrementing state
		switch (curState) {
		case GameState.State.placeSettlement:
			break;
		case GameState.State.placeRoad:
			break;
		case GameState.State.roll:
			if(rollForAI || !gamestate.GetCurrentTurnPlayer().isAI) ShowHideDice();
			break;
		case GameState.State.robber:
			break;
		case GameState.State.trade:
			if(!gamestate.GetCurrentTurnPlayer().isAI) ShowHideTrade(true);
			break;
		case GameState.State.place:
			if(!gamestate.GetCurrentTurnPlayer().isAI) { ShowHideSelector(); ShowHideEndTurnButton(); }
			break;
		case GameState.State.end:
			break;
		default:
			break;
			
		}

		curState = gamestate.IncrementState ();

		//Things to do after
		switch (curState) {
		case GameState.State.placeSettlement:
			UpdateTurnInfo("Placing Settlement", gamestate.GetCurrentTurnPlayer().id);break;
		case GameState.State.placeRoad:
			UpdateTurnInfo("Placing Road", gamestate.GetCurrentTurnPlayer().id);break;
		case GameState.State.roll:
			UpdateTurnInfo("Rolling", gamestate.GetCurrentTurnPlayer().id);
			if(rollForAI || !gamestate.GetCurrentTurnPlayer().isAI) ShowHideDice();

			break;
		case GameState.State.robber:
			UpdateTurnInfo("Moving Robber", gamestate.GetCurrentTurnPlayer().id);break;
		case GameState.State.trade:
			if(!gamestate.GetCurrentTurnPlayer().isAI) { ShowHideTrade(true); ShowHideEndTurnButton(); }
			UpdateTurnInfo("Trading", gamestate.GetCurrentTurnPlayer().id);break;
		case GameState.State.place:
			if(!gamestate.GetCurrentTurnPlayer().isAI) ShowHideSelector();
			UpdateTurnInfo("Placing", gamestate.GetCurrentTurnPlayer().id);break;
		case GameState.State.end:
			UpdateTurnInfo("WINNER!", gamestate.GetCurrentTurnPlayer().id);break;
		default:
			UpdateTurnInfo("What is going on!?!", gamestate.GetCurrentTurnPlayer().id);break;

		}

	}

	private void OfferTrade()
	{

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

		int sumGiveResources = giveResources [0] + giveResources [1] + giveResources [2] + giveResources [3] + giveResources [4];
		int sumGetResources = getResources [0] + getResources [1] + getResources [2] + getResources [3] + getResources [4];

		if (sumGiveResources == 4 && sumGetResources == 1 &&
				(giveResources [0] == 4 || giveResources [1] == 4 || giveResources [2] == 4 || giveResources [3] == 4 || giveResources [4] == 4)) {
			tradeManager.ExecuteTradeWithBank (offer, gamestate.GetCurrentTurnPlayer());
		} else {
			if( tradeManager.ExecuteTradeOfferNotification (offer) ){
				//todo feedback for successful trade
			} else {
				//todo feedback for failed trade
			}
		}

	}

	public void ShowHideTrade(bool makingOffer = true, TradeOffer offer = null) {

		tradeThisDiscription.renderer.enabled = !tradeThisDiscription.renderer.enabled;

		
		forThisDiscription.renderer.enabled = !forThisDiscription.renderer.enabled;


		offerTradeButton.renderer.enabled = !offerTradeButton.renderer.enabled;
		offerTradeButton.collider.enabled = !offerTradeButton.collider.enabled; 
		
		offerTradeButtonText.renderer.enabled = !offerTradeButtonText.renderer.enabled; 


		endTradeButtonText.renderer.enabled = !endTradeButtonText.renderer.enabled;
		 
		
		endTradeButton.renderer.enabled = !endTradeButton.renderer.enabled;
		endTradeButton.collider.enabled = !endTradeButton.collider.enabled;

		int[] getArray = {};
		int[] giveArray = {};

		if(!makingOffer){
			PlayerHand get = offer.convertGetResourcesToPlayerHand();
			getArray = get.ToArray ();

			PlayerHand give = offer.convertGiveResourcesToPlayerHand();
			giveArray = give.ToArray ();
		}



		GameObject[][] tradingObjects = {tradeThisText, forThisText, tradeThis, forThis};
		int counter = 0;
		int objCounter = 0;
		foreach (GameObject[] arr in tradingObjects) {
			counter = 0;
			foreach (GameObject obj in arr){

				obj.renderer.enabled = !obj.renderer.enabled;
				if (obj.collider != null) obj.collider.enabled = !obj.collider.enabled;

				if (obj.collider == null && makingOffer){
					obj.GetComponent<TextMesh>().text = "0";
				} else if (obj.collider == null){
					if(objCounter == 2) obj.GetComponent<TextMesh>().text = ""+giveArray[counter];
					else obj.GetComponent<TextMesh>().text = ""+getArray[counter];
					counter++;
				}

			}
			objCounter++;
		}

		//TODO make gui for trading offer
		if (makingOffer) {
			offerTradeButtonText.GetComponent<TextMesh> ().text = "Offer Trade!";
			endTradeButtonText.GetComponent<TextMesh> ().text = "End Trading";

		} else {
			offerTradeButtonText.GetComponent<TextMesh> ().text = "Accept Offer";
			endTradeButtonText.GetComponent<TextMesh> ().text = "Decline Offer";
		}

	}

	private void ShowHideSelector() {
		selectorBacking.renderer.enabled = !selectorBacking.renderer.enabled;
		selectorBacking.collider.enabled = !selectorBacking.collider.enabled;

		citySelector.renderer.enabled = !citySelector.renderer.enabled;
		citySelector.collider.enabled = !citySelector.collider.enabled;

		settlementSelector.renderer.enabled = !settlementSelector.renderer.enabled;
		settlementSelector.collider.enabled = !settlementSelector.collider.enabled;

		roadSelector.renderer.enabled = !roadSelector.renderer.enabled;
		roadSelector.collider.enabled = !roadSelector.collider.enabled;

		citySelectorText.renderer.enabled = !citySelectorText.renderer.enabled;


		roadSelectorText.renderer.enabled = !roadSelectorText.renderer.enabled;


		settlementSelectorText.renderer.enabled = !settlementSelectorText.renderer.enabled;


	}

	private void ShowHideDice () {
		dice.renderer.enabled = !dice.renderer.enabled;
		dice.collider.enabled = !dice.collider.enabled;
	}

	private void ShowHideEndTurnButton () {
		endTurnButton.renderer.enabled = !endTurnButton.renderer.enabled;
		endTurnButton.collider.enabled = !endTurnButton.collider.enabled;

		endTurnButtonText.renderer.enabled = !endTurnButtonText.renderer.enabled;

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
					UpdateTurnInfo("Placing Initial Road", currentTurnPlayer.id);
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
				else if (curState == GameState.State.trade)
				{
					proposedObjective = null;

					if(debugMessages)
					{
						print ("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");
					}

					//TODO
					List<AIEngine.Objective> objectives = AIEngine.GetObjectives(currentTurnPlayer, board, gamestate);

					// Trade With Other Players
					int tradeOffersThisTurn = 0;
					bool hasAnyOfferBeenAccepted = false;
					foreach(AIEngine.Objective objective in objectives)
					{
						if(objective.Score () > 0 && objective.TotalCardsNeeded() > 0 && tradeOffersThisTurn < 3 && !hasAnyOfferBeenAccepted)
						{
							TradeOffer offer = currentTurnPlayer.generateAITradeRequest(gamestate.getTurnCounter(), objective);

							if(null != offer)
							{
								if(debugMessages)
								{
									print ("TRADENUM: " + tradeOffersThisTurn + " | " + gamestate.GetCurrentTurnPlayer() +  "\nATTEMPTING TO TRADE WITH OTHER PLAYERS TO ACHIEVE OBJECT: " + objective.GetObjectiveScore());
								}

								tradeOffersThisTurn++;
								hasAnyOfferBeenAccepted = tradeManager.ExecuteTradeOfferNotification(offer);
								proposedObjective = objective;
							}
						}
					}

					// Trade With Bank
					foreach(AIEngine.Objective objective in objectives)
					{
						if(objective.Score () > 0 && objective.TotalCardsNeeded() > 0 && !hasAnyOfferBeenAccepted)
						{
							if(debugMessages)
							{
								print (gamestate.GetCurrentTurnPlayer() +  " ATTEMPTING TO TRADE WITH BANK");
							}

							TradeOffer offer = currentTurnPlayer.generateAITradeWithBank(objective);
							
							if(null != offer)
							{
								tradeManager.ExecuteTradeWithBank(offer, gamestate.GetCurrentTurnPlayer());
								hasAnyOfferBeenAccepted = true;
							}
						}
					}

					if(!hasAnyOfferBeenAccepted && debugMessages)
					{
						print (gamestate.GetCurrentTurnPlayer() + " MADE NO TRADES THIS TURN");
					}
				
					IncrementState();
					FORCED_TIME_BETWEEN_AI_ACTIONS = 0f; //TODO Remove
				}
				//Building phase
				else if (curState == GameState.State.place)
				{
					FORCED_TIME_BETWEEN_AI_ACTIONS = 0f; //TODO Remove

					//TODO
					List<AIEngine.Objective> objectives = AIEngine.GetObjectives(currentTurnPlayer, board, gamestate);
					foreach (AIEngine.Objective objective in objectives) {
						print (objective);
					}
					
					foreach (AIEngine.Objective objective in objectives) {
						if (objective.TotalCardsNeeded() == 0) {
							AIEngine.PerformObjective(objective, board);
							break;
						}
					}

					IncrementState();
				}
				//Place robber
				else if (curState == GameState.State.robber) {
					Player competitorPlayer = gamestate.BiggestCompetitorToPlayer(currentTurnPlayer);
					List<Tile> possiblePlacements = AIEngine.GetListOfRobberPlacements(currentTurnPlayer, competitorPlayer, board);

					bool robberPlaced = false;

					//Attempt to place robber on recommended tiles
					foreach (Tile tile in possiblePlacements) {
						int index = board.tiles.IndexOf (tile);
						if (board.PlaceRobber (board.tileHitboxes[index].transform)) {
							robberPlaced = true;
							break;
						}
					}

					//If for some reason we're out of recommendations...
					while (!robberPlaced) {
						print ("ERROR: ATTEMPTING TO RANDOMLY PLACE ROBBER!");
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
					//Request trades with other players or bank
					else if (curState == GameState.State.trade) {

						UpdateTradePanel (hit.transform);
						if(hit.transform == endTradeButton.transform){
							IncrementState();
						} else if (hit.transform == offerTradeButton.transform) {
							OfferTrade();
						} else if (hit.transform == endTurnButton.transform){
							IncrementState ();
							IncrementState ();
						}


						//tradeManager.ExecuteTradeWithBank(offer, gamestate.GetCurrentTurnPlayer());


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
		diceNum.GetComponent<TextMesh> ().text = "Dice Roll: " + gamestate.GetRoll();

	}

	private void UpdateHumanCardCounts()
	{
		humanCardCounts[0].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).GetPlayerHand().brick;
		humanCardCounts[1].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).GetPlayerHand().ore;
		humanCardCounts[2].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).GetPlayerHand().wood;
		humanCardCounts[3].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).GetPlayerHand().grain;
		humanCardCounts[4].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).GetPlayerHand().sheep;
	}

	private void UpdateTurnInfo(String message, int PlayerId){

		foreach (GameObject t in turnInfoText) {
			t.GetComponent<TextMesh> ().text = "";
				}

		turnInfoText [PlayerId].GetComponent<TextMesh> ().text = message;



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
					((Convert.ToInt32(tradeThisText[i].GetComponent<TextMesh>().text)+1) % (gamestate.GetPlayerAtIndex(0).GetPlayerHand().GetResourceQuantity(i)+1) );
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
