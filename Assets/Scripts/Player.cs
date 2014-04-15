using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {

	public int id;
	public List<int> cards;
	public Color color;
	
	public Player(int id){
		this.id = id;
		switch (id) {
			case 1: color = Color.blue; break;
			case 2: color = Color.red; break;
			case 3: color = Color.green; break;
			case 4: color = Color.yellow; break;
			default: color = Color.black; break;
		}
		cards = new List<int>{ 0, 0, 0, 0, 0 };
	}

}
