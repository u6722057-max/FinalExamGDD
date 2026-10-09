using UnityEngine;

public class SpinPropellerX : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1000f;

    private Transform propeller;

    private void Start()
    {
        propeller = FindChild(transform, "Propeller");
    }

    private void Update()
    {
        if (propeller != null)
        {
            propeller.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    private static Transform FindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }
}
