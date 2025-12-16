using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Movement
{
    public class BotMovement : MonoBehaviour
    {
        [SerializeField] private Transform _body;
        private Transform _target;

        private IMovementService _movement;

        private void Awake()
        {
            var agent = GetComponent<NavMeshAgent>();
            _movement = new CharacterMovementService(agent, _body);
        }

        private void Update()
        {
            if (_target == null)
            {
                _movement.Stop();
                return;
            }

            _movement.SetDestination(_target.position);
        }
    }
}