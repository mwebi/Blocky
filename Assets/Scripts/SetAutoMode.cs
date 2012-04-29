using UnityEngine;
using System.Collections;

public class SetAutoMode : MonoBehaviour {
	
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
		gameLogicScript.AutoMode = !gameLogicScript.AutoMode;
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
		if(gameLogicScript.AutoMode)
			transform.GetComponentInChildren<TextMesh>().text = "Auto Mode : ON";
		else
			transform.GetComponentInChildren<TextMesh>().text = "Auto Mode : OFF";
	}
}
