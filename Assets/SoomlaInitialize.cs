using UnityEngine;
using System.Collections;
using Soomla.Profile;

public class SoomlaInitialize : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SoomlaProfile.Initialize ();
	}

	void Awake() {
	}
	// Update is called once per frame
	void Update () {
	
	}
}
