using UnityEngine;

public class KartReactions : MonoBehaviour {
    private AccelerationController acc_ctrl;
    //private TurningController trn_ctrl;
    private VehicleController vhc_ctrl;

    [SerializeField] private Transform [ ] tires;
    [SerializeField] private Transform [ ] frontSuspensions;

    private readonly float rotationSpeed = 3;
    private readonly float turnAngle = 20;

    private float CurrentSpeed {
        get {
            return acc_ctrl.CurrentSpeed;
        }
    }
    private float MaxSpeed {
        get {
            return acc_ctrl.MaxSpeed;
        }
    }
    private float TurningDirection {
        get {
            return Input.GetAxisRaw("Horizontal");
        }
    }
    private Transform PlayerKart {
        get {
            return vhc_ctrl.PlayerKart;
        }
    }
    
    private void OnEnable () {
        acc_ctrl = transform.parent.GetComponent<AccelerationController>();
        //trn_ctrl = transform.parent.GetComponent<TurningController>();
        vhc_ctrl = transform.parent.GetComponent<VehicleController>();
    }

    private void Update () {
        foreach( Transform _tire in tires ) {
            RotateTireToSpeed(_tire);
        }

        foreach( Transform f_suspension in frontSuspensions ) {
            TurnToDirection(f_suspension);
        }
    }

    private void TurnToDirection ( Transform _frontSuspension ) {
        Quaternion minRot = Quaternion.AngleAxis(-turnAngle, _frontSuspension.up) * PlayerKart.rotation;
        Quaternion maxRot = Quaternion.AngleAxis(turnAngle, _frontSuspension.up) * PlayerKart.rotation;
        Quaternion lerp = Quaternion.Lerp(minRot, maxRot, ( TurningDirection + 1 ) / 2);
        _frontSuspension.rotation = lerp;
    }

    private void RotateTireToSpeed ( Transform _tire ) {
        float speed = rotationSpeed * CurrentSpeed / MaxSpeed;
        float dot = Vector3.Dot(transform.TransformDirection(Vector3.right), _tire.TransformDirection(Vector3.up));
        float y = dot > 0 ? speed : -speed;
        _tire.Rotate(0,y,0, Space.Self);
    }
}