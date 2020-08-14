using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace FPS
{
    public class MouseLook : MonoBehaviour
    {
        public Camera camera;
        public Transform cameraTransform;
        public float mouseSentivity = 10f;

        [Header("Interactable Params")]
        public float interactableRange = 1;
        public LayerMask interactableLayer = 11;

        private float xRotation = 0f;
        private string verticalAxisName;
        private string horizontalAxisName;

        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            camera = GetComponent<Camera>();

            verticalAxisName = GameConstants.MouseVerticalAxisName;
            horizontalAxisName = GameConstants.MouseHorizontalAxisName;
        }

        // TODO Add into input handle get right axis vertical/horizontal and take care of joystick inputs

        // Called in Update
        public void RotateCamera()
        {
            float mouseY = Input.GetAxis(verticalAxisName);

            xRotation -= mouseY * mouseSentivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Called in Fixed Update
        public void RotatePlayer(PlayerParams playerParams)
        {
            float horizontal = Input.GetAxis(horizontalAxisName);

            Vector3 direction = Vector3.up * horizontal * (mouseSentivity * Time.fixedDeltaTime);
            playerParams.rigidBody.rotation *= Quaternion.Euler(direction);
        }

        public void CheckForInteractable()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, interactableRange, interactableLayer))
            {
                Transform objectHit = hit.transform;
                // Get the iiteractable function call it
            }
        }
    }
}