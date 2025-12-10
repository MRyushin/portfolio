using UnityEngine;

public class RotateImage : MonoBehaviour
{
    public float rotationSpeed = -1f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed);
    }
}
