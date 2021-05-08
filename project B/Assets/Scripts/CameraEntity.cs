using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEntity : MonoBehaviour
{
    const float MOVE_SPEED = 5f;
    const float ROTATE_SPEED = 30f;
    const float ANGLE_LIMIT = 45f;
    float verticalAngle = 0f;
    float horizontalAngle = 0f;


    Vector3 m_MousePosition;


    public void CameraRotate(string dir)
    {
        switch (dir)
        {
            case "counterclockwise":
                // Rotate counter clockwise.
                horizontalAngle = Mathf.Max(horizontalAngle - ROTATE_SPEED * Time.deltaTime, -ANGLE_LIMIT);
                break;
            case "clockwise":
                // Rotate clockwise.
                horizontalAngle = Mathf.Min(horizontalAngle + ROTATE_SPEED * Time.deltaTime, ANGLE_LIMIT);
                break;
            case "upward":
                // Rotate upward.
                verticalAngle = Mathf.Max(verticalAngle - ROTATE_SPEED * Time.deltaTime, -ANGLE_LIMIT + 25f);
                break;
            case "downward":
                // Rotate downward.
                verticalAngle = Mathf.Min(verticalAngle + ROTATE_SPEED * Time.deltaTime, ANGLE_LIMIT + 25f);
                break;
            case "freeReset":
                // Rotate free reset.
                m_MousePosition = Input.mousePosition;
                break;
            case "freeControl":
                // Rotate free.
                Vector3 mouseDeltaPosition = m_MousePosition - Input.mousePosition;
                horizontalAngle -= mouseDeltaPosition.x * 0.5f;
                verticalAngle = Mathf.Clamp(verticalAngle - mouseDeltaPosition.y, -89f, 89f);
                m_MousePosition = Input.mousePosition;
                break;
            default:
                break;
        }
        this.transform.localEulerAngles = new Vector3(verticalAngle, horizontalAngle, 0f);
    }
    public void CameraMove(string dir)
    {
        switch (dir)
        {
            case "forward":
                // Move forward.
                this.transform.Translate(Vector3.forward * MOVE_SPEED * Time.deltaTime);
                break;
            case "backward":
                // Move backward.
                this.transform.Translate(Vector3.back * MOVE_SPEED * Time.deltaTime);
                break;
            case "upward":
                // Move upward.
                this.transform.Translate(Vector3.up * MOVE_SPEED * Time.deltaTime);
                break;
            case "downward":
                // Move downward.
                this.transform.Translate(Vector3.down * MOVE_SPEED * Time.deltaTime);
                break;
            case "leftward":
                // Move leftward.
                this.transform.Translate(Vector3.left * MOVE_SPEED * Time.deltaTime);
                break;
            case "rightward":
                // Move rightward.
                this.transform.Translate(Vector3.right * MOVE_SPEED * Time.deltaTime);
                break;
            default:
                break;
        }
    }
    public void ResetCamera()
    {
        verticalAngle = 0f;
        horizontalAngle = 0f;
        this.transform.localEulerAngles = new Vector3(verticalAngle, horizontalAngle, 0f);
    }
}
