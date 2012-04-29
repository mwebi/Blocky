using UnityEngine;
using System.Collections;

public class rules : MonoBehaviour {
	
	Vector3 originalScale;
	Vector3 originalPosition;
	Vector3 resizePosition;
	// Use this for initialization
	void Start () {
		originalScale=transform.localScale;
		originalPosition = transform.position;
		resizePosition = new Vector3(-120,-10,transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void OnMouseOver (){
		transform.position = resizePosition;
		transform.localScale = originalScale*7f;
		
	}
	
	void OnMouseExit (){
		transform.localScale = originalScale;
		transform.position = originalPosition;
	}
}
