using UnityEngine;

public class Spinner : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0.0f, 0.0f, 10.0f); // Degrees per second

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
