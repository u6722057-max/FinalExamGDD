using UnityEngine;

public sealed class DrivingCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target == null) return;
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.2f);
    }

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + target.rotation * offset;
        transform.LookAt(target.position + Vector3.up * 1.2f);
    }
}
