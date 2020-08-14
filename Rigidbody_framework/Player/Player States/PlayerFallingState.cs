using UnityEngine;

namespace FPS
{
    public class PlayerFallingState : IPlayerState
    {
        Vector3 beforeJumpVelocity;
        bool canWallRun;

        // So, if the state calling the fall state is a valid state where your can wall run from
        // set to true
        public PlayerFallingState(bool p_canWallRun = false)
        {
            canWallRun = p_canWallRun;
        }

        public void OnEnteredState(PlayerParams playerParams)
        {
            beforeJumpVelocity = playerParams.rigidBody.velocity;
        }

        public void UpdateState(PlayerParams playerParams)
        {
            if (playerParams.IsTouchingAWall())
            {
                RaycastHit wallHit = playerParams.player.GetClosestWall();
                // If the player is not just toching the wall but the direction is going toward the wall.
                if (playerParams.player.IsMovingInDirectionOfTheWall())
                {
                    playerParams.player.SetState(new PlayerOnWallState(wallHit, canWallRun));
                }
            }

            if (playerParams.isGrounded)
            {
                playerParams.player.SetState(new PlayerOnGroundState());
            }
        }

        public void FixedUpdateState(PlayerParams playerParams)
        {
            if (!playerParams.isGrounded)
            {
                Move(playerParams);
            }
        }

        public void OnExitState(PlayerParams playerParams)
        {

        }

        public string GetName()
        {
            return "PlayerFallingState";
        }

        private void Move(PlayerParams playerParams)
        {
            Vector3 targetDirection = playerParams.moveDirection;

            if (!playerParams.IsTouchingAWall() && targetDirection != Vector3.zero && !playerParams.isColliding)
            {
                Vector3 velocity = playerParams.rigidBody.velocity;
                Vector3 targetVelocity = beforeJumpVelocity + (targetDirection * playerParams.fallingMoveSpeed);

                // playerParams.player.Move(targetVelocity);
                playerParams.rigidBody.AddForce(targetDirection * playerParams.fallingMoveSpeed, ForceMode.Force);
            }
        }
    }
}