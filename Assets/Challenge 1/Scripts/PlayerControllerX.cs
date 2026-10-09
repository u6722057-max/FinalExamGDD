using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerX : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float verticalInput;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get the user's vertical input through Unity's active Input System.
        verticalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.upArrowKey.isPressed)
            {
                verticalInput = 1f;
            }
            else if (Keyboard.current.downArrowKey.isPressed)
            {
                verticalInput = -1f;
            }
        }

        // move the plane forward at a constant rate
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // tilt the plane up/down based on up/down arrow keys
        transform.Rotate(Vector3.right * verticalInput * rotationSpeed * Time.deltaTime);
    }
}
