using UnityEngine;

public class TurningController : MonoBehaviour {
    private VehicleController vehicleController;
    private GroundCheck groundCheck;
    private AccelerationController accelerationController;
    private SlideAndJumpController slideController;

    [SerializeField] private float maxHandling;         // How fast are we allowed to make that turn 
    [SerializeField] private AnimationCurve curve;      // Used for lerping from standstill to full turn 

    private readonly float maxTurnAngle = 1f;    // How much we are allowed to turn "Radius" 
    private float turningDirection;     // Input axis direction 
    private float turningAngle;         // How large is the angle that we desire to turn with 
    private float dot;                  // To make sure we can only turn if we are not falling unless jump was activated 
    private float speed;                // Our velocity magnitude
    private RaycastHit hit;             // For turning the kart along the track normals 
    private Ray ray;                    // For turning the kart along the track normals 

    // Private getters from other components 
    private SlideDirection SlideDir {
        get {
            return slideController.SlideDir;
        }
    }
    private Rigidbody SphereBody {
        get {
            return vehicleController.SphereBody;
        }
    }
    private Transform PlayerKart {
        get {
            return vehicleController.PlayerKart;
        }
    }
    private float CurrentSpeed {
        get {
            return accelerationController.CurrentSpeed;
        }
    }
    private float MaxSpeed {
        get {
            return accelerationController.MaxSpeed;
        }
    }
    private float MaxSlideAngle {
        get {
            return slideController.MaxSlideAngle;
        }
    }
    private float MinSlideAngle {
        get {
            return slideController.MinSlideAngle;
        }
    }
    private bool IsGrounded {
        get {
            return groundCheck.IsGrounded;
        }
    }
    private bool IsSliding {
        get {
            return slideController.IsSliding;
        }
    }
    private bool IsJumping {
        get {
            return slideController.IsJumping;
        }
    }
    private LayerMask LayerMask {
        get {
            return groundCheck.LayerMask;
        }
    }
    private Vector3 RayOffset {
        get {
            return groundCheck.RayOffset;
        }
    }

    // Public accessors for other components 
    public float TurnDirection {
        get {
            return turningDirection;
        }
    }


    public void OnEnable () {
        vehicleController = GetComponent<VehicleController>();
        groundCheck = GetComponent<GroundCheck>();
        accelerationController = GetComponent<AccelerationController>();
        slideController = GetComponent<SlideAndJumpController>();
    }

    // Assing user horizontal input each frame to turning direction as base
    private void Update () {
        turningDirection = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate () {
        // Turning angle lerping based on speed and direction
        dot = Vector3.Dot(PlayerKart.up, SphereBody.velocity);
        speed = Mathf.Abs(CurrentSpeed) / MaxSpeed;

        if( dot >= -2f && IsJumping || speed >= 0.1f ) {
            turningAngle = Mathf.Lerp(turningAngle, maxTurnAngle * turningDirection * curve.Evaluate(speed), maxHandling * Time.fixedDeltaTime);
        }
        else {
            turningAngle = Mathf.Lerp(turningAngle, 0, 50 * Time.fixedDeltaTime);
            if( turningAngle < 0.1f && turningAngle > -0.1f ) {
                turningAngle = 0;
            }
        }

        // Turning the kart based on hit normals and turning angle
        if( !IsGrounded && !IsJumping ) {
            TurnKart(Vector3.up, 0);
        }
        else {
            hit = new RaycastHit();
            ray = new Ray();

            ray.origin = PlayerKart.position + RayOffset;
            ray.direction = -PlayerKart.up;

            Physics.Raycast(ray, out hit, LayerMask);

            // Remapping user input to align with sliding 
            if( IsSliding ) {
                turningDirection = Mathf.Abs(( turningDirection + ( SlideDir == SlideDirection.LEFT ? 1 : -1 ) ) / 2 * ( SlideDir == SlideDirection.LEFT ? -1 : 1 ));
                float slideLerp = Mathf.Lerp(MinSlideAngle, MaxSlideAngle, turningDirection) * ( SlideDir == SlideDirection.LEFT ? 1 : -1 );
                TurnKart(hit.normal, slideLerp);
            }
            else {
                TurnKart(hit.normal, turningAngle);
            }
        }
    }

    private void TurnKart ( Vector3 _normal, float _amount ) {
        // Along surface normal
        Quaternion up = Quaternion.FromToRotation(PlayerKart.up, _normal) * PlayerKart.rotation;
        PlayerKart.rotation = Quaternion.Lerp(PlayerKart.rotation, up, 3 * Time.fixedDeltaTime);

        // Input turning
        PlayerKart.rotation *= Quaternion.AngleAxis(_amount * maxHandling, PlayerKart.up);
    }
}