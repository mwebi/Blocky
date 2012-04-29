using UnityEngine;
using System.Collections;

public enum RoundResult
{
	player1Won,
    player2Won,
	draw
}

public class gamelogic : MonoBehaviour {
	
	GameObject player1;
	PlayerScript player1Script;
	public GameObject[] player1Blocks = new GameObject[5];
	
	GameObject player2;
	PlayerScript player2Script;
	public GameObject[] player2Blocks = new GameObject[5];
	
	public GameObject activePlayer;
	
	GameObject resultText;
	TextMesh comboText;
	
	int[,] winLooseMatrix = new int[5, 5] {
			// Red, Blue, Green, Black, White
		/*Red*/{ 0, 2, 1, 1, 2 }, 
	   /*Blue*/{ 1, 0, 2, 2, 1 },
	  /*Green*/{ 2, 1, 0, 1, 2 },
	  /*Black*/{ 2, 1, 2, 0, 1 },
	  /*White*/{ 1, 2, 1, 2, 0 }
	};
		
	public bool roundActive = true;
	public bool gameEnded = false;
	public RoundResult currentRoundResult;

 	public bool AutoMode = true;   
 	public float AutoModeDelay = 0.5f;   
	
	public int MaxScore = 10;
	
	public int GamesToPlay = 100;

	public float sumOfGamesWonP1 = 0;
	public float sumOfroundsWonP1 = 0;
	public float sumOfOverallScoreP1 = 0;
	public float sumOfBonusScoreP1 = 0;
	
	public float sumOfGamesWonP2 = 0;
	public float sumOfroundsWonP2 = 0;
	public float sumOfOverallScoreP2 = 0;
	public float sumOfBonusScoreP2 = 0;
	
	
	saveData dataObject;
	
	void Awake(){
		
	}
	
	// Use this for initialization
	void Start () {
		
		player1 = GameObject.Find("player1");
		player1Script = player1.GetComponent<PlayerScript>();
			
		player2 = GameObject.Find("player2");
		player2Script = player2.GetComponent<PlayerScript>();
		
		activePlayer = player1;
		
		comboText = GameObject.Find("ComboText").GetComponent<TextMesh>();
		resultText = GameObject.Find("ResultText");
		
		foreach (GameObject cube in player1Blocks) {
			cube.GetComponent<Cube>().playerThisCubeBelongsTo = player1;
		}
		foreach (GameObject cube in player2Blocks) {
			cube.GetComponent<Cube>().playerThisCubeBelongsTo = player2;
		}
		
		GameObject.Find("ScoreNeeded").GetComponent<TextMesh>().text = "Score needed to win: " + MaxScore;
		
		if(AutoMode){
			GameObject.Find("ResetButton").SetActiveRecursively(false);
		}
		
		if(GameObject.Find("dataObjectOriginal") != null)
			dataObject = GameObject.Find("dataObjectOriginal").GetComponent<saveData>();
		else
			dataObject = GameObject.Find("dataObject").GetComponent<saveData>();
		
		//if(dataObject.isCopy)
			//dataObject = GameObject.Find("dataObjectOriginal").GetComponent<saveData>();
		//if(dataObject ==null)
			//dataObject = GameObject.Find("dataObjectOriginal").GetComponent<saveData>();
		
		//if(dataObject.firstRound && dataObject.gameObject.name != "dataOb)
			//Destroy(dataObject.gameObject);
		gameEnded = false;
		
	}


