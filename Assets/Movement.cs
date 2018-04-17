using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObject/MovementSettigns")]
[System.Serializable]
public class MovementSettings : ScriptableObject
{
    [SerializeField]
    public float ForwardSpeed = 8f; // Speed when walikng forwad
    public float BackwardSpeed = 4.0f;
    public float StrafeSpeed = 4f;
    public float RunMultiplier = 2f; // 
    public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
    public float JumpForce = 30f;


}


[CreateAssetMenu(menuName = "ScriptableObject/AdvanceSettings")]
[Serializable]
public class AdvanceSettings : ScriptableObject
{
    public float groundCheckDistance = 0.01f;
    public float stickToGroundHelperDistance = 0.5f;
    public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
    public bool airControl;
    [Tooltip("Set it to 0.1 or more if you get stuck in wall")]
    public float shellOffset;//reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
}

[CreateAssetMenu(menuName = "ScriptableObject/SettingsInput")]
[Serializable]
public class SettingsInput : ScriptableObject
{
#if !MOBILE_INPUT
    public KeyCode KeyJump = KeyCode.Space;
    public KeyCode KeyRun = KeyCode.LeftShift;
    public KeyCode KeyForward = KeyCode.W;
    public KeyCode KeyBackward = KeyCode.S;
    public KeyCode KeyLeftStrafe = KeyCode.A;
    public KeyCode KeyRightStrafe = KeyCode.D;
#endif
}


public class Movement : MonoBehaviour {
    [SerializeField]
    public MovementSettings Settings;
    [SerializeField]
    public AdvanceSettings AdvanceSettigns;

    [HideInInspector]
    public float CurrentSpeed;

    [HideInInspector]
    
    private bool m_Running; // Checkout running
    public bool Running { get { return Running; } }



#if !MOBILE_INPUT
    [SerializeField]
    public SettingsInput keyboard;
#endif

    
    Rigidbody m_RigidBody;

    public Vector3 Velocity
    {
        get { return m_RigidBody.velocity; }
    }

    private CapsuleCollider m_Capsule;

    private Vector3 m_GroundContactNormal;

    private bool m_IsGrounded, m_Jump;
    private bool m_PreviouslyGrounded, m_Jumping ;

    public Camera m_Camera;

    private Animator m_Animator;

    // Use this for initialization
    void Start () {
        //cam = Camera.main;
        m_RigidBody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();


        m_Animator = GetComponent<Animator>();
	}


    
    public void UpdateDesiredTargetSpeed(Vector2 input)
    {
        if (input == Vector2.zero)
            return;

        if (input.x > 0 || input.x < 0)
        {
            CurrentSpeed = Settings.StrafeSpeed;
        }

        CurrentSpeed = (input.y < 0 ) ? Settings.BackwardSpeed : Settings.ForwardSpeed;

#if !MOBILE_INPUT
        if (Input.GetKey(keyboard.KeyRun))
        {
            this.CurrentSpeed *= Settings.RunMultiplier;
            this.m_Running = true;
        }
        else
            this.m_Running = false;
#endif
    }


    private Vector2 GetInput()
    {
        Vector2 input = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")
        };

