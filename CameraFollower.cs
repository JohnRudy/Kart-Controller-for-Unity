using UnityEngine;

public class CameraFollower : MonoBehaviour {
    [SerializeField] private VehicleController vehicleController;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform playerKart;

    private Vector3 followDirection;

    private void OnValidate () {
        if( playerKart ) {
            FollowPlayer();
        }
    }

    private void Start () {
        vehicleController = transform.parent.GetComponent<VehicleController>();
        playerKart = vehicleController.PlayerKart;
    }

    private void FixedUpdate () {
        if( playerKart ) {
            FollowPlayer();
        }
    }

    private void FollowPlayer () {
        followDirection = Vector3.Lerp(transform.position, playerKart.position + playerKart.TransformDirection(offset), 7 * Time.deltaTime);
        transform.position = followDirection;
        transform.LookAt(playerKart.position);
    }
}
