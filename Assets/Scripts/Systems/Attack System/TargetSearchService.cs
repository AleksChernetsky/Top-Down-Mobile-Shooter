using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.Systems
{
    public interface ITargetSearchService : IService
    {
        Transform Target(CharacterIdentity self, float radius);
    }

    public class TargetSearchService : ITargetSearchService
    {
        private readonly LayerMask UnitLayer = LayerMask.GetMask("Unit");
        private readonly LayerMask ObstacleLayer = LayerMask.GetMask("Obstacle");

        public void Init() { }

        public Transform Target(CharacterIdentity self, float radius)
        {
            Collider[] hits = Physics.OverlapSphere(self.transform.position, radius, UnitLayer);

            float minDist = float.MaxValue;
            Transform best = null;

            foreach (var hit in hits)
            {
                if (hit.transform == self)
                    continue;

                var vitality = hit.GetComponent<IVitalitySystem>();
                if (vitality == null || vitality.IsDead)
                    continue;

                var targetIdentity = hit.GetComponent<IIdentity>();
                if (!self.IsEnemy(targetIdentity))
                    continue;

                Vector3 dir = hit.transform.position - self.transform.position;
                float dist = dir.sqrMagnitude;

                if (dist >= minDist)
                    continue;

                Vector3 origin = self.transform.position;
                if (Physics.Raycast(origin, dir.normalized, Mathf.Sqrt(dist), ObstacleLayer))
                    continue;

                minDist = dist;
                best = hit.transform;
            }

            return best;
        }
    }

    public struct CombatTarget
    {
        public Transform Transform;
        public IVitalitySystem Vitality;
        public IIdentity Identity;

        public bool IsValid => Transform != null && Vitality != null && !Vitality.IsDead;

        public static CombatTarget None => new CombatTarget();
    }
}