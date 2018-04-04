using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour {


    public enum MovingPlatformType
    {
        BACK_FORTH,
        LOOP,
        ONCE
    }

    public PlatformCatcher platformCatcher;
    public float speed = 1.0f;
    public MovingPlatformType platformType;

    public bool startMovingOnlyWhenVisible;
    public bool isMovingAtStart = true;

    [HideInInspector]
    public Vector3[] localNodes = new Vector3[1];

    public float[] waitTimes = new float[1];

    public Vector3[] worldNode { get { return m_WorldNode; } }

    protected Vector3[] m_WorldNode;

    protected int m_Current = 0;
    protected int m_Next = 0;
    protected int m_Dir = 1;

    protected float m_WaitTime = -1.0f;

    protected Rigidbody2D m_Rigidbody2D;
    protected Vector2 m_Velocity;

    protected bool m_Started = false;
    protected bool m_VeryFirstStart = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
