using UnityEngine;

public class RepeatBackgroundX : MonoBehaviour
{
    private Vector3 startPos;
    private float repeatWidth;

    private void Start()
    {
        startPos = transform.position; // Establish the default starting position 
        // Bounds includes the object's scale, so this is always half of the
        // background's real world-space width rather than half of its height.
        repeatWidth = GetComponent<BoxCollider>().bounds.size.x / 2.0f;
    }

    private void Update()
    {
        // If background moves left by its repeat width, move it back to start position
        if (transform.position.x < startPos.x - repeatWidth)
        {
            transform.position = startPos;
        }
    }

 
}
