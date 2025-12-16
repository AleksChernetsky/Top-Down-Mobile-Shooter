using UnityEngine;

namespace TowerDefence.Movement
{
    public interface IMovementService
    {
        void SetDestination(Vector3 worldPosition);
        void Stop();
        void Tick(float deltaTime);
    }
}