using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerX : MonoBehaviour
{
    public GameObject dogPrefab;
    public float speed = 20f;
    public float horizontalLimit = 22f;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        float horizontalInput = 0f;
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            horizontalInput = -1f;
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            horizontalInput = 1f;
        }

        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime, Space.World);
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -horizontalLimit, horizontalLimit);
        transform.position = position;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && dogPrefab != null)
        {
            Instantiate(dogPrefab, transform.position, dogPrefab.transform.rotation);
        }
    }
}