        this.UpdateDesiredTargetSpeed(input);
        return input;
    }

    private float SlopeMultiplier()
    {
        float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
        return this.Settings.SlopeCurveModifier.Evaluate(angle);
    }

    private void StickToGroundHelper()
    {
        RaycastHit hitInfo;

        if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - AdvanceSettigns.shellOffset), Vector3.down, out hitInfo,
                               ((m_Capsule.height / 2f) - m_Capsule.radius) +
                               AdvanceSettigns.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
            {
                m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
            }
        }
    }

    private void GroundCheck()
    {
        m_PreviouslyGrounded = m_IsGrounded;

        RaycastHit hitInfo;

#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * AdvanceSettigns.groundCheckDistance));
#endif
        // Notes: Postion = Transform.postion + m_Capsule.center
        if (Physics.SphereCast(transform.position + m_Capsule.center, m_Capsule.radius * (1.0f - AdvanceSettigns.shellOffset), Vector3.down, out hitInfo,
                       ((m_Capsule.height) / 2f - m_Capsule.radius) + AdvanceSettigns.groundCheckDistance 
                       , Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            m_IsGrounded = true;
            //m_Animator.applyRootMotion = true;
            m_GroundContactNormal = hitInfo.normal;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundContactNormal = Vector3.up;
            //m_Animator.applyRootMotion = false;
        }

        if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
        {
            m_Jumping = false;
        }
    }

    bool m_Crouching;
    [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    const float k_Half = 0.5f;

    void UpdateAnimator(Vector3 move)
    {
        // update the animator parameters
        m_Animator.SetFloat("Forward", this.Velocity.z, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", this.Velocity.x, 0.1f, Time.deltaTime);
        m_Animator.SetBool("Crouch", m_Crouching);
        m_Animator.SetBool("OnGround", m_IsGrounded);
        if (!m_IsGrounded)
        {
            m_Animator.SetFloat("Jump", this.Velocity.y);
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * CurrentSpeed;

        ////Debug.Log(m_IsGrounded);

        //if (m_IsGrounded)
        //{
        //    m_Animator.SetFloat("JumpLeg", jumpLeg);
        //}

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_IsGrounded && move.magnitude > 0)
        {
            m_Animator.speed = Settings.RunMultiplier;
        }
        else
        {
            // don't use that while airborne
            m_Animator.speed = 1;
        }
    }

    void HandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // jump!
            m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, Settings.JumpForce, m_RigidBody.velocity.z);
            m_IsGrounded = false;
            m_Animator.applyRootMotion = false;
            this.AdvanceSettigns.groundCheckDistance = 0.1f;
        }
    }

    public void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (m_IsGrounded && Time.deltaTime > 0)
        {
            Vector3 v = (m_Animator.deltaPosition * Settings.RunMultiplier) / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = m_RigidBody.velocity.y;
            m_RigidBody.velocity = v;
        }
    }

    private void FixedUpdate()
    {
        GroundCheck();

        Vector2 input = GetInput();


        if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (this.AdvanceSettigns.airControl || m_IsGrounded))
        {
            // always move along the camera forward as it is the direction that it being aimed at

            Debug.LogError(input);
            Vector3 desiredMove = m_Camera.transform.forward * input.y + m_Camera.transform.right * input.x;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

            desiredMove.x = desiredMove.x * this.CurrentSpeed;
            desiredMove.z = desiredMove.z * this.CurrentSpeed;
            desiredMove.y = desiredMove.y * this.CurrentSpeed;

            Debug.LogWarning(desiredMove);

            if (m_RigidBody.velocity.sqrMagnitude <
                (this.CurrentSpeed * this.CurrentSpeed))
            {
                m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Acceleration);
            }
        }

        if (m_IsGrounded)
        {
            m_RigidBody.drag = 5f;

            if (m_Jump)
            {
                m_RigidBody.drag = 0f;
                m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);

                m_RigidBody.AddForce(new Vector3(0f, Settings.JumpForce, 0f), ForceMode.Impulse);
                m_Jumping = true;
                m_IsGrounded = false;
                m_Animator.applyRootMotion = false;
            }

            if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
            {
                m_RigidBody.Sleep();
            }
        }
        else
        {
            m_RigidBody.drag = 0f;
            if (m_PreviouslyGrounded && !m_Jumping)
            {
                StickToGroundHelper();
            }
        }



        //ScaleCapsuleForCrouching(this.m_Crouching);
        //PreventStandingInLowHeadroom();
        //if (m_IsGrounded)
        //    EditorApplication.isPaused = true;
        this.UpdateAnimator(Vector3.zero);
        m_Jump = false;
    }


    void ScaleCapsuleForCrouching(bool crouch)
    {
        if (m_IsGrounded && crouch)
        {
            if (m_Crouching) return;
            m_Capsule.height = m_Capsule.height / 2f;
            m_Capsule.center = m_Capsule.center / 2f;
            m_Crouching = true;
        }
        else
        {
            Ray crouchRay = new Ray(m_RigidBody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = m_Capsule.height - m_Capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_Crouching = true;
                return;
            }
            //m_Capsule.height = m_CapsuleHeight;
            //m_Capsule.center = m_CapsuleCenter;
            m_Crouching = false;
        }
    }

    void PreventStandingInLowHeadroom()
    {
        // prevent standing up in crouch-only zones
        if (!m_Crouching)
        {
            Ray crouchRay = new Ray(m_RigidBody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = m_Capsule.height - m_Capsule.radius * k_Half;

            if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_Crouching = true;
            }
        }
    }

}