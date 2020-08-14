using UnityEngine;
using Core;

namespace FPS
{
    public class PlayerSitedInVehicleState : IPlayerState
    {

        // States used: FallingState, PlayerOnGroundRunningState


        public void OnEnteredState(PlayerParams playerParams)
        {

        }

        public void UpdateState(PlayerParams playerParams)
        {

        }

        public void FixedUpdateState(PlayerParams playerParams)
        {

        }

        public void OnExitState(PlayerParams playerParams)
        {

        }

        public string GetName()
        {
            return "PlayerSitedInVehicleState";
        }
    }
}