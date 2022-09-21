using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamMovement : MonoBehaviour
{
    public float cameraSpeed = 100f;
    public float camSensitivity = 1f;

    public void Update()
    {
        InputMovement();

        // Movement mode with right click
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            MouseCameraAngle();
        }
        else
        {
            // Hide cursor if not using camera
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void InputMovement()
    {
        // Get input from keyboard
        Vector3 input = GetMovementInput();

        // Only move when direction key is pressed
        if (input.sqrMagnitude > 0)
        {

            Vector3 movement = input * Time.unscaledDeltaTime * cameraSpeed;
            Vector3 newPosition = transform.position;
            transform.Translate(movement);
        }
    }

    public void MouseCameraAngle()
    {
        // Get X and Y axis
        Vector3 mouseInput = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);

        // Convert to direction using sensitivity
        Vector3 direction = new Vector3(-mouseInput.y * camSensitivity, mouseInput.x * camSensitivity, 0);

        // Add to exisitng camera angle
        Vector3 angle = new Vector3(WrapAngle(transform.eulerAngles.x + direction.x), transform.eulerAngles.y + direction.y, 0);
        transform.eulerAngles = angle;
    }

    // Wrap viewing angles around to stop flip snapping
    public float WrapAngle(float angle)
    {
        if (angle < 0)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return angle;
    }

    private Vector3 GetMovementInput()
    {
        Vector3 input = new Vector3();

        // Add input based on key presses
        if (Input.GetKey(KeyCode.W))
        {
            input += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            input += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            input += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            input += new Vector3(1, 0, 0);
        }
        return input;
    }
}
