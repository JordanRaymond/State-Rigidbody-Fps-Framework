using UnityEngine;


namespace FPS
{
    [CreateAssetMenu(fileName = "PlayerParams", menuName = "ScriptableObjects/PlayerParamsScriptableObj", order = 2)]

    public class PlayerParams : Params<PlayerParams>
    {
        [Header("INFO")]
        public Vector3 moveDirection;
        public float magnitude;
        [Header("Movement params")]
        public float walkSpeed = 20f;
        public float runSpeed = 30f;
        public float fallingMoveSpeed = 1f;
        public float maxVelocityChange = 20.0f;
        public float jumpHeight = 2.75f;
        public float gravity = 8.0f;
        [Header("Walls Hit")]
        public bool leftWallHit = false;
        public bool rightWallHit = false;
        public float wallRunSpeed = 10f; // More like a multiplier force
        public float wallRunAngle = 50f;
        public float gravityCounterForce = 1;
        public float wallRunForceAppliedTime = 0.75f; // TODO: Find better name
        public float wallJumpForce = 5f;
        public float wallJumpAngle = 45f;
        [Header("---------")]
        public bool isGrounded = false;
        public bool isColliding = false;
        [Header("---------")]
        public float playerHeight = 2;

        [HideInInspector] public FpsController player;
        [HideInInspector] public MouseLook camera;
        [HideInInspector] public Rigidbody rigidBody = null;
        [HideInInspector] public Transform transform = null;


        public bool IsTouchingAWall() { return leftWallHit || rightWallHit; }
        // TODO: Its meh, need uupdate too
        public override void UpdateValues(PlayerParams newValues)
        {
            walkSpeed = newValues.walkSpeed;
            maxVelocityChange = newValues.maxVelocityChange;
            jumpHeight = newValues.jumpHeight;
            gravity = newValues.gravity;
            isGrounded = newValues.isGrounded;
            rigidBody = newValues.rigidBody == null ? rigidBody : newValues.rigidBody;
            transform = newValues.transform == null ? transform : newValues.transform;
        }

    }
}