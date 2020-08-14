using UnityEngine;
using System.Collections.Generic;
using Core;

namespace FPS
{
    public class PlayerOnWallState : IPlayerState
    {
        private bool canWallRun;
        private Vector3 beforeJumpVelocity;
        private float wallRunTime = 0f;
        private RaycastHit wallHit;
        private bool alreadyJumped = false;
        private bool jump = false;
        private Transform cameraTransform;

        public PlayerOnWallState(RaycastHit p_wallHit, bool p_canWallrun = false)
        {
            wallHit = p_wallHit;
            canWallRun = p_canWallrun;
        }

        public void OnEnteredState(PlayerParams playerParams)
        {
            beforeJumpVelocity = playerParams.rigidBody.velocity;
            wallRunTime = playerParams.wallRunForceAppliedTime;

            cameraTransform = playerParams.player.mouseLook.cameraTransform;
        }

        public void UpdateState(PlayerParams playerParams)
        {
            if (!jump) jump = GameManager.InputHandler.haveJumpInputBeenPressed;

            wallRunTime -= Time.deltaTime;
            if (wallRunTime < 0) canWallRun = false;

            if (!playerParams.IsTouchingAWall() || playerParams.isGrounded)
            {
                playerParams.player.SetState(new PlayerFallingState(false));
            }
        }

        public void FixedUpdateState(PlayerParams playerParams)
        {
            // if toching wall, else falling state or onground
            if (canWallRun)
            {
                WallRun(playerParams);
            }

            HandleWallSliding(playerParams, wallHit);
            HandleWallJumping(playerParams);
        }

        public void OnExitState(PlayerParams playerParams)
        {

        }

        public string GetName()
        {
            return "PlayerWallRunState";
        }

        private void HandleWallSliding(PlayerParams playerParams, RaycastHit wallHit)
        {
            // If is sliding down
            if (Vector3.Dot(playerParams.rigidBody.velocity, Vector3.up) < 0)
            {
                playerParams.rigidBody.AddForce(new Vector3(0, playerParams.gravityCounterForce, 0), ForceMode.Force);
            }
        }

        private void HandleWallJumping(PlayerParams playerParams)
        {
            if (jump && !alreadyJumped)
            {
                jump = false;
                alreadyJumped = true;

                Vector3 perpandicularVec = playerParams.player.GetVectorParalelToWall(wallHit);
                Vector3 axis = playerParams.leftWallHit ? perpandicularVec : -perpandicularVec;
                Vector3 rotatedVector = Quaternion.AngleAxis(playerParams.wallJumpAngle, axis) * wallHit.normal;

                rotatedVector *= playerParams.wallJumpForce;
                Vector3 direction = rotatedVector + playerParams.rigidBody.velocity;
                direction = Vector3.ClampMagnitude(direction, playerParams.wallRunSpeed);
                playerParams.player.Jump(direction);
            }
        }

        private void WallRun(PlayerParams playerParams)
        {
            RaycastHit wallHit = playerParams.player.GetClosestWall();
            if (playerParams.player.IsMovingInDirectionOfTheWall())
            {
                // GetVectorParalelToWall 
                Vector3 targetDirection = playerParams.player.GetVectorParalelToWall(wallHit);
                // Rotate the vector paralelle to the wall upward at wallRunAngle
                // The normal direction should always be from left to right else the rotation will be downward 
                Vector3 axis = playerParams.leftWallHit ? wallHit.normal : -wallHit.normal;

                float angle = Vector3.Dot(cameraTransform.forward, Vector3.up) > 0 ? -GetRunAngle(playerParams, targetDirection) : 0;
                Vector3 rotatedTargetDirection = Quaternion.AngleAxis(angle, axis) * targetDirection;
                Vector3 targetVelocity = (rotatedTargetDirection * playerParams.wallRunSpeed);

                // TODO
                Vector3 currentVel = playerParams.rigidBody.velocity;
                currentVel.Normalize();
                targetVelocity += currentVel;
                playerParams.rigidBody.velocity = targetVelocity;
            }
        }

        private float GetRunAngle(PlayerParams playerParams, Vector3 perpandicularVector)
        {
            return Vector3.Angle(cameraTransform.forward, perpandicularVector);
        }
    }
}