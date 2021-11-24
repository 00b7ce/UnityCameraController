using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.IO;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public string saveDirectory  = "Screenshots";
    public float rotateSpeed     = 1.0F;
    public float moveSpeed       = 1.0F;

    private bool cameraMode      = true;

    private Vector3    startPosition;
    private Quaternion startRotation;
    private Vector3    targetPosition;

    void Start()
    {
        startPosition = this.transform.position;
        startRotation = this.transform.rotation;
        if (target == null)
        {
            Debug.LogWarning("Target object is not set!!");
            cameraMode = false;
        }
        else
        {
            if (cameraMode) this.transform.LookAt(target.transform);
        }
    }

    void Update()
    {
        if (Gamepad.current == null) return;
        // Toggle camera mode
        if (Gamepad.current.selectButton.wasPressedThisFrame) {
            cameraMode = !cameraMode;
            if (cameraMode && (target != null)) this.transform.LookAt(target.transform);
        }
        // Screenshot
        if (Gamepad.current.startButton.wasPressedThisFrame) {
            if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);
            ScreenCapture.CaptureScreenshot(saveDirectory + "/" + DateTime.Now.ToString("yyyymmddHHmmss") + ".png");
        }
        // Apply position
        if (cameraMode) targetCamera();
        else            freeCamera();
        // Apply rotation
        this.transform.Rotate(new Vector3( Gamepad.current.rightStick.ReadValue().y * (rotateSpeed * 0.5F), 
                                           Gamepad.current.rightStick.ReadValue().x * (rotateSpeed * 0.5F),
                                          (Gamepad.current.leftShoulder.ReadValue() - Gamepad.current.rightShoulder.ReadValue()) * (rotateSpeed * 0.5F))
                             );

    }

    private void targetCamera()
    {
        if (target == null) return;
        // Reset rotation
        if (Gamepad.current.rightStickButton.isPressed) this.transform.LookAt(target.transform);
        // Set position
        this.transform.RotateAround(target.transform.position,
                                    new Vector3(0.0F,(Gamepad.current.leftStick.x.ReadValue() * -1), 0.0F),
                                    360.0F / (0.1F / (moveSpeed * 0.05F)) * Time.deltaTime
                                   );
        this.transform.Translate(Vector3.up      *  Gamepad.current.leftStick.y.ReadValue() * (moveSpeed * 0.05F));
        this.transform.Translate(Vector3.forward * Gamepad.current.rightTrigger.ReadValue() * (moveSpeed * 0.05F));
        this.transform.Translate(Vector3.back    * Gamepad.current.leftTrigger.ReadValue()  * (moveSpeed * 0.05F));
    }

    private void freeCamera()
    {
        // Reset position
        if (Gamepad.current.leftStickButton.isPressed)  this.transform.position = startPosition;
        // Reset rotation
        if (Gamepad.current.rightStickButton.isPressed) this.transform.rotation = startRotation;
        // Set position
        this.transform.Translate(Vector3.right   * Gamepad.current.leftStick.x.ReadValue()  * (moveSpeed * 0.05F));
        this.transform.Translate(Vector3.up      * Gamepad.current.leftStick.y.ReadValue()  * (moveSpeed * 0.05F));
        this.transform.Translate(Vector3.forward * Gamepad.current.rightTrigger.ReadValue() * (moveSpeed * 0.05F));
        this.transform.Translate(Vector3.back    * Gamepad.current.leftTrigger.ReadValue()  * (moveSpeed * 0.05F));
    }
}
