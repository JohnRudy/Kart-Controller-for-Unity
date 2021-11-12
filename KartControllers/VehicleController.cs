using UnityEngine;

public class VehicleController : MonoBehaviour {
    [SerializeField] private Transform playerKart;      // Model of the kart. Is used to get forward vector for velocity
    [SerializeField] private Rigidbody sphereBody;      // Rigidbody that is moved by the forward vector of the kart
    [SerializeField] private Vector3 kartCenterOfMass;  // A simple offset to be applied to the kart. 

    // Public accessors for other components
    public Transform PlayerKart {
        get {
            return playerKart;
        }
    }
    public Rigidbody SphereBody {
        get {
            return sphereBody;
        }
    }

    private void OnValidate () {
        if (playerKart != null && sphereBody != null ) {
            KartPosition();
        }
    }

    private void FixedUpdate () {
        KartPosition();
    }

    private void KartPosition () {
        playerKart.position = sphereBody.position + playerKart.TransformDirection(kartCenterOfMass);
    }
}