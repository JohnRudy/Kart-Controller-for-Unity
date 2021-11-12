using UnityEngine;

public class AccelerationController : MonoBehaviour {

    private GroundCheck groundCheck;
    private SlideAndJumpController slideController;

    [Header("Acceleration states")]
    private bool isAccelerating;       // Bool to switch from input
    private bool isReversing;          // Bool to switch from input

    [Header("Acceleration Stats")]
    [SerializeField] private float maxSpeed;            // Maximum forward magnitude to achieve
    [SerializeField] private float maxReverseSpeed;     // Maximum reverse magnitude to achieve
    [SerializeField] private float maxAcceleration;     // How fast do we accelerate to maximum magnitude
    [SerializeField] private float maxBreaking;         // How fast do we break to 0 if crossing inputs or just reverse 
    
    private readonly float friction = 2f;        // How fast do we break to 0 if no input received
    private float currentSpeed;         // Magnitude multiplier for velocity
    float toSpeed;                      // Used in lerping in a single line
    float byForce;                      // Used in lerping in a single line

    // Private getters from other components 
    private bool IsGrounded {
        get {
            return groundCheck.IsGrounded;
        }
    }
    private bool IsJumping {
        get {
            return slideController.IsJumping;
        }
    }
    private bool IsBoosting {
        get {
            return slideController.IsBoosting;
        }
    }

    // Public accessors for other components
    public float CurrentSpeed {
        get {
            return currentSpeed;
        }
    }
    public float MaxSpeed {
        get {
            return maxSpeed;
        }
    }


    private void OnEnable () {
        groundCheck = GetComponent<GroundCheck>();
        slideController = GetComponent<SlideAndJumpController>();
    }

    // Receiving inputs
    private void Update () {
        isAccelerating = Input.GetAxisRaw("Accelerate") != 0 ? true : false || Input.GetButton("Accelerate");
        isReversing = Input.GetAxisRaw("Reverse") != 0 ? true : false || Input.GetButton("Reverse");
    }

    // Lerping and states
    private void FixedUpdate () {
        // Only allow acceleration to either direction if we are grounded
        if( IsGrounded ) {
            if( isAccelerating && !isReversing ) {
                if( currentSpeed < -0.1f ) {
                    toSpeed = 0;
                    byForce = maxBreaking;
                }
                else {
                    toSpeed = maxSpeed;
                    byForce = maxAcceleration;
                }
            }
            else if( isAccelerating && isReversing ) {
                toSpeed = 0;
                byForce = maxBreaking;
            }
            else if( !isAccelerating && isReversing ) {
                if( currentSpeed > 0.1f ) {
                    toSpeed = 0;
                    byForce = maxBreaking;
                }
                else {
                    toSpeed = maxReverseSpeed;
                    byForce = maxAcceleration;
                }
            }
        }

        // If no input is received from player
        if( !isAccelerating && !isReversing ) {
            toSpeed = 0;
            byForce = friction;
            if( currentSpeed > -0.01f && currentSpeed < 0.01f ) {
                currentSpeed = 0;
            }
        }

        // Keep current velocity if we are jumping 
        if( !IsGrounded && IsJumping ) {
            toSpeed = currentSpeed;
            byForce = maxAcceleration;
        }

        // Return to max speed after boost ends 
        if( IsBoosting ) {
            toSpeed = maxSpeed;
            byForce = maxAcceleration;
        }

        // Lerp by fixed delta time 
        byForce *= Time.fixedDeltaTime;
        currentSpeed = Mathf.Lerp(currentSpeed, toSpeed, byForce);
    }
}