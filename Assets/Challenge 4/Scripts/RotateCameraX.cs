using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCameraX : MonoBehaviour
{
    private float speed = 200;
    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) horizontalInput = -1f;
            else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) horizontalInput = 1f;
        }
        transform.Rotate(Vector3.up, horizontalInput * speed * Time.deltaTime);

        transform.position = player.transform.position; // Move focal point with player

    }
}
