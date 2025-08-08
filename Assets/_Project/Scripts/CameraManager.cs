using System.Collections;
using Unity.Cinemachine;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Platformer
{
    public class CameraManager : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Anywhere] private InputReader input;
        [SerializeField, Anywhere][System.Obsolete] private CinemachineFreeLook freeLookVCam;

        [Header("Settings")]
        [SerializeField, Range(0.5f, 3f)] private float speedMultiplier = 1f;  // Controls camera speed

        private bool isRMBPressed;
        private bool cameraMovementLock;

        void OnEnable()
        {
            // Subscribe to input events
            input.Look += OnLook;
            input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        void OnDisable()
        {
            // Unsubscribe from input events
            input.Look -= OnLook;
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }

        // Handles camera movement based on mouse input
        void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if (cameraMovementLock) return;

            if (isDeviceMouse && !isRMBPressed) return;

            // Adjust movement speed based on whether it's mouse input or not
            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

            // Update the camera's X and Y axis movement
            freeLookVCam.m_XAxis.Value += cameraMovement.x * speedMultiplier * deviceMultiplier;
            freeLookVCam.m_YAxis.Value += cameraMovement.y * speedMultiplier * deviceMultiplier;
        }

        // Enables mouse control and locks the cursor to the center of the screen
        void OnEnableMouseControlCamera()
        {
            isRMBPressed = true;

            // Lock the cursor to the center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(DisableMouseForFrame());
        }

        // Disables mouse control and restores cursor visibility
        void OnDisableMouseControlCamera()
        {
            isRMBPressed = false;

            // Unlock the cursor and make it visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Reset the camera axes to prevent jumping when re-enabling mouse control
            freeLookVCam.m_XAxis.Value = 0f;
            freeLookVCam.m_YAxis.Value = 0f;
        }

        // Prevents camera movement for one frame when mouse control is disabled
        IEnumerator DisableMouseForFrame()
        {
            cameraMovementLock = true;
            yield return new WaitForEndOfFrame();
            cameraMovementLock = false;
        }
    }
}
