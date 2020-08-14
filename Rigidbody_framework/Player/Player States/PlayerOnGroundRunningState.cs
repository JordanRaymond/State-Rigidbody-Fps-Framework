using UnityEngine;
using Core;

namespace FPS
{
    public class PlayerOnGroundRunningState : IPlayerState
    {

        public PlayerFallingState fallingState;

        public void OnEnteredState(PlayerParams playerParams)
        {

        }

        public void UpdateState(PlayerParams playerParams)
        {
            if (playerParams.isGrounded)
            {
                if (GameManager.InputHandler.haveJumpInputBeenPressed)
                {
                    playerParams.player.Jump();
                }
                else if (!GameManager.InputHandler.isRunButtonBeingPressed)
                {
                    playerParams.player.SetState(new PlayerOnGroundState());

                }
            }
            else
            {
                playerParams.player.SetState(new PlayerFallingState(true));
            }
        }

        public void FixedUpdateState(PlayerParams playerParams)
        {
            if (playerParams.isGrounded)
            {
                Move(playerParams);
            }
        }

        public void OnExitState(PlayerParams playerParams)
        {

        }

        public string GetName()
        {
            return "PlayerOnGroundRunningState";
        }

        private void Move(PlayerParams playerParams)
        {
            if (playerParams.moveDirection != Vector3.zero)
            {
                playerParams.player.StickToGround();
            }
            playerParams.player.Move(playerParams.runSpeed);
        }
    }
}