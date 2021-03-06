﻿using UnityEngine;
using System.Collections;

public class AttackerWithTarget {
	public GameObject attacker;
	public Vector3 target;
	public AttackerWithTarget(GameObject a, Vector3 targetPosition) {
		attacker = a;
		target = targetPosition;
	}
}

public class ProtectGame : MonoBehaviour {

	private Vector3 targetPos = new Vector3(7.5f, 7.5f, Utility.Z);
	public GameObject defender;
	private int health = 10;
	
	public GameObject[] attacks;
	AttackerWithTarget[] attackers = new AttackerWithTarget[10];

	public Vector3 clickedPos = new Vector3 (-100, -100, -100);
	
	void Start() {
		defender = Instantiate (defender);
		Utility.InitializeFish (defender, targetPos, Utility.Z);
		
		for (int i = 0; i < attackers.Length; ++i) {
			attackers [i] = new AttackerWithTarget (Instantiate (attacks [i]), targetPos);
			
			GameObject attacker = attackers [i].attacker;

			float x = Random.Range(-48, 50);
			float y = Mathf.Sqrt(50 * 50 - x * x);

			if (x > 0)
				// if y is a value that if it was negative would not make the object more
				// than 30 degrees below the horizontal, then randomly flip the sign
				if (y < 50 * Mathf.Sin(0.523599f))
					y = (Random.value > 0.75) ? y : -y;
			else if (x == 0)
				y = 50;

			x += targetPos.x;
			y += targetPos.y;

			Utility.InitializeFish (attacker, new Vector3 (x, y, Utility.Z), Random.Range (2, 10));
			attacker.GetComponent<ActionObject> ().MakeUndestroyable ();
		}
	}
	
	void Update () {
		bool attackersRemaining = false;
		for (int i = 0; i < attackers.Length; ++i) {
			if (!attackers [i].attacker)
				continue;
			else
				attackersRemaining = true;
			
			GameObject attacker = attackers [i].attacker;
			
			ActionObject script = attacker.GetComponent<ActionObject> ();
			
			if (Utility.V3Equal (script.pos, attackers [i].target)) {
				if (Utility.V3Equal (script.pos, targetPos)) {
					--health;
					print (string.Format ("Hit! {0} health remaining.", health));
					Destroy (attacker);
					attackers [i].attacker = null;
				} else
					attackers [i].target = targetPos;
				
			} else {
				if (script.ClickedOn (clickedPos) && Utility.V3Equal (attackers [i].target, targetPos))
					attackers [i].target = GetNewTarget (script); 
				script.MoveTowardsTarget (attackers [i].target);
			}
		}
		
		if (!attackersRemaining)
			enabled = false;
		
		if (health == 0) {
			print ("Game over, attackers win!");
			Destroy (defender);
			foreach (AttackerWithTarget g in attackers) {
				if (g.attacker)
					Destroy (g.attacker);
			}
			enabled = false;
		}
	}
	
	Vector3 GetNewTarget(ActionObject a)
	{
		Vector3 currentLocation = a.pos;
		
		if (currentLocation.x == targetPos.x) // In a vertical line
			return new Vector3 (targetPos.x, 50+targetPos.y, Utility.Z);
		else if (currentLocation.y == targetPos.y) // In a horizontal line
			return new Vector3 (50+targetPos.x, targetPos.y, Utility.Z);

		float slope = (targetPos.y - currentLocation.y) / (targetPos.x - currentLocation.x);

		float diff = 10f;

		if (currentLocation.x < targetPos.x) // left
			return new Vector3 (targetPos.x - diff, targetPos.y - (slope * diff), Utility.Z);
		else // right
			return new Vector3 (targetPos.x + diff, targetPos.y + (slope * diff), Utility.Z);
	}
}