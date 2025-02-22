﻿using UnityEngine;
using System.Collections;

public class BubbleBehaviour : MonoBehaviour {
	public float sinAmp = 0.5f;
	public float sinOsc = 15f;
	public Vector3 initpos;
	public bool hasMeemo = false;
	public bool isPopped;
	public Animator anim;

	public float lastingTime = 10f;
	public float timePassed = 0f;

	private Hero_Interaction thisMeemo;

	private CameraBehavior globalBehavior;

	// Use this for initialization
	void Start () {
		globalBehavior = GameObject.Find("Main Camera").GetComponent<CameraBehavior>();

		initpos = transform.position;

		anim = GetComponent<Animator> ();
		isPopped = false;
	}




	// Update is called once per frame
	void Update () {

		if(!hasMeemo)
			FollowSineCurve ();

		Vector3 size = GetComponent<Renderer> ().bounds.size;

		// When top of bubble touches world bound, Pop it
		if (((transform.position.y + size.y / 2f) > globalBehavior.globalyMax && !isPopped)) {
			PopBubble ();
		}

		if (hasMeemo) {
			timePassed += Time.deltaTime;
			if (timePassed > lastingTime && !isPopped) {
				PopBubble ();
			}
		}


		// When bottom of bubble touches world bound, destroy it
		if ((transform.position.y - size.y / 2f) > globalBehavior.globalyMax) {
			Debug.Log ("bubble is destroyed");
			Destroy (this.gameObject);
		}
	}




	// Update position of bubble following sine curve
	public void FollowSineCurve(){
		float newY = transform.position.y + 0.03f;
		float newX = initpos.x + GetXValue (newY); 
		transform.position = new Vector3 (newX, newY, 0f);
	}




	// Calculate the x value for bubble movement
	private float GetXValue(float y){
		float sinFreqScale = sinOsc * 2f * (Mathf.PI) / globalBehavior.globalxMax;
		return sinAmp * (Mathf.Sin(y * sinFreqScale));
	}




	// When bubble touches other game objects
	void OnTriggerEnter2D(Collider2D other)
	{
		Hero_Interaction meemo = GameObject.FindGameObjectWithTag ("Player").GetComponent<Hero_Interaction> ();
		if (other.gameObject.name == "Meemo" && !meemo.isInBubble && !isPopped) {
			Debug.Log ("Touches Meemo");
			thisMeemo = other.GetComponent<Hero_Interaction> ();

			hasMeemo = true;
			thisMeemo.GetComponent<Rigidbody2D> ().isKinematic = true;
			thisMeemo.bubble = this;
			thisMeemo.isInBubble = true;

		}

		// Bubble pops when touches jellyfish or squid
		if (other.gameObject.tag == "jelly" || other.gameObject.tag == "squid") {
			PopBubble ();
		}
	}


	void PopBubble() {
		Debug.Log ("Bubble is popping");
		if (hasMeemo) {
			thisMeemo.isInBubble = false;
			thisMeemo.GetComponent<Rigidbody2D> ().isKinematic = false;
			hasMeemo = false;
		}
		anim.SetTrigger ("trigger");
		isPopped = true;
	}
}
