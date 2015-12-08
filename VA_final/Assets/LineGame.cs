using UnityEngine;
using System.Collections;

public enum objectState {NORMAL, MOVINGTO, DONE, SHOULD_DIVE, DIVING, RESTART};
public class whaleWithState {
	public GameObject whale;
	public objectState state;
	public Vector3 targetPos;
	public Vector3 diveTargetPos;
	public int timer = 1000;
	public whaleWithState(GameObject w, objectState s, Vector3 targetPosition, Vector3 diveTargetPosition) {
		whale = w;
		state = s;
		targetPos = targetPosition;
		diveTargetPos = diveTargetPosition;
	}
}

public class LineGame : MonoBehaviour {
	public static int count = 0;
	public static int numObjects = 4; 
	public static int lineCount = 0;
	
	public Vector3 targetPos;
	public Vector3 diveTargetPos = new Vector3 (0, 0, 0);
	
	public Vector3 offscreenPos = new Vector3 (-14,0,0);
	
	public Vector3 onscreenPos;
	
	//	private AquariumMusic music;  // how this module plays music in the application
	
	public Vector3 clickedPos = new Vector3 (-100, -100, -100); //kevin
	//public bool kinectClickedOn = false;
	
	public Vector3 whalePos;
	
	public GameObject[] ws;
	
	public whaleWithState[] whaleList = new whaleWithState[4];
	
	void Start() {
		Debug.Log ("Start");
		
		targetPos = new Vector3(2.7f,2.5f,0);
		for (int i = 0; i < 4; ++i) {
			whaleList [i] = new whaleWithState (Instantiate(ws[i]), objectState.NORMAL, targetPos, diveTargetPos);
			whaleList[i].whale.GetComponent<ActionObject>().MakeUndestroyable();
			Debug.Log ("Instantiate whale");
			targetPos.x = targetPos.x + 1.5f;
		}
		
		foreach (whaleWithState w in whaleList) {
			w.state = objectState.NORMAL;
			onscreenPos = Utility.GetRandomVector(2.5f, 7.5f, 0f, 5f);
			w.whale.GetComponent<ActionObject>().MoveTowardsTarget(onscreenPos);
		}
	}
	
	
	void Update() {
		foreach (whaleWithState w in whaleList) {
			ActionObject script = w.whale.GetComponent<ActionObject>();
			switch (w.state) {
			case objectState.NORMAL:
				/*kinectClickedOn, clickedPos*/
				if (script.ClickedOn (clickedPos)) {
					w.state = objectState.MOVINGTO;
					script.MoveTowardsTarget(w.targetPos);
					break;
				}
				break;
			case objectState.MOVINGTO:
				//print ("MOVING TO");
				if (Utility.V3Equal(script.pos, w.targetPos)) {
					lineCount++;
					if (lineCount == numObjects) {
						//all objects must dive
						//							music.PlayFeedback(music.neg);
						foreach (whaleWithState item in whaleList) {
							item.state = objectState.SHOULD_DIVE;
						}
					} 
					else {
						w.state = objectState.DONE;
					}
				}
				break;
			case objectState.SHOULD_DIVE:
				//print ("DIVING");
				////w.diveTargetPos.x = w.targetPos.x - 0.5f;
				//w.diveTargetPos.y = w.targetPos.y - 1;
				//script.MoveTowardsTarget(w.diveTargetPos);
				//w.whale.GetComponent<Animation>()["dive"].wrapMode=WrapMode.Once;
				w.whale.GetComponent<Whale>().Dive();
				//GetComponent<ActionObject>().shouldMove = false;
				w.state = objectState.DIVING;
				lineCount--;
				break;
			case objectState.DIVING:
				print ("IS DIVING");
				//if (Utility.V3Equal(script.pos, w.diveTargetPos)) {
				if (!w.whale.GetComponent<Animation>().IsPlaying("dive")) {
					Debug.Log ("Dive complete");
					lineCount++;
				}
				if (lineCount == numObjects) {
					//all objects must dive
					foreach (whaleWithState item in whaleList) {
						item.state = objectState.RESTART;
					}
				} 
				//}
				break;
			case objectState.DONE:
				break;
			case objectState.RESTART:
				Debug.Log ("RESTART");
				//GetComponent<ActionObject>().shouldMove = true;
				targetPos = new Vector3(2.7f,2.5f,0);
				foreach (whaleWithState item in whaleList) {
					lineCount = 0;
					onscreenPos = Utility.GetRandomVector(2.5f, 7.5f, 0f, 5f);
					item.whale.GetComponent<ActionObject>().MoveTowardsTarget(onscreenPos);
					item.targetPos = targetPos;
					targetPos.x = targetPos.x + 1.5f; //update x
					item.state = objectState.NORMAL;
				}
				break;
			}
		}
		//kinectClickedOn = false;
	}
	
	//	void SetFeedbackAudio() {
	//		if (!audioIsPlaying) {
	//			print ("play sound");
	//			audio.clip = positive;
	//			audio.loop = false;
	//			audio.Play ();
	//			audioIsPlaying = true;
	//		}
	//	}
}