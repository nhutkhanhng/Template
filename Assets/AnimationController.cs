using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    protected Movement movement;

    private Animator m_Animator;
	// Use this for initialization
	void Start () {
        movement = GetComponent<Movement>();

        m_Animator = GetComponent<Animator>();
	}
	

    private void UpdateAnimation(Vector3 Move)
    {
        //m_Animator.SetFloat("Forward", )
    }
	// Update is called once per frame
	void Update () {
		
	}
}
