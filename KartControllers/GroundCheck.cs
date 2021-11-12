using UnityEngine;

public class GroundCheck : MonoBehaviour {
    private VehicleController vehicleController;

    [SerializeField] private LayerMask layerMask;       // Track mask
    [SerializeField] private bool isGrounded;           // Bool to switch when is airborn and coyote is done 

    [Header("Raycast")]
    [SerializeField] private Vector3 rayOffset;         // For ground raycasting

    private float groundDistance;   // How far down are we looking from the kart position
    private RaycastHit hit;         // For ground raycasting 
    private Ray ray;                // For ground raycasting

    // Getters from other classes 
    private Transform PlayerKart {
        get {
            return vehicleController.PlayerKart;
        }
    }

    // Public accessors for other components
    public LayerMask LayerMask {
        get {
            return layerMask;
        }
    }
    public bool IsGrounded {
        get {
            return isGrounded;
        }
    }
    public Vector3 RayOffset {
        get {
            return rayOffset;
        }
    }

    private void OnEnable () {
        vehicleController = GetComponent<VehicleController>();
    }

    // Setting ground offset at start. Each vehicle can have their own ground distance
    private void Start () {
        Ray startRay = new Ray();
        RaycastHit startHit = new RaycastHit();

        startRay.origin = PlayerKart.position + rayOffset;
        startRay.direction = -PlayerKart.up;

        if( Physics.Raycast(startRay, out startHit, layerMask) ) {
            groundDistance = Vector3.Distance(startRay.origin, startHit.point);
        }
    }

    // Simple raycast for ground checking 
    private void FixedUpdate () {
        ray.origin = PlayerKart.position + rayOffset;
        ray.direction = PlayerKart.TransformDirection(Vector3.down);

        Physics.Raycast(ray, out hit, layerMask);

        if( Vector3.Distance(hit.point, PlayerKart.position) > groundDistance ) {
            isGrounded = false;
        }
        else {
            isGrounded = true;
        }
    }
}