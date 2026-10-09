using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBoundsX : MonoBehaviour
{
    private float leftLimit = -30;
    private float bottomLimit = -5;

    // Update is called once per frame
    void Update()
    {
        // Destroy dogs if x position less than left limit
        if (CompareTag("Dog") && transform.position.x < leftLimit)
        {
            Destroy(gameObject);
        }
        else if (!CompareTag("Dog") && transform.position.y < bottomLimit)
        {
            Debug.Log("Game Over!");
            Destroy(gameObject);
        }

    }
}
