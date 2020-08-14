
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace FPS
{
    public class FpsController : MonoBehaviour
    {
        public PlayerParams defaultPlayerParams;
        public PlayerOnGroundState defaultState = new PlayerOnGroundState();

        [HideInInspector] public InputHandler inputHandler;
        [HideInInspector] public MouseLook mouseLook;

        // Is on ground params
        [Header("Is grounded check")]
        public Transform groundCheck; // set to (0, -0.75, 0)
        public float groundPhysicSphereWidth = 0.4f; // Capsule width is 0.5f
        public LayerMask groundMask;

        // Wall check params
        [Header("Run on wall params")]
        public float wallRayDistance = 0.75f; // Capsule width is 0.5f, 0.5f to get out of the body + 25
        public LayerMask wallLayerMask;
        // Wall check params
        [Header("Obstacle Check params")]
        public float obstaclesRayDistance = 0.75f; // Capsule width is 0.5f, 0.5f to get out of the body + 25
        public LayerMask obstaclesRayLayeryMask;
        // Gizmos params
        [Header("Slope params")]
        public float stickToGroundRayLength = 1.5f; // Capsule height is 2, 1 is the height of the cilender + 0.5f for each sphere half
        public float minFloorAngle = 30f;

        [HideInInspector]
        public IPlayerState currentState;
        [HideInInspector]
        public List<RaycastHit> wallsHits = new List<RaycastHit>();

        private Vector3 inputDir;
        private Vector3 groudCheckSpherePosition;

        private PlayerRaycastUtils raycastUtils;

        void Start()
        {
            InitPlayerParams();

            raycastUtils = new PlayerRaycastUtils(this);
            inputHandler = GameManager.InputHandler;

            mouseLook = GetComponent<MouseLook>();
            SetState(defaultState);
        }

        private void InitPlayerParams()
        {
            defaultPlayerParams.player = this;
            defaultPlayerParams.rigidBody = GetComponent<Rigidbody>();
            defaultPlayerParams.transform = GetComponent<Transform>();
            mouseLook = GetComponent<MouseLook>();
            defaultPlayerParams.camera = mouseLook;
        }

        void Update()
        {
            // Debug.Log(playerParams.rigidBody.velocity.magnitude);
            // Debug.Log(Input.GetAxis("Mouse X2"));
            // Debug.Log(Input.GetAxis("Mouse Y"));

            mouseLook.RotateCamera();

            defaultPlayerParams.isGrounded = IsGrounded();
            CheckIsTouchingWalls();

            HandleInputs(); // TODO Change name
            currentState.UpdateState(defaultPlayerParams);
            GameManager.InputHandler.haveJumpInputBeenPressed = false;
        }

        void FixedUpdate()
        {
            currentState.FixedUpdateState(defaultPlayerParams); // jump
            mouseLook.RotatePlayer(defaultPlayerParams);

            // Apply gravity manually for more tuning control
            defaultPlayerParams.rigidBody.AddForce(new Vector3(0, -defaultPlayerParams.gravity * defaultPlayerParams.rigidBody.mass, 0));

            // playerParams.rigidBody.rotation *= Quaternion.Euler(Vector3.up * 50 * Time.fixedDeltaTime);
        }

        void OnCollisionEnter()
        {
            defaultPlayerParams.isColliding = true;
        }

        void OnCollisionExit()
        {
            defaultPlayerParams.isColliding = false;
        }

        void HandleInputs()
        {
            defaultPlayerParams.moveDirection = CalculateMoveDir();
            defaultPlayerParams.magnitude = defaultPlayerParams.rigidBody.velocity.magnitude;
        }

        public void SetState(IPlayerState state)
        {
            if (state != null)
            {
                // Debug.Log("State set:" + state.GetName());
                if (currentState != null)
                {
                    currentState.OnExitState(defaultPlayerParams);
                }
                currentState = state;
                currentState.OnEnteredState(defaultPlayerParams);
            }
            else
            {
                Debug.LogError("Can't enter null state");
            }
        }

        public Vector3 Move(float moveSpeed, bool handleObstacle = true)
        {
            Vector3 moveDirection = defaultPlayerParams.moveDirection;
            if (handleObstacle)
            {
                RaycastHit obstacleHit;
                if (IsTouchingObstacle(moveDirection, out obstacleHit) && IsGoingIntoObstacle(obstacleHit, moveDirection))
                {
                    // Its a dir
                    Vector3 vectorParalelToWall = GetVectorParalelToWall(obstacleHit);
                    if (Vector3.Dot(moveDirection, defaultPlayerParams.transform.forward) < 0) vectorParalelToWall *= -1;

                    moveDirection = vectorParalelToWall;
                }
            }

            Vector3 currentVelocity = defaultPlayerParams.rigidBody.velocity;

            Vector3 newVelocity;
            if (moveDirection != Vector3.zero)
            {
                newVelocity = moveDirection * moveSpeed;
            }
            else
            {
                newVelocity = new Vector3(0, currentVelocity.y, 0);
            }

            newVelocity = Vector3.ClampMagnitude(newVelocity, moveSpeed);
            newVelocity.y = currentVelocity.y;

            defaultPlayerParams.rigidBody.velocity = newVelocity;

            return newVelocity;
        }

        // V1
        private Vector3 CalculateVelocityChange(Vector3 targetVelocity, bool keepY = false)
        {
            // Calculate a force that attempts to reach our target velocity
            float maxVelocityChange = defaultPlayerParams.maxVelocityChange;

            return CalculateVelocityChange(targetVelocity, maxVelocityChange, keepY);
        }

        private Vector3 CalculateVelocityChange(Vector3 targetVelocity, float maxVelocityChange, bool keepY)
        {
            // Calculate a force that attempts to reach our target velocity
            Vector3 velocity = defaultPlayerParams.rigidBody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

            velocityChange.y = keepY ? Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange)
            : 0;

            return velocityChange;
        }

        public void Jump()
        {
            defaultPlayerParams.rigidBody.AddForce(new Vector3(0, CalculateJumpVerticalSpeed(), 0), ForceMode.Impulse);
            GameManager.InputHandler.haveJumpInputBeenPressed = false;
        }

        public void Jump(Vector3 direction)
        {
            defaultPlayerParams.rigidBody.AddForce(direction, ForceMode.Impulse);
            GameManager.InputHandler.haveJumpInputBeenPressed = false;
        }

        public Vector3 CalculateMoveDir()
        {
            Vector3 inputDir = GameManager.InputHandler.GetMoveInput(); // TODO: Implement controller 

            float horizontal = inputDir.x;
            float vertical = inputDir.z;

            Vector3 moveDir = (defaultPlayerParams.transform.forward * vertical) + (defaultPlayerParams.transform.right * horizontal);

            return moveDir;
        }

        private float CalculateJumpVerticalSpeed()
        {
            // From the jump height and gravity we deduce the upwards speed 
            // for the character to reach at the apex.
            float gravity = defaultPlayerParams.gravity < 0 ? 0f : defaultPlayerParams.gravity;
            return Mathf.Sqrt(2 * defaultPlayerParams.jumpHeight * gravity);
        }

        public void StickToGround()
        {
            RaycastHit hit;
            Transform transform = defaultPlayerParams.transform;

            if (inputHandler.haveJumpInputBeenPressed) return;
            if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, stickToGroundRayLength, groundMask))
            {
                // Debug.DrawLine(transform.position + transform.forward * 1.5f, hit.point, Color.green);
                float floorAngle = Vector3.Angle(transform.up, hit.normal);
                if (floorAngle >= minFloorAngle)
                {
                    if (defaultPlayerParams.rigidBody.velocity.y <= 0)
                    {
                        defaultPlayerParams.transform.position = new Vector3(transform.position.x, hit.point.y + defaultPlayerParams.playerHeight / 2 + 0.03f, transform.position.z);
                        // playerParams.rigidBody.AddForce(new Vector3(0, -playerParams.gravity * 0.5f, 0), ForceMode.Force);
                    }
                }
            }
        }

        private bool IsGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundPhysicSphereWidth, groundMask);
        }

        public List<RaycastHit> CheckIsTouchingWalls()
        {
            RaycastHit hit;
            wallsHits.Clear();
            defaultPlayerParams.leftWallHit = false;
            defaultPlayerParams.rightWallHit = false;

            // Left
            if (raycastUtils.RaycastLeft(out hit, wallRayDistance, wallLayerMask))
            {
                defaultPlayerParams.leftWallHit = true;
                wallsHits.Add(hit);
            }

            if (raycastUtils.RaycastDiagonalLeft(out hit, wallRayDistance, wallLayerMask))
            {
                defaultPlayerParams.leftWallHit = true;
                wallsHits.Add(hit);
            }

            // Right
            if (raycastUtils.RaycastRight(out hit, wallRayDistance, wallLayerMask))
            {
                defaultPlayerParams.rightWallHit = true;
                wallsHits.Add(hit);
            }

            if (raycastUtils.RaycastDiagonalRight(out hit, wallRayDistance, wallLayerMask))
            {
                defaultPlayerParams.rightWallHit = true;
                wallsHits.Add(hit);
            }

            return wallsHits;
        }

        // Don't like this
        public bool IsTouchingObstacle(Vector3 moveDir, out RaycastHit validhit)
        {
            Vector3 legsPosition = transform.position;
            float height = legsPosition.y - 0.5f;

            // Forward
            if (raycastUtils.RaycastForward(out validhit, obstaclesRayDistance, obstaclesRayLayeryMask, height))
            {
                return true;
            }

            // Left
            if (raycastUtils.RaycastLeft(out validhit, obstaclesRayDistance, obstaclesRayLayeryMask, height))
            {
                return true;
            }

            if (raycastUtils.RaycastDiagonalLeft(out validhit, obstaclesRayDistance, obstaclesRayLayeryMask, height))
            {
                return true;
            }

            // Right
            if (raycastUtils.RaycastRight(out validhit, obstaclesRayDistance, obstaclesRayLayeryMask, height))
            {
                return true;
            }

            if (raycastUtils.RaycastDiagonalRight(out validhit, obstaclesRayDistance, obstaclesRayLayeryMask, height))
            {
                return true;
            }

            return false;
        }

        public bool IsGoingIntoObstacle(RaycastHit hit, Vector3 movingDirection)
        {
            float dotProduct = Vector3.Dot(hit.normal, movingDirection);
            return dotProduct < 0 && dotProduct > -0.975f;
        }

        public bool IsMovingInDirectionOfTheWall()
        {
            if (defaultPlayerParams.leftWallHit)
            {
                if (GameManager.InputHandler.GetMoveInput().x < 0) return true;
            }
            if (defaultPlayerParams.rightWallHit)
            {
                if (GameManager.InputHandler.GetMoveInput().x > 0) return true;
            }

            return false;
        }

        public RaycastHit GetClosestWall()
        {
            List<RaycastHit> wallsHits = defaultPlayerParams.player.wallsHits;

            float lastDis = Mathf.Infinity;
            RaycastHit closet = wallsHits[0];
            foreach (RaycastHit hit in wallsHits)
            {
                if (hit.distance <= lastDis) closet = hit;
            }

            return closet;
        }

        public Vector3 GetVectorParalelToWall(RaycastHit wallHit)
        {
            Vector3 wallPerpVector = Vector3.Cross(wallHit.normal, Vector3.up).normalized;
            if (Vector3.Dot(wallPerpVector, defaultPlayerParams.transform.forward) < 0) wallPerpVector *= -1;

            return wallPerpVector;
        }

        // DEBUG CODE
        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawSphere(groundCheck.position, groundPhysicSphereWidth);

            // int wallLayerMask = 1 << 9;
            // RaycastHit hit;
            // Vector3 leftDir = -transform.right;
            // if (Physics.Raycast(transform.position, leftDir, out hit, wallRayDistance, wallLayerMask))
            // {
            //     DrawWallRaysHitsInfo(hit);
            // }

            // Vector3 rightDir = transform.right;
            // if (Physics.Raycast(transform.position, rightDir, out hit, wallRayDistance, wallLayerMask))
            // {
            //     DrawWallRaysHitsInfo(hit);

            // }

            // Vector3 rightDiagDir = transform.right + transform.forward;
            // if (Physics.Raycast(transform.position, rightDiagDir, out hit, wallRayDistance, wallLayerMask))
            // {
            //     DrawWallRaysHitsInfo(hit);

            // }

            // Vector3 leftDiagDir = -transform.right + transform.forward;
            // if (Physics.Raycast(transform.position, leftDiagDir, out hit, wallRayDistance, wallLayerMask))
            // {
            //     DrawWallRaysHitsInfo(hit);
            // }

            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Vector3 legsPosition = transform.position;
                legsPosition.y -= 0.50f;
                Gizmos.DrawLine(legsPosition, legsPosition + CalculateMoveDir() * obstaclesRayDistance);
            }



            // RaycastHit wallHit = playerParams.player.GetClosestWall();
            // Vector3 targetDirection = playerParams.player.GetVectorParalelToWall(wallHit);
            // // Rotate the vector paralelle to the wall upward at wallRunAngle 
            // Vector3 axis = playerParams.leftWallHit ? wallHit.normal : -wallHit.normal;
            // Vector3 rotatedVector = Quaternion.AngleAxis(-playerParams.wallRunAngle, axis) * targetDirection;
            // Vector3 targetVelocity = (rotatedVector * playerParams.wallRunSpeed);
            // Debug.DrawRay(playerParams.transform.position, targetVelocity, Color.green);
            // playerParams.rigidBody.AddForce(new Vector3(0, playerParams.gravity * 2 * playerParams.rigidBody.mass + playerParams.gravityCounterForce, 0));

        }
        private void DrawWallRaysHitsInfo(RaycastHit hit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, hit.point);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(hit.point, hit.point + hit.normal); // Draw the normal

            Gizmos.color = Color.red;
            Vector3 wwallPerpVector = Vector3.Cross(hit.normal, Vector3.up).normalized;

            Gizmos.DrawLine(hit.point + hit.normal, hit.point + hit.normal + wwallPerpVector); // Draw the line paralelle to the wall
        }
    }
}

