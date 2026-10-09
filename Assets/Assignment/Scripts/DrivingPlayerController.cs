using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class DrivingPlayerController : MonoBehaviour
{
    [SerializeField] private float driveSpeed = 16f;
    [SerializeField] private float reverseSpeed = 8f;
    [SerializeField] private float turnSpeed = 75f;
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.mass = 1200f;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (Keyboard.current == null) return;

        // Cruise forward automatically when the player is not pressing a drive key.
        float throttle = 1f;
        if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed) throttle = -1f;

        float steering = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) steering -= 1f;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) steering += 1f;

        float speed = throttle >= 0f ? driveSpeed : reverseSpeed;
        Vector3 movement = transform.forward * (throttle * speed * Time.fixedDeltaTime);
        body.MovePosition(body.position + movement);
        float direction = throttle >= 0f ? 1f : -1f;
        Quaternion turn = Quaternion.Euler(0f, steering * turnSpeed * direction * Time.fixedDeltaTime, 0f);
        body.MoveRotation(body.rotation * turn);
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(16, 16, 360, 78), "AUTO DRIVE: car cruises forward\nA/D or ←/→: Steer  •  S or ↓: Reverse\nESC: Pause");
    }
}
