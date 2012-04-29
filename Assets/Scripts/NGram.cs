using UnityEngine;
using System.Collections;

public struct KeyDataRecord{
    //Holds the counts for each successor action counts
    public Hashtable counts;
    
    //Holds the total number of times the window has been seen
    public int total;// = 0;
}

public class NGramPredictor{

	// Holds the frequency data
    Hashtable data;

	// nValue is the size of the window + 1
	public int nValue = 3; 
	
	public string playerThisNgramPredictorBelongsTo = "";
	public bool doDebugLogs;
    
		
	public double averageTimeTakenForRegistering = 0;
	public double overallTimeTakenForRegistering = 0;
	public int overallRegisters = 0;
	public double averageTimeTakenForPredicting = 0;
	public double overallTimeTakenForPredicting = 0;
	public int overallPredicts = 0;
	
	public void Start(){
        data = new Hashtable();
    }
	
	string tempName = "";
	private string returnStringName(CubeType[] actionsArray){
        tempName = "";
		for (int i = 0; i < actionsArray.Length; i++)
            tempName += actionsArray[i].ToString();
		return tempName;
    }
	
	string keyStringOfArray = "";
	// Registers a set of actions with predictor, updating
	// its data. We assume actions has exactly nValue
	// elements in it.
    public void registerSequence(CubeType[] actions)
    {
		double startTime = Time.realtimeSinceStartup;
		
		if(actions.Length != nValue){
			Debug.Log("actions.Length != nValue");
			Debug.Break();
		}
			
        // Split the sequence into a key and value for the Hashtable
		CubeType[] previousActions = new CubeType[nValue-1];
        for (int i = 0; i < nValue-1; i++)
        {
            previousActions[i] = actions[i];
        }

		CubeType currentAction = actions[nValue-1];
        
		keyStringOfArray = "";
		keyStringOfArray = returnStringName(previousActions);
		if(doDebugLogs)
			Debug.Log(playerThisNgramPredictorBelongsTo +  "registered sequence: " + keyStringOfArray + " + " + currentAction);
		
		if (!data.ContainsKey(keyStringOfArray)) 
        { 
            data[keyStringOfArray] = new KeyDataRecord();
        }

		// Get the correct data structure
		KeyDataRecord keyData = (KeyDataRecord)data[keyStringOfArray];
		
		if (keyData.counts == null)
            keyData.counts = new Hashtable();

		// Make sure we have a record for the follow on value
		if(!keyData.counts.ContainsKey(currentAction)) 
		    keyData.counts[currentAction] = 0;

		// Add to the total, and to the count for the value
		keyData.counts[currentAction] = (int)keyData.counts[currentAction] + 1;
		keyData.total += 1;
		
		data[keyStringOfArray] = keyData;
		
		double temp = Time.realtimeSinceStartup-startTime;
		overallTimeTakenForRegistering += temp;
		overallRegisters++;
		averageTimeTakenForRegistering = overallTimeTakenForRegistering / overallRegisters;
	}

	// Gets the next action most likely from the given one.
	// We assume actions has nValue - 1 elements in it (i.e.
	// the size of the window).
    public CubeType getMostLikely(CubeType[] actions)
    {
		double startTime = Time.realtimeSinceStartup;
		
		if(actions.Length != nValue-1){
			Debug.Log("actions.Length != nValue");
			Debug.Break();
		}
		
		keyStringOfArray = "";
		keyStringOfArray = returnStringName(actions);
        
		//if we don't have data for the actions, we cant predict
        if (data[keyStringOfArray] == null){
			if(doDebugLogs)
            	Debug.Log(playerThisNgramPredictorBelongsTo + ": Prediction fail: no data for this sequence: " + keyStringOfArray);
			return CubeType.none;
		}
		if(doDebugLogs)
			Debug.Log(playerThisNgramPredictorBelongsTo + " predicting sequence: " + keyStringOfArray);
		
        // Get the key data
		KeyDataRecord keyData = (KeyDataRecord)data[keyStringOfArray];

		// Find the highest probability
		int highestValue = 0;
		CubeType bestAction = CubeType.none;

		// Get the list of actions in the store
		ICollection possibleActions = keyData.counts.Keys;
          
		// Go through each
		foreach(CubeType action in possibleActions){
		    // Check for the highest value
            if ((int)keyData.counts[action] > highestValue)
            {
                // Store the action
                highestValue = (int)keyData.counts[action];
                bestAction = action;
            }
        }
		
		double temp = Time.realtimeSinceStartup-startTime;
		overallTimeTakenForPredicting += temp;
		overallPredicts++;
		averageTimeTakenForPredicting = overallTimeTakenForPredicting / overallPredicts;
		
		return bestAction;
	}
	
}
