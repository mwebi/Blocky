using UnityEngine;
using System.Collections;

public class SetAutoModeDelay : MonoBehaviour {
	
	gamelogic gameLogicScript;
	Vector3 originalScale;
	
	// Use this for initialization
	void Start () {
		gameLogicScript = GameObject.Find("GameLogic").GetComponent<gamelogic>();
		originalScale=transform.localScale;
		setAutoModeInText();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseUp() {
		if(gameLogicScript.AutoModeDelay <= 0.49) //if == 0
			gameLogicScript.AutoModeDelay = 0.5f;
		else if(gameLogicScript.AutoModeDelay <= 0.99) //if == 0.5
			gameLogicScript.AutoModeDelay = 1.0f;
		else if(gameLogicScript.AutoModeDelay <= 1.49) //if == 1
			gameLogicScript.AutoModeDelay = 1.5f;		
		else if(gameLogicScript.AutoModeDelay <= 1.99) //if == 1.5
			gameLogicScript.AutoModeDelay = 0.0f;		
		
		setAutoModeInText();
    }
	void OnMouseOver (){
		transform.localScale = originalScale*1.1f;	
	}
	void OnMouseExit (){
		transform.localScale = originalScale;	
	}
	
	void setAutoModeInText ()
	{
		transform.GetComponentInChildren<TextMesh>().text = "Auto Mode Delay: " + gameLogicScript.AutoModeDelay + " seconds";
	}
}
