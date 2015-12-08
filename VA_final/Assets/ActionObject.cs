﻿using UnityEngine;
using System.Collections;

public class ActionObject : MonoBehaviour {
	
	private const float INCREASE_FACTOR = 1.01f;	// The rate at which the object grows
	private const float DECREASE_FACTOR = 0.99f;	// The rate at which the object shrinks
	private const float NORMAL_SIZE = 1.0f;		// The normal scale of an object
	private const int OBJECT_RADIUS = 50;			// How far around an object is considered touching the object
	
	private Vector3 targetLocation;		   // Where this object is aiming to go
	private Quaternion targetRotation; 	   // Which direction this object is aiming to turn
	private float speed;				   // How fast the object should move in a direction
	private bool moving;				   // If the object is moving or not
	private bool rotatedInPlace = false;   // If the object has been rotated into a reasonable direction while standing still
	private Vector3 directionOfMotion;	   // The direction that the object should be facing
	
	private bool destroyable = true;	   // If the object should be destroyed when it is outside the screen
	private bool rotatable = true;		   // If the object should could be rotated
	
	public virtual void Awake()
	{
		targetLocation = pos = Utility.GetRandomVector (15);
		speed = Random.Range (5,8);

		SetRotation ();

		SetScale ();
	}
	
	// to initialize the location of an object, call Insantiate(x); followed by x.Initialize(position, speed);
	public virtual void Initialize (Vector3 pos_, float speed_)
	{
		targetLocation = pos = pos_;
		speed = speed_;
	}
	
	// Move the object towards the 
	public void Update()
	{
		// If the object isn't at its target location, move towards it
		if (!Utility.V3Equal (pos, targetLocation)) {
			moving = true;
			MoveTowardsTarget (targetLocation);
			rotatedInPlace = false;
			directionOfMotion = targetLocation - pos;
			if (tag == "whale")
				directionOfMotion = -directionOfMotion;
		} else {
			if (!rotatedInPlace) {
				SetRotation ();
			}
			moving = false;
		}

		// If the object isn't at its target rotation, rotate towards it
		float rotationAngle = Quaternion.Angle (rotation, targetRotation);
		if (!moving) {
			if (rotationAngle > 2.0f && rotationAngle < 178.0f)
				RotateTowardsTarget ();
		} else {
			RotateTowardsTarget ();
		}

		// If the object is outside of the frame of the camera
		if (OutsideCamera () && destroyable) {
			Destroy (gameObject);
			Debug.Log ("Object destroyed!");
		}
	}

	// Sets the value of destroyable to false so that the object will not be destroyed when it is outside the camera
	public void MakeUndestroyable()
	{
		destroyable = false;
	}

	// Sets the value of rotatable to false so that the object will never be rotated
	public void MakeUnrotateable()
	{
		rotatable = false;
	}
	
	public bool IsMoving()
	{
		return moving;
	}

	// Scales the object by increase 
	public void Grow (float increase=INCREASE_FACTOR)
	{
		scale = scale * increase;
	}

	// Scales the object by decrease
	public void Shrink (float decrease=DECREASE_FACTOR)
	{
		Grow (decrease);
	}

	// Resets the scale to size
	public void ResetScale(float size=NORMAL_SIZE)
	{
		scale = new Vector3 (size, size, size);
	}

	// Moves towards the target, and sets the object to move towards the targetPosition
	public void MoveTowardsTarget(Vector3 targetPosition) {
		pos = Vector3.MoveTowards (pos, targetPosition, speed*Time.deltaTime);

		moving = true;
		targetLocation = targetPosition;
	}
	
	public Vector3 PositionOnScreen()
	{
		return Camera.main.WorldToScreenPoint (pos);
	}

	// Returns true if the object is clicked on by the mouse or by the Kinect
	public bool ClickedOn(Vector3 clickedPos)
	{
		// Kinect is enabled
		if (Utility.kinectClickedOn) {
			// Scale the position down to 1 by 1
			Vector3 scaledPos = PositionOnScreen ();
			scaledPos.x /= 10f;
			scaledPos.y /= 10f;

			// If that position is pressed close to the object's position, return true
			if (Vector3.Distance (scaledPos, clickedPos) < OBJECT_RADIUS) {
				print ("Action object clicked on through Kinect");
				print ("clickedLocation: " + clickedPos);
				return true;
			}
		}

		// Mouse is not being clicked
		if (!Input.GetMouseButton (0))
			return false;

		// Return if the mouse is clicked close to the object's position
		return (Vector3.Distance (PositionOnScreen (), Input.mousePosition) < OBJECT_RADIUS);
	}

	private bool OutsideCamera(float shift = Utility.OFFSET)
	{
		float max = 10 + shift;
		float min = 0 - shift;
		return (pos.x > max ||
		        pos.x < min ||
		        pos.y > max ||
		        pos.y < min);
	}

	// Sets the scale for each object based on the specfic prefab and how big it should be
	private void SetScale()
	{
		switch (tag) {
		case "whale":
			scale = new Vector3 (1f, 1f, 1f);
			break;
		case "cruscarp":
			scale = new Vector3 (10f, 10f, 10f);
			break;
		default:
			break;
		}
	}

	// Sets the rotation of the object either to a resting rotation or towards the target location
	private void RotateTowardsTarget()
	{
		if (!rotatable)
			return;
		targetRotation = Quaternion.LookRotation(directionOfMotion);
		Vector3 temp = Vector3.RotateTowards(transform.forward, directionOfMotion, 100*Mathf.Deg2Rad*Time.deltaTime, 0.0f);
		rotation = Quaternion.LookRotation (temp);
	}
	
	private void SetRotation() 
	{
		if (!rotatable)
			return;
		
		// A rotation has already been set in place
		if (rotatedInPlace)
			return;
		
		rotatedInPlace = true;
		
		float offset = Random.value > 0.5f ? -100f : 100f;
		Vector3 temp = new Vector3(pos.x + offset, pos.y, pos.z) - pos;
		directionOfMotion = temp - pos;
		if (tag == "whale")
			directionOfMotion = -directionOfMotion;
		
		targetRotation = Quaternion.LookRotation(directionOfMotion);
	}

	/* Getters & Setters */
	public Vector3 scale
	{
		get {
			return (transform.localScale);
		} set {
			transform.localScale = value;
		}
	}
	public Vector3 pos {
		get {
			return (transform.position);
		}
		set {
			transform.position = value;
		}
	}
	public Quaternion rotation {
		get {
			return (transform.rotation);
		}
		set {
			transform.rotation = value;
		}
	}
}