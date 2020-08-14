using UnityEngine;
using Core;

namespace FPS
{
    public class PlayerOnGroundState : IPlayerState
    {

        // States used: FallingState, PlayerOnGroundRunningState
        Rigidbody rigid;

        public void OnEnteredState(PlayerParams playerParams)
        {
            rigid = playerParams.rigidBody;
        }

        public void UpdateState(PlayerParams playerParams)
        {
            if (playerParams.isGrounded)
            {
                if (GameManager.InputHandler.haveJumpInputBeenPressed)
                {
                    playerParams.player.Jump();
                }
                else if (GameManager.InputHandler.isRunButtonBeingPressed)
                {
                    playerParams.player.SetState(new PlayerOnGroundRunningState());
                }
            }
            else
            {
                playerParams.player.SetState(new PlayerFallingState());
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
            return "PlayerGroundState";
        }

        private void Move(PlayerParams playerParams)
        {
            if (playerParams.moveDirection != Vector3.zero)
            {
                playerParams.player.StickToGround();
            }
            playerParams.player.Move(playerParams.walkSpeed);
        }
    }
}