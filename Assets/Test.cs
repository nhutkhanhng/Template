using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    SomeFunction();

	}

    public void SomeFunction()
    {
        // Directly
        Debug.Log("Maximum health: " + PlayerData.Instance.maximumPlayerHealth);

        // Storing a reference, e.g. on Start()
        PlayerData playerData = PlayerData.Instance;
        Debug.Log("Starting lives: " + playerData.startingLives);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