// OLD CODE
// private Dictionary<ForceMode, Vector3> totalForcesDic = new Dictionary<ForceMode, Vector3>();
// Did nothing diferent
// public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
// {
//     if (totalForcesDic.ContainsKey(forceMode))
//     {
//         totalForcesDic[forceMode] += force;
//     }
//     else
//     {
//         totalForcesDic.Add(forceMode, force);
//     }
// }

// public void ApplyForces()
// {
//     foreach (KeyValuePair<ForceMode, Vector3> kvp in totalForcesDic)
//     {
//         playerParams.rigidBody.AddForce(kvp.Value, kvp.Key);
//     }

//     totalForcesDic.Clear();
// }

// V2, shit
// AccelerateTo: https://gamedev.stackexchange.com/questions/113178/when-should-i-use-velocity-versus-addforce-when-dealing-with-player-objects
// public Vector3 CalculateVelocityChange(Vector3 targetVelocity, float maxAccel)
// {
//     Vector3 deltaV = targetVelocity - playerParams.rigidBody.velocity;
//     Vector3 accel = deltaV / Time.deltaTime;

//     if (accel.sqrMagnitude > maxAccel * maxAccel)
//         accel = accel.normalized * maxAccel;

//     accel.y = 0;
//     return accel;
//     // playerParams.rigidBody.AddForce(accel, ForceMode.Acceleration);
// }
