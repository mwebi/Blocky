using UnityEngine;
using System.Collections;

public class resetButton : MonoBehaviour {

	gamelogic gameLogicScript;
	Vector3 originalScale;
	
	float timeIntervall = 0.01f;
	float timePassed = 0.0f;
	public bool AutoMode = false;
	
	// Use this for initialization
	void Start () {
		gameLogicScript = GameObject.Find("GameLogic").GetComponent<gamelogic>();
		originalScale=transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(AutoMode){
			timePassed+= Time.deltaTime;
			if(timePassed> timeIntervall){
				timePassed=0;
				gameLogicScript.Reset();
			}
		}
	}
	
	void OnMouseUp() {
		gameLogicScript.Reset();
    }
	void OnMouseOver (){
		transform.localScale = originalScale*1.2f;	
	}
	void OnMouseExit (){
		transform.localScale = originalScale;	
	}
}
