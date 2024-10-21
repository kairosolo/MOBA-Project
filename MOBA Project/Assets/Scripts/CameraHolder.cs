using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraHolder : MonoBehaviour
{
    public float rotationSpeed = 10f; // Rotation speed in degrees per second
    public Vector3 rotationAxis = Vector3.up; // Axis of rotation, default is the Y axis

    private Quaternion targetRotation; // The target rotation

    // Start is called before the first frame update
    private void Start()
    {
        // Set the initial target rotation to the current rotation
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        // Rotate the CameraHolder smoothly
        RotateCameraHolder();
    }

    // Function to smoothly rotate the CameraHolder
    private void RotateCameraHolder()
    {
        // Calculate the target rotation based on the speed and axis
        targetRotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, rotationAxis);

        // Smoothly interpolate towards the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
    }

    public void EditLocalScale(float newScale)
    {
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void EditRotationSpeed(float rotationSpeed)
    {
        this.rotationSpeed = -rotationSpeed;
    }
}