	void Update () {
		if(!roundActive || gameEnded)
			return;
				
		//let AI pick
		if(activePlayer.GetComponent<PlayerScript>().isAI){
			if(activePlayer == player1){
            	player1Script.pickCubeByAI();
				
				player1Blocks[(int)activePlayer.GetComponent<PlayerScript>().pickedCube].GetComponent<Cube>().gotPicked();
				activePlayer = player2;
			}else if(activePlayer == player2){
                player2Script.pickCubeByAI();

				player2Blocks[(int)activePlayer.GetComponent<PlayerScript>().pickedCube].GetComponent<Cube>().gotPicked();
				activePlayer = player1;
			}
		}
		
		//check if everyone has picked, if so end the round, determine winner and reset
		if(player1Script.hasPickedACube == true && player2Script.hasPickedACube == true){
			//determineWinner1();
			determineWinner();
			updatePlayerScoreTexts();
			roundActive= false;
			if(AutoMode){
				if(AutoModeDelay > 0.001)
					StartCoroutine( ResetAfterDelay(AutoModeDelay) );
				else
					Reset();
			}
		}
		
	}
	IEnumerator ResetAfterDelay(float delay){
		yield return new WaitForSeconds(delay);
		Reset();
	}
	void StartNewMatch(){
		dataObject.firstRound = false;
		dataObject.gameObject.name = "dataObjectOriginal";
		Application.LoadLevel("game");
	}
	void checkEndOfGame(){
		bool StartNew= true;
		
		if(player1Script.OverallScore >= MaxScore){
			resultText.GetComponent<TextMesh>().text = "Result: Player 1 won the game";
			dataObject.sumOfGamesWonP1 += 1;
			
			dataObject.sumOfroundsWonP1 += player1Script.roundsWon;
			dataObject.sumOfOverallScoreP1 += player1Script.OverallScore;
			dataObject.sumOfBonusScoreP1 += player1Script.BonusScore;
			
			dataObject.sumOfroundsWonP2 += player2Script.roundsWon;
			dataObject.sumOfOverallScoreP2 += player2Script.OverallScore;
			dataObject.sumOfBonusScoreP2 += player2Script.BonusScore;
			
			if(player1Script.isAINgram){
				dataObject.P1RegisterTime += player1Script.ngramPredictor.averageTimeTakenForRegistering;
				dataObject.P1PredictionTime += player1Script.ngramPredictor.averageTimeTakenForPredicting;
				
				Debug.Log("average register time P1: " + player1Script.ngramPredictor.averageTimeTakenForRegistering);
				Debug.Log("average prediction time P1: " + player1Script.ngramPredictor.averageTimeTakenForPredicting);
			}
			if(player2Script.isAINgram){
				dataObject.P2RegisterTime += player2Script.ngramPredictor.averageTimeTakenForRegistering;
				dataObject.P2PredictionTime += player2Script.ngramPredictor.averageTimeTakenForPredicting;
				
				Debug.Log("average register time P2: " + player2Script.ngramPredictor.averageTimeTakenForRegistering);
				Debug.Log("average prediction time P2: " + player2Script.ngramPredictor.averageTimeTakenForPredicting);
			}
			
			gameEnded = true;
			
		}
		else if(player2Script.OverallScore >= MaxScore){
			resultText.GetComponent<TextMesh>().text = "Result: Player 2 won the game";
			dataObject.sumOfGamesWonP2 += 1;
			
			dataObject.sumOfroundsWonP1 += player1Script.roundsWon;
			dataObject.sumOfOverallScoreP1 += player1Script.OverallScore;
			dataObject.sumOfBonusScoreP1 += player1Script.BonusScore;
			
			dataObject.sumOfroundsWonP2 += player2Script.roundsWon;
			dataObject.sumOfOverallScoreP2 += player2Script.OverallScore;
			dataObject.sumOfBonusScoreP2 += player2Script.BonusScore;
			
			if(player1Script.isAINgram){
				dataObject.P1RegisterTime += player1Script.ngramPredictor.averageTimeTakenForRegistering;
				dataObject.P1PredictionTime += player1Script.ngramPredictor.averageTimeTakenForPredicting;
				
				Debug.Log("average register time P1: " + player1Script.ngramPredictor.averageTimeTakenForRegistering);
				Debug.Log("average prediction time P1: " + player1Script.ngramPredictor.averageTimeTakenForPredicting);
			}
			if(player2Script.isAINgram){
				dataObject.P2RegisterTime += player2Script.ngramPredictor.averageTimeTakenForRegistering;
				dataObject.P2PredictionTime += player2Script.ngramPredictor.averageTimeTakenForPredicting;
				
				Debug.Log("average register time P2: " + player2Script.ngramPredictor.averageTimeTakenForRegistering);
				Debug.Log("average prediction time P2: " + player2Script.ngramPredictor.averageTimeTakenForPredicting);
			}
			
			gameEnded = true;
		}
		

		
		if(dataObject.sumOfGamesWonP1+dataObject.sumOfGamesWonP2 >= GamesToPlay){
			Debug.Log("sumOf roundsWonP1: " + dataObject.sumOfroundsWonP1);
			Debug.Log("sumOf BonusScoreP1: " + dataObject.sumOfBonusScoreP1);
			Debug.Log("sumOf OverallScoreP1: " + dataObject.sumOfOverallScoreP1);
			Debug.Log("sumOf GamesWonP1: " + dataObject.sumOfGamesWonP1);
			
			Debug.Log("sumOf roundsWonP2: " + dataObject.sumOfroundsWonP2);
			Debug.Log("sumOf BonusScoreP2: " + dataObject.sumOfBonusScoreP2);
			Debug.Log("sumOf OverallScoreP2: " + dataObject.sumOfOverallScoreP2);
			Debug.Log("sumOf GamesWonP2: " + dataObject.sumOfGamesWonP2);
			

			
			gameEnded = true;
			StartNew = false;
		}
		
		if(gameEnded && StartNew)
			StartNewMatch();
		
	}
		
