using UnityEngine;
using System.Collections;

public class Player {

	public int id;
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
	}

}
