using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardX : MonoBehaviour
{
    public float speed;

    private void FixedUpdate()
    {
        // Ball prefabs already have dynamic Rigidbodies with gravity enabled,
        // so they fall at the intended challenge speed. Only dogs need a
        // scripted movement direction.
        if (CompareTag("Dog"))
        {
            transform.Translate(Vector3.left * speed * Time.fixedDeltaTime, Space.World);
        }
    }
}
