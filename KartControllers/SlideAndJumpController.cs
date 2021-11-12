using UnityEngine;
using UnityEngine.UI;

// Easier to read slide directions 
public enum SlideDirection { LEFT, RIGHT, NONE }

public class SlideAndJumpController : MonoBehaviour {

    private AccelerationController accelerationController;
    private GroundCheck groundCheck;
    private TurningController turningController;
    private VelocityController velocityController;

    // UI slider to show how much we have gathered sliding 
    // Place this into a UI Manager
    [SerializeField] private Slider slideSlider;
    [SerializeField] private ParticleSystem boostParticle;
    [SerializeField] private ParticleSystem slideParticle;

    private ForceMode forceMode = ForceMode.VelocityChange;     // Used in boosting, sliding and jumping. Velocity change by default
    private SlideDirection slideDirection;     // Which direction we want to slide into
    private float minSlideSpeed = 7;           // How fast does the kart need to be moving to apply sliding after jump
    private float slideForce = 7f;             // How much force to apply to karts velocity vector 
    private float slideTimer = 0;              // How long have we been sliding 
    private float maxSlideTime = 5;            // How much do we need to slide to have boost available

    private bool canSlide;                     // Is the player allowed to slide
    private bool isSliding;                    // Are we sliding 

    private float maxSlideAngle = 1;           // Max amount to apply turning when sliding by user
    private float minSlideAngle = 0.1f;        // Min amount to apply turning when sliding by user

    private float boostStrength = 10f;         // How much force to apply to kart when boosting 
    private float maxBoostTime = 3f;           // How long should the boost be active
    private float boostTimer = 0;              // Countdown for boosting 
    private bool isBoosting = false;           // Are we boosting 

    private bool isJumping;                    // Are we jumping 
    private float jumpInterval = 60f;          // How long to wait till new jump can be applied 
    private float jumpStrength = 5;            // How much force to apply to karts up vector
    private float jumpTimer;                   // How long have we been jumping 

    // Private getters from other components 
    private float CurrentSpeed {
        get {
            return accelerationController.CurrentSpeed;
        }
    }
    private float TurnDirection {
        get {
            return turningController.TurnDirection;
        }
    }
    private bool IsGrounded {
        get {
            return groundCheck.IsGrounded;
        }
    }

    // Public accessors for other components 
    public float MaxSlideAngle {
        get {
            return maxSlideAngle;
        }
    }
    public float MinSlideAngle {
        get {
            return minSlideAngle;
        }
    }
    public bool IsSliding {
        get {
            return isSliding;
        }
    }
    public bool IsJumping {
        get {
            return isJumping;
        }
    }
    public bool IsBoosting {
        get {
            return isBoosting;
        }
    }
    public SlideDirection SlideDir {
        get {
            return slideDirection;
        }
    }

    private void OnEnable () {
        accelerationController = GetComponent<AccelerationController>();
        groundCheck = GetComponent<GroundCheck>();
        turningController = GetComponent<TurningController>();
        velocityController = GetComponent<VelocityController>();
        slideDirection = SlideDirection.NONE;
        slideParticle.gameObject.SetActive(false);
        boostParticle.gameObject.SetActive(false);
    }

    // User inputs and slide direction handling  
    private void Update () {
        // Jumping
        if( Input.GetButtonDown("Slide") && IsGrounded && !isJumping ) {
            isJumping = true;
            jumpTimer += 1;
            velocityController.Jump(jumpStrength, forceMode);
        }

        if( isJumping ) {
            jumpTimer++;
        }

        if( jumpTimer >= jumpInterval && IsGrounded ) {
            jumpTimer = 0;
        }

        if( IsGrounded && jumpTimer == 0 ) {
            isJumping = false;
        }

        // Sliding 
        if( Input.GetButton("Slide") && IsJumping && CurrentSpeed >= minSlideSpeed ) {
            canSlide = true;
        }

        if( Input.GetButtonUp("Slide") ) {
            canSlide = false;
            isSliding = false;
            slideDirection = SlideDirection.NONE;

            if( isBoosting ) {
                boostTimer = 0;
            }

            if( slideTimer >= maxSlideTime ) {
                isBoosting = true;
                slideTimer = 0;
            }
        }


        if( canSlide && IsGrounded && slideDirection != SlideDirection.NONE ) {
            isSliding = true;
        }
        else if( canSlide && IsGrounded && !isSliding ) {
            canSlide = false;
        }

        if( canSlide && !IsGrounded && !isSliding ) {
            if( TurnDirection > 0 ) {
                slideDirection = SlideDirection.LEFT;
            }
            else if( TurnDirection < 0 ) {
                slideDirection = SlideDirection.RIGHT;
            }
            else {
                slideDirection = SlideDirection.NONE;
            }
        }

        slideSlider.value = slideTimer / maxSlideTime;

        boostParticle.gameObject.SetActive(isBoosting);
        slideParticle.gameObject.SetActive(isSliding);
    }

    // Applying force and counting slide and boost times 
    private void FixedUpdate () {

        // While sliding
        if( isSliding ) {
            if( isBoosting ) {
                slideTimer += Time.fixedDeltaTime * 2.5f;
            }
            else {
                slideTimer += Time.fixedDeltaTime;
            }

            if( slideTimer >= maxSlideTime ) {
                slideTimer = maxSlideTime;
            }

            velocityController.Slide(slideDirection == SlideDirection.LEFT ? Vector3.left : Vector3.right, slideForce, forceMode);
        }

        // Applying Boost for the allowed time
        if( isBoosting ) {
            boostTimer += Time.fixedDeltaTime;

            velocityController.Boost(boostStrength, forceMode);

            if( boostTimer >= maxBoostTime ) {
                boostTimer = 0;
                isBoosting = false;
            }
        }

        if( CurrentSpeed < minSlideSpeed && slideTimer != 0) {
            slideTimer -= Time.fixedDeltaTime / 2;

            if (slideTimer <= 0 ) {
                slideTimer = 0;
            }
        }
    }
}