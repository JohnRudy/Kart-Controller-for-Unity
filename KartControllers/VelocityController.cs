using UnityEngine;

public class VelocityController : MonoBehaviour {
    private VehicleController vehicleController;
    private AccelerationController accelerationController;
    private GroundCheck groundCheck;

    private Vector3 desiredVelocity;    // What is our desired velocity 
    private Vector3 slideVelocity;

    private float gravity = 3f;             // For when falling 
    private float terminalVelocity = 5f;    // For when falling 

    // Private getters from other components 
    private Transform PlayerKart {
        get {
            return vehicleController.PlayerKart;
        }
    }
    private Rigidbody SphereBody {
        get {
            return vehicleController.SphereBody;
        }
    }
    private float CurrentSpeed {
        get {
            return accelerationController.CurrentSpeed;
        }
    }
    private bool IsGrounded {
        get {
            return groundCheck.IsGrounded;
        }
    }

    private void OnEnable () {
        vehicleController = GetComponent<VehicleController>();
        accelerationController = GetComponent<AccelerationController>();
        groundCheck = GetComponent<GroundCheck>();
    }

    // Apply current velocity to karts forward vector each frame
    // Keep y velocity of the rigidbody 
    private void Update () {
        desiredVelocity = PlayerKart.TransformDirection(Vector3.forward) * CurrentSpeed;
        desiredVelocity.y = SphereBody.velocity.y;
    }

    // Add gravity velocity if we arent grounded
    private void FixedUpdate () {
        if( !IsGrounded ) {
            desiredVelocity += Vector3.Lerp(Vector3.zero, Vector3.down * terminalVelocity, gravity * Time.fixedDeltaTime);
        }

        SphereBody.velocity = desiredVelocity;
    }

    // Called from Slide and Jump controller to apply a upward force in world space to rigidbody 
    public void Jump ( float _jumpStrength, ForceMode _forceMode ) {
        SphereBody.AddForce(Vector3.up * _jumpStrength, _forceMode);
    }

    // Called from Slide and Jump controller to apply a sideways force in local space to slide the kart diagonally
    public void Slide ( Vector3 _direction, float _strength, ForceMode _forceMode ) {
        SphereBody.AddForce(PlayerKart.TransformDirection(_direction) * _strength, _forceMode);
    }

    // Called from Slide and Jump controller to apply a forward extra force in local space 
    public void Boost ( float _boostStrength, ForceMode _forceMode ) {
        SphereBody.AddForce(PlayerKart.forward * _boostStrength, _forceMode);
    }
}