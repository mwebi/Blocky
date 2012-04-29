using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	
	public CubeType pickedCube;
	public CubeType previousCube = CubeType.none;
	public bool hasPickedACube = false;
	public bool isAI = false;
	public bool isAINgram = false;
	public bool isAIPattern = false;
	public bool isAIRandom = false;
	public bool doDebugLogs = false;
	
	public int NgramWindowSize = 2;
	
	gamelogic gameLogicScript;
	
	public int roundsWon = 0;
	public int OverallScore = 0;
	public int BonusScore = 0;
	
	public GameObject RoundsWonTextNumber;
	public GameObject ScoreTextNumber;
	public GameObject BonusScoreTextNumber;
	
	GameObject CurrentPrevCube;
	public GameObject PrevCubeRed;
	public GameObject PrevCubeBlue;
	public GameObject PrevCubeGreen;
	public GameObject PrevCubeBlack;
	public GameObject PrevCubeWhite;

    public CubeType[] previousCubePicks; //[0] is oldest, [ARRAYSIZE-1] newest
	
	public NGramPredictor ngramPredictor;
	private PlayerScript otherPlayerScript;
	// Use this for initialization
	void Start () {
		previousCube = CubeType.none;
		gameLogicScript = GameObject.Find("GameLogic").GetComponent<gamelogic>();

        
		if(isAINgram){
        	ngramPredictor = new NGramPredictor();
        	ngramPredictor.Start();
        	ngramPredictor.nValue = NgramWindowSize + 1;
			ngramPredictor.doDebugLogs = doDebugLogs;
			ngramPredictor.playerThisNgramPredictorBelongsTo = this.gameObject.ToString();
		}
		
		otherPlayerScript = gameLogicScript.returnOtherPlayer(this);
		//track previous actions if other player uses ngrams
		if(otherPlayerScript.isAINgram){
			previousCubePicks = new CubeType[otherPlayerScript.NgramWindowSize + 1];
			
			for (int i = 0; i < previousCubePicks.Length; i++)
            	previousCubePicks[i] = CubeType.none;
		}
	}
	
	public void updateScoreText(){
		RoundsWonTextNumber.GetComponent<TextMesh>().text = roundsWon.ToString();
		ScoreTextNumber.GetComponent<TextMesh>().text = OverallScore.ToString();
		BonusScoreTextNumber.GetComponent<TextMesh>().text = BonusScore.ToString();
	}
	
	public void Reset(){
		updatePreviousCubePicks();
		hasPickedACube = false;
		
		previousCube = pickedCube;
		instantiatePrevCube();
		pickedCube = CubeType.none;
	}
	
	void instantiatePrevCube(){
		Destroy(CurrentPrevCube);
		switch(pickedCube) {
		    case CubeType.red:
		        CurrentPrevCube = Instantiate(PrevCubeRed) as GameObject;
		        break;
			
		    case CubeType.blue:
				CurrentPrevCube = Instantiate(PrevCubeBlue) as GameObject;
		        break;
			
		    case CubeType.green:
		        CurrentPrevCube = Instantiate(PrevCubeGreen) as GameObject;
		        break;
			
		    case CubeType.black:
		        CurrentPrevCube = Instantiate(PrevCubeBlack) as GameObject;
		        break;
			
			case CubeType.white:
		        CurrentPrevCube = Instantiate(PrevCubeWhite) as GameObject;
		        break;
			
		    default:
	        	Debug.Log("default called, something went wrong");
				Debug.Break();
				break;
		}	
	}
	public void pickCubeByAI () {
        if(isAINgram)
			pickCubeByNGramAI();
		else if(isAIPattern)
			pickCubeByPattern();
		else if(isAIRandom)
			pickCubeByRandom();
	}
	
	private void pickCubeByRandom () {
        
		pickedCube = (CubeType)Random.Range(0, 5);
		if(doDebugLogs)
			Debug.Log(transform.gameObject.ToString() +  " picked Cube By Random: " + pickedCube);

        //updatePreviousCubePicks(pickedCube);
		hasPickedACube = true;
	}
	
	private void pickCubeByPattern () {

		switch(previousCube) {
	    	case CubeType.red:
				pickedCube = CubeType.blue;	
				break;
			case CubeType.blue:
				pickedCube = CubeType.green;	
				break;
			case CubeType.green:
				pickedCube = CubeType.black;	
				break;
			case CubeType.black:
				pickedCube = CubeType.white;	
				break;
			case CubeType.white:
				pickedCube = CubeType.red;	
				break;
			case CubeType.none:
				pickedCube = CubeType.red;	
				break;
			default:
	        	Debug.Log("default called, something went wrong");
				Debug.Break();
				pickedCube = CubeType.red;	
				break;
		}
		if(doDebugLogs)
			Debug.Log(transform.gameObject.ToString() +  " picked Cube By Pattern: " + pickedCube);

        //updatePreviousCubePicks(pickedCube);
		hasPickedACube = true;
	}
    
	CubeType[] getOtherPlayersLastActions()
    {
		CubeType[] lastActions = new CubeType[ngramPredictor.nValue - 1]; //lastactions are the previous actions without the current one
        for (int i = 0; i < ngramPredictor.nValue - 1; i++)
            lastActions[i] = otherPlayerScript.previousCubePicks[i+1];
        
		return lastActions;
    }
	
	private void pickCubeByNGramAI () {
        CubeType predictedOtherPlayerPick = ngramPredictor.getMostLikely(getOtherPlayersLastActions());
        if(doDebugLogs)
			Debug.Log(transform.gameObject.ToString() + " predicted: " + predictedOtherPlayerPick);
		
		//if no prediction was made, make a random guess
		if(predictedOtherPlayerPick == CubeType.none){
			pickedCube = (CubeType)Random.Range(0, 5);
			if(doDebugLogs)
				Debug.Log(transform.gameObject.ToString() + " picked Cube by Ngram, fallback to random: " + pickedCube);
		}
		else{
			pickedCube = pickCounterCube(predictedOtherPlayerPick);
			if(doDebugLogs)
				Debug.Log(transform.gameObject.ToString() + " picked Cube by Ngram, counter picked: " + pickedCube);
		}
		
        //updatePreviousCubePicks(pickedCube);
		hasPickedACube = true;
	}
	public CubeType pickCounterCube (CubeType pickToCounter){
		switch(pickToCounter) {
	    	case CubeType.red:
		        if(Random.Range(0, 1) == 0)
					return CubeType.black;
				else
					return CubeType.green;
			case CubeType.blue:
		        if(Random.Range(0, 1) == 0)
					return CubeType.red;
				else
					return CubeType.white;
			case CubeType.green:
		        if(Random.Range(0, 1) == 0)
					return CubeType.blue;
				else
					return CubeType.black;
			case CubeType.black:
		        if(Random.Range(0, 1) == 0)
					return CubeType.blue;
				else
					return CubeType.white;
			case CubeType.white:
		        if(Random.Range(0, 1) == 0)
					return CubeType.green;
				else
					return CubeType.red;
			default:
	        	Debug.Log("default called, something went wrong");
				Debug.Break();
				return CubeType.none;
		}
	}
    public void pickedACube(CubeType pickedType)
    {
        pickedCube = pickedType;
        //updatePreviousCubePicks(pickedCube);
        hasPickedACube = true;
    }

    public void updatePreviousCubePicks()
    {
        //dont do anything if other player doesnt use ngrams
		if(!otherPlayerScript.isAINgram)
			return;
		
		//resort array
        for (int i = 0; i < previousCubePicks.Length - 1; i++)
            previousCubePicks[i] = previousCubePicks[i + 1];
        
        //previousCubePicks[0] = previousCubePicks[1];
        //previousCubePicks[1] = previousCubePicks[2];        
        
        //save current pick
        previousCubePicks[previousCubePicks.Length - 1] = pickedCube;
    }
}
	
