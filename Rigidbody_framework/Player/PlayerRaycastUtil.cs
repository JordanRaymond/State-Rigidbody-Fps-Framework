using UnityEngine;

namespace FPS
{
    public class PlayerRaycastUtils
    {
        private FpsController player;
        private Transform transform;

        public PlayerRaycastUtils(FpsController p_player)
        {
            player = p_player;
            transform = p_player.transform;
        }

        public bool RaycastForward(out RaycastHit hit, float rayDistance, LayerMask mask, float height = Mathf.Infinity)
        {
            Vector3 forward = transform.forward;

            return Raycast(forward, out hit, rayDistance, mask);
        }

        public bool RaycastLeft(out RaycastHit hit, float rayDistance, LayerMask mask, float height = Mathf.Infinity)
        {
            Vector3 leftDir = -transform.right;

            return Raycast(leftDir, out hit, rayDistance, mask);
        }

        public bool RaycastDiagonalLeft(out RaycastHit hit, float rayDistance, LayerMask mask, float height = Mathf.Infinity)
        {
            Vector3 leftDiagDir = -transform.right + transform.forward;

            return Raycast(leftDiagDir, out hit, rayDistance, mask);
        }

        public bool RaycastRight(out RaycastHit hit, float rayDistance, LayerMask mask, float height = Mathf.Infinity)
        {
            Vector3 rightDir = transform.right;


            return Raycast(rightDir, out hit, rayDistance, mask);
        }

        public bool RaycastDiagonalRight(out RaycastHit hit, float rayDistance, LayerMask mask, float height = Mathf.Infinity)
        {
            Vector3 rightDiagDir = transform.right + transform.forward;

            return Raycast(rightDiagDir, out hit, rayDistance, mask);
        }

        private bool Raycast(Vector3 direction, out RaycastHit hit, float rayDistance, LayerMask mask, float height = Mathf.Infinity)
        {
            Vector3 origin = transform.position;
            if (height != Mathf.Infinity)
            {
                origin.y = height;
                direction.y = height;
            }

            return Physics.Raycast(origin, direction, out hit, rayDistance, mask);
        }
    }
}