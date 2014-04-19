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

	GameObject[] humanCardCounts;

	void Start () {
		board = new Board (); // instantiates and draws
		gamestate = new GameState (4, board);
		humanCardCounts = new GameObject[5];

		BuildPlayerTurnWindow ();
		BuildSelectionPanel ();
		BuildHumanPlayerHandDisplay ();
		BuildDiceRoller ();
	}

	private void BuildDiceRoller() 
	{
		Instantiate (Resources.Load ("dice"), new Vector3 (14.24463f, 7.134943f, 1), Quaternion.identity);
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
		citySelector.renderer.material.color = Color.red;
		citySelector.transform.localScale += citySelector.transform.localScale;
		roadSelector = Instantiate(Resources.Load("road"), new Vector3(-2.5f,0f,-1f), Quaternion.identity) as GameObject;
		roadSelector.renderer.material.color = Color.red;
		roadSelector.transform.localScale += roadSelector.transform.localScale;
		settlementSelector = Instantiate(Resources.Load("settlement"), new Vector3(-2.5f,2f,-1f), Quaternion.identity) as GameObject;
		settlementSelector.renderer.material.color = Color.red;
		settlementSelector.transform.localScale += settlementSelector.transform.localScale;
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown(0)){ // if left button pressed...
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			print ("mouse press");
			if (Physics.Raycast(ray, out hit)){
				if(hit.transform == citySelector.transform || hit.transform == settlementSelector.transform || hit.transform == roadSelector.transform){
					objectToBuild = hit.transform;
					print ("on a building");
				}
				else if(objectToBuild != null ){
					if (objectToBuild == roadSelector.transform && board.roadHitboxes.ContainsKey(hit.transform)) {
						GameObject s = Instantiate(Resources.Load("road"), hit.transform.position , Quaternion.identity) as GameObject;
						s.transform.eulerAngles = hit.transform.eulerAngles;
						s.renderer.material.color = Color.red;
						Destroy(board.roadHitboxes[hit.transform]);
						board.roadHitboxes.Remove(hit.transform);
						objectToBuild = null;
						print ("road built!");
						//TODO add color based on player and save the road in an list/array/dict
					}
					else if(objectToBuild == settlementSelector.transform && board.settlementHitboxes.ContainsKey(hit.transform)){
						GameObject s = Instantiate(Resources.Load("settlement"), hit.transform.position , Quaternion.identity) as GameObject;
						s.renderer.material.color = Color.red;
						board.settlements.Add(s.transform, s);
						Destroy(board.settlementHitboxes[hit.transform]);
						board.settlementHitboxes.Remove(hit.transform);
						objectToBuild = null;
						print ("settlement built!");
					}
					else if (objectToBuild == citySelector.transform && board.settlements.ContainsKey(hit.transform)) {
						GameObject s = Instantiate(Resources.Load("city"), hit.transform.position , Quaternion.identity) as GameObject;
						s.renderer.material.color = Color.red;
						Destroy(board.settlements[hit.transform]);
						board.settlements.Remove(hit.transform);
						objectToBuild = null;
						print ("city built!");
						//TODO add color based on player and save the city in an list/array/dict
					}
				}
			}
		}
	}

	private void UpdateHumanCardCounts()
	{
		humanCardCounts[0].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().brick;
		humanCardCounts[1].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().ore;
		humanCardCounts[2].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().wood;
		humanCardCounts[3].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().grain;
		humanCardCounts[4].GetComponent<TextMesh> ().text = "" + gamestate.GetPlayerAtIndex(0).Hand().sheep;
	}
}
