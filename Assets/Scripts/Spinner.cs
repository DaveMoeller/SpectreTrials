using UnityEngine;

public class Spinner : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Degrees per second

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
