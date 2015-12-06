using UnityEngine;
using System.Collections;
using System;

public enum turn {LEFT, RIGHT, START, END};

public class GrowingTeamGame : MonoBehaviour
{
	public static float WINNER_SCALE = 1.5f;
	private static float Y_VAL = 2.5f; // the horizonal location of all objects in the game
	
	turn turnState = turn.START; // which fish should grow next? 

	public Vector3 clickedPos = new Vector3 (-100, -100, -100);
	
	public GameObject left;		// the left fish in the game
	public GameObject right;	// the right fish in the game

	private DateTime last;		// the last time that a fish has grown or shrunk
	
	// locations for the fish on and off screen
	private Vector3 offscreenLeft  = new Vector3 (-5f,   Y_VAL, Utility.Z);
	private Vector3 offscreenRight = new Vector3 (15f,   Y_VAL, Utility.Z);
	private Vector3 onscreenLeft   = new Vector3 (1.25f, Y_VAL, Utility.Z);
	private Vector3 onscreenRight  = new Vector3 (8.75f, Y_VAL, Utility.Z);
	
	// Runs on the beginning of instantiation of this class
	void Start() {
		
		// Initialize each of the fish in the game
		left = Instantiate (left);
		right = Instantiate (right);
		Utility.InitializeFish (left, offscreenLeft);
		Utility.InitializeFish (right, offscreenRight);

		MoveOnScreen ();
		
		UpdateTime ();			 // set the initial time of the game
	}
	
	void OnEnable()
	{	
		MoveOnScreen ();
		turnState = turn.START;
		
		RescaleFish ();
	}
	
	// Runs once per frame
	void Update()
	{
		Animation a = left.GetComponent<Animation> ();
		if (!a.isPlaying) {
//			print ("not playing");
//			left.GetComponent<Whale>().Swim();
		}

		// If it hasn't been half a second since the last action, don't do anything
		if ((DateTime.Now - last).TotalSeconds < 0.5f)
			return;
		
		// If a fish has already been clicked, 
		if (turnState == turn.LEFT || turnState == turn.RIGHT) {
			
			// set grower to be the fish whose turn it is to grow
			ActionObject grower = (turnState == turn.LEFT) ? left.GetComponent<ActionObject> () : right.GetComponent<ActionObject> ();
			// set shrinker to be the fish whose turn it isn't to grow (it will shrink if clicked)
			ActionObject shrinker = (turnState == turn.RIGHT) ? left.GetComponent<ActionObject> () : right.GetComponent<ActionObject> ();
			
			// If the proper fish is clicked on, grow it
			if (grower.ClickedOn (clickedPos))
				Grow (grower, (turnState == turn.LEFT) ? turn.RIGHT : turn.LEFT);
			
			// If the improper whale is clicked on, shrink both whales
			else if (shrinker.ClickedOn (clickedPos))
				Shrink (grower, shrinker);
			
			// If both of the fish are of the winning scale, the user wins!
			if (WinningScale (grower) && WinningScale (shrinker)) {
				turnState = turn.END;
				MoveOffScreen();
			}
		}
		
		// No fish has been clicked yet
		else if (turnState == turn.START) {
			
			// Get the ActionObject components of each of the fish
			ActionObject left_ob = left.GetComponent<ActionObject> ();
			ActionObject right_ob = right.GetComponent<ActionObject> ();
			
			// Grow a fish if it is clicked on
			if (left_ob.ClickedOn (clickedPos))
				Grow (left_ob, turn.RIGHT);
			else if (right_ob.ClickedOn (clickedPos))
				Grow (right_ob, turn.LEFT);
		}
		else if (turnState == turn.END) {}
	}
	
	void RescaleFish()
	{
		Vector3 s = left.GetComponent<ActionObject> ().scale;
		left.GetComponent<ActionObject> ().scale = new Vector3 (s.x / WINNER_SCALE, s.y / WINNER_SCALE, s.z / WINNER_SCALE);
		s = right.GetComponent<ActionObject> ().scale;
		right.GetComponent<ActionObject> ().scale = new Vector3 (s.x / WINNER_SCALE, s.y / WINNER_SCALE, s.z / WINNER_SCALE);
	}
	
	void MoveOnScreen()
	{
		Utility.MoveHelper (left, onscreenLeft, right, onscreenRight);
	}
	
	void MoveOffScreen()
	{
		Utility.MoveHelper (left, offscreenLeft, right, offscreenRight);
	}
	
	void Grow(ActionObject item, turn new_state)
	{	
		item.Grow (1.05f);
		
		turnState = new_state;
		
		UpdateTime ();
	}
	
	void Shrink(ActionObject item1, ActionObject item2)
	{	
		ShrinkHelper (item1);
		ShrinkHelper (item2);
		
		UpdateTime ();
	}
	
	void ShrinkHelper(ActionObject item)
	{
		if (item.scale [0] > 0.5)
			item.Grow (0.95f);
	}
	
	bool WinningScale(ActionObject item)
	{
		return item.scale [0] >= WINNER_SCALE;
	}
	
	void UpdateTime()
	{
		last = DateTime.Now;
	}
}