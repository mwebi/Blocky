using UnityEngine;
using System.Collections;


public enum CubeType
{
	red,
    blue,
	green,
	black,
	white,
	none
}


public class Cube: MonoBehaviour {
	
	
	public CubeType cubeType;
	Vector3 originalScale;
	GameObject gameLogic;
	gamelogic gameLogicScript;
	
	public GameObject playerThisCubeBelongsTo;
	
	public bool isPicked = false;
	
	// Use this for initialization
	void Start () {
		originalScale=transform.localScale;
		gameLogic = GameObject.Find("GameLogic");
		gameLogicScript = gameLogic.GetComponent<gamelogic>();
	}
	
	public void Reset () {
		isPicked = false;
		transform.localScale = originalScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(!gameLogicScript.roundActive || gameLogicScript.gameEnded)
			return;
		
		if(cubeType==CubeType.red && Input.GetKeyUp("a") && !playerThisCubeBelongsTo.GetComponent<PlayerScript>().isAI)
			gotPicked ();
		else if(cubeType==CubeType.blue && Input.GetKeyUp("s") && !playerThisCubeBelongsTo.GetComponent<PlayerScript>().isAI)
			gotPicked ();
		else if(cubeType==CubeType.green && Input.GetKeyUp("d") && !playerThisCubeBelongsTo.GetComponent<PlayerScript>().isAI)
			gotPicked ();
		else if(cubeType==CubeType.black && Input.GetKeyUp("f") && !playerThisCubeBelongsTo.GetComponent<PlayerScript>().isAI)
			gotPicked ();
		else if(cubeType==CubeType.white && Input.GetKeyUp("g") && !playerThisCubeBelongsTo.GetComponent<PlayerScript>().isAI)
			gotPicked ();
	}
 	public void gotPicked () {   
		isPicked = true;
		
		transform.localScale = originalScale*1.3f;
		gameLogicScript.cubePicked(cubeType);
	}
	void OnMouseOver (){
		if(!isPicked)
			if(playerThisCubeBelongsTo == gameLogicScript.activePlayer && !gameLogicScript.activePlayer.GetComponent<PlayerScript>().hasPickedACube)
				transform.localScale = originalScale*1.2f;
	}
	void OnMouseExit (){
		if(!isPicked)
			if(playerThisCubeBelongsTo == gameLogicScript.activePlayer && !gameLogicScript.activePlayer.GetComponent<PlayerScript>().hasPickedACube)
				transform.localScale = originalScale;
	}
	void OnMouseUp() {
		if(!isPicked)
			if(playerThisCubeBelongsTo == gameLogicScript.activePlayer && !gameLogicScript.activePlayer.GetComponent<PlayerScript>().hasPickedACube)
				gotPicked();
    }
}
