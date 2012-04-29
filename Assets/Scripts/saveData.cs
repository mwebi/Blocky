using UnityEngine;
using System.Collections;

public class saveData : MonoBehaviour {
	public float sumOfGamesWonP1 = 0;
	public float sumOfroundsWonP1 = 0;
	public float sumOfOverallScoreP1 = 0;
	public float sumOfBonusScoreP1 = 0;
	
	public float sumOfGamesWonP2 = 0;
	public float sumOfroundsWonP2 = 0;
	public float sumOfOverallScoreP2 = 0;
	public float sumOfBonusScoreP2 = 0;

	
	public bool firstRound = true;
	public bool isCopy = false;
	
	public double P1RegisterTime;
	public double P1PredictionTime;
	public double P2RegisterTime;
	public double P2PredictionTime;
	
	void Awake () {
		if(GameObject.Find("dataObjectOriginal"))
			Destroy(this.gameObject);
	}
	
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
