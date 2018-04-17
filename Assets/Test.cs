using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    private Rigidbody m_Rig;

	// Use this for initialization
	void Start ()
	{
	    SomeFunction();
        m_Rig = GetComponent<Rigidbody>();
	}

    public void SomeFunction()
    {
        // Directly
        //Debug.Log("Maximum health: " + PlayerData.Instance.maximumPlayerHealth);

        //// Storing a reference, e.g. on Start()
        //PlayerData playerData = PlayerData.Instance;
        //Debug.Log("Starting lives: " + playerData.startingLives);
    }

    // Update is called once per frame
    void Update () {
        float h = Input.GetAxis("Vertical");

        m_Rig.MovePosition(this.m_Rig.transform.position +  Vector3.forward * h * Time.deltaTime);
	}
}
