using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class InputHandler : MonoBehaviour
    {
        public bool haveJumpInputBeenPressed = false;
        public bool haveRunInputBeenPressed = false;
        public bool haveRunInputBeenReleased = false;

        public bool isRunButtonBeingPressed = false;
        private void Update()
        {
            if (Input.GetButtonDown(GameConstants.JumpButton) && !haveJumpInputBeenPressed)
                haveJumpInputBeenPressed = true;

            CheckIsRunning();
        }

        public void CheckIsRunning()
        {
            // Runing
            haveRunInputBeenPressed = Input.GetButtonDown(GameConstants.RunButton);
            haveRunInputBeenReleased = Input.GetButtonUp(GameConstants.RunButton);

            isRunButtonBeingPressed = Input.GetButton(GameConstants.RunButton);
        }

        public Vector3 GetMoveInput()
        {
            float horizontal = Input.GetAxisRaw(GameConstants.HorizontalAxisName);
            float vertical = Input.GetAxisRaw(GameConstants.VerticalAxisName);

            Vector3 move = new Vector3(horizontal, 0f, vertical);
            move = Vector3.ClampMagnitude(move, 1); // TODO im sure theire is a better way 

            return move;
        }


        // public float GetRightVerticalAxis()
        // {
        //     float vertical = Input.GetAxisRaw(GameConstants.VerticalAxisName);

        // }
        // public Vector3 GetRightAxis()
        // {

    }
}