	private void registerSequencesOfPlayers(){
		//register the last actions of the other player
		if(player1Script.isAINgram)
			player1Script.ngramPredictor.registerSequence(player2Script.previousCubePicks);
		if(player2Script.isAINgram)
			player2Script.ngramPredictor.registerSequence(player1Script.previousCubePicks);
	}
	public PlayerScript returnOtherPlayer(PlayerScript callingPlayer){
		
		if(callingPlayer == player1Script)
			return player2Script;
		if(callingPlayer == player2Script)
			return player1Script;
		
		Debug.Log("Couldn't return other player script");
		Debug.Break();
		return player1Script;
	}
	
	public void cubePicked(CubeType pickedType){
		//Debug.Log(activePlayer.ToString() + " has picked cube of type: " + pickedType.ToString());
		if(activePlayer == player1 && !activePlayer.GetComponent<PlayerScript>().isAI){
			if(player1Script.doDebugLogs)
				Debug.Log("human player 1 picked: " + pickedType);
			activePlayer = player2;
            player1Script.pickedACube(pickedType);
		
		}else if(activePlayer == player2 && !activePlayer.GetComponent<PlayerScript>().isAI){
			if(player2Script.doDebugLogs)
				Debug.Log("human player 2 picked: " + pickedType);
			activePlayer = player1;
            player2Script.pickedACube(pickedType);
		}
	}
	
	/*void determineWinner1(){
		if(player1Script.pickedCube == CubeType.none || player2Script.pickedCube == CubeType.none){
			Debug.Log("player picked no cube");
			Debug.Break();
		}
		if(player1Script.pickedCube == CubeType.red){
			
		}
		switch(player1Script.pickedCube) {
		    case CubeType.red:
		        switch(player2Script.pickedCube) {
		    		case CubeType.red:
						nooneWonRound();
						break;
		    		case CubeType.blue:
						player1WonRound();
						break;
					case CubeType.green:
						player2WonRound();
						break;
					case CubeType.black:
						player2WonRound();
						break;
					case CubeType.white:
						player1WonRound();
						break;
					default:
			        	Debug.Log("default called, something went wrong");
						Debug.Break();
						break;
				}
		        break;
			
		    case CubeType.blue:
				switch(player2Script.pickedCube) {
		    		case CubeType.red:
						player2WonRound();
						break;
		    		case CubeType.blue:
						nooneWonRound();
						break;
					case CubeType.green:
						player1WonRound();
						break;
					case CubeType.black:
						player1WonRound();
						break;
					case CubeType.white:
						player2WonRound();
						break;
					default:
			        	Debug.Log("default called, something went wrong");
						Debug.Break();
						break;
				}
		        break;
			
		    case CubeType.green:
		        switch(player2Script.pickedCube) {
		    		case CubeType.red:
						player1WonRound();
						break;
		    		case CubeType.blue:
						player2WonRound();
						break;
					case CubeType.green:
						nooneWonRound();
						break;
					case CubeType.black:
						player2WonRound();
						break;
					case CubeType.white:
						player1WonRound();
						break;
					default:
			        	Debug.Log("default called, something went wrong");
						Debug.Break();
						break;
				}
		        break;
			
		    case CubeType.black:
		        switch(player2Script.pickedCube) {
		    		case CubeType.red:
						player1WonRound();
						break;
		    		case CubeType.blue:
						player2WonRound();
						break;
					case CubeType.green:
						player1WonRound();
						break;
					case CubeType.black:
						nooneWonRound();
						break;
					case CubeType.white:
						player2WonRound();
						break;
					default:
			        	Debug.Log("default called, something went wrong");
						Debug.Break();
						break;
				}
		        break;
			
			case CubeType.white:
		        switch(player2Script.pickedCube) {
		    		case CubeType.red:
						player2WonRound();
						break;
		    		case CubeType.blue:
						player1WonRound();
						break;
					case CubeType.green:
						player2WonRound();
						break;
					case CubeType.black:
						player1WonRound();
						break;
					case CubeType.white:
						nooneWonRound();
						break;
					default:
			        	Debug.Log("default called, something went wrong");
						Debug.Break();
						break;
				}
		        break;
			
		    default:
	        	Debug.Log("default called, something went wrong");
				Debug.Break();
				break;
		}	
	}*/
	
	void determineBonusScore(PlayerScript givenPlayer){
		if(player1Script.previousCube == CubeType.none || player2Script.previousCube == CubeType.none){
			//Debug.Log("no previous cube");
			return;
		}
		//no bonus if round was draw
		if(currentRoundResult == RoundResult.draw){ 
			return;
		}
			
		switch(givenPlayer.pickedCube) {
		    case CubeType.red:
		        switch(givenPlayer.previousCube) {
		    		case CubeType.blue:
						giveBonusScore(2, givenPlayer);
						break;
					case CubeType.white:
						giveBonusScore(1, givenPlayer);
						break;
				}
		        break;
			
		    case CubeType.blue:
				switch(givenPlayer.previousCube) {
		    		case CubeType.green:
						giveBonusScore(2, givenPlayer);
						break;
					case CubeType.red:
						giveBonusScore(1, givenPlayer);
						break;
				}
		        break;
			
		    case CubeType.green:
		        switch(givenPlayer.previousCube) {
		    		case CubeType.black:
						giveBonusScore(2, givenPlayer);
						break;
					case CubeType.blue:
						giveBonusScore(1, givenPlayer);
						break;
				}
		        break;
			
		    case CubeType.black:
		        switch(givenPlayer.previousCube) {
		    		case CubeType.white:
						giveBonusScore(2, givenPlayer);
						break;
					case CubeType.green:
						giveBonusScore(1, givenPlayer);
						break;
				}
		        break;
			
			case CubeType.white:
		        switch(givenPlayer.previousCube) {
		    		case CubeType.red:
						giveBonusScore(2, givenPlayer);
						break;
					case CubeType.black:
						giveBonusScore(1, givenPlayer);
						break;
				}
		        break;
			
		    default:
	        	Debug.Log("default called, something went wrong");
				Debug.Break();
				break;
		}	
	}
	
	void giveBonusScore(int bonusScoreToGive, PlayerScript toThisPlayer){
		toThisPlayer.OverallScore +=bonusScoreToGive;
		toThisPlayer.BonusScore +=bonusScoreToGive;
		
		if(bonusScoreToGive==1){
			comboText.text = "Combo! +1 extra Score";
		}else if(bonusScoreToGive==2){
			comboText.text = "MEGA Combo! +2 extra Score";
		}
	}
	
	void determineWinner(){
		if(player1Script.pickedCube == CubeType.none || player2Script.pickedCube == CubeType.none){
			Debug.Log("player picked no cube");
			Debug.Break();
		}
		
		//adapt players choise to be entered to matrix
		int player1Choice = (int)player1Script.pickedCube;
		int player2Choice = (int)player2Script.pickedCube;
		
		if(winLooseMatrix[player2Choice ,player1Choice] == 0)
			nooneWonRound();
		else if(winLooseMatrix[player2Choice ,player1Choice] == 1)
			player1WonRound();
		else if(winLooseMatrix[player2Choice ,player1Choice] == 2)
			player2WonRound();
	}
	
	void updatePlayerScoreTexts(){
		player1Script.updateScoreText();
		player2Script.updateScoreText();
	}
	
	void player1WonRound(){
		//Debug.Log("Player 1 won");
		currentRoundResult = RoundResult.player1Won;
		player1Script.roundsWon++;
		player1Script.OverallScore++;
		determineBonusScore(player1Script);
			
		resultText.GetComponent<TextMesh>().text = "Result: Player 1 won. +1 Score";
	}
	
	void player2WonRound(){
		//Debug.Log("Player 2 won");
		currentRoundResult = RoundResult.player2Won;
		player2Script.roundsWon++;
		player2Script.OverallScore++;
		determineBonusScore(player2Script);
		
		resultText.GetComponent<TextMesh>().text = "Result: Player 2 won. +1 Score";
	}
	
	void nooneWonRound(){
		//Debug.Log("Round is draw");
		currentRoundResult = RoundResult.draw;
		resultText.GetComponent<TextMesh>().text = "Result: Round is draw";
	}
	
	public void Reset(){
		if(!roundActive){
			resultText.GetComponent<TextMesh>().text = "Result:";
			
			comboText.text = "";
			
			//first update cubepicks, then register them
			player1Script.Reset();
			player2Script.Reset();
			registerSequencesOfPlayers();
			
			foreach (GameObject cube in player1Blocks) {
				cube.GetComponent<Cube>().Reset();
			}
			foreach (GameObject cube in player2Blocks) {
				cube.GetComponent<Cube>().Reset();
			}
			if(player1Script.doDebugLogs || player2Script.doDebugLogs)
				Debug.Log("--------------- NEW ROUND ---------------");
			
			checkEndOfGame();
			roundActive = true;
		}
	}
	
}

