using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.Movement
{
    public interface ITargetSearchService : IService
    {
        Transform Target(Transform self, float radius, LayerMask enemyMask, LayerMask obstacleMask);
    }

    public class TargetSearchService : ITargetSearchService
    {
        public void Init() { }

        public Transform Target(Transform self, float radius, LayerMask enemyMask, LayerMask obstacleMask)
        {
            Collider[] hits = Physics.OverlapSphere(self.position, radius, enemyMask);

            float minDist = float.MaxValue;
            Transform best = null;

            foreach (var hit in hits)
            {
                var vitality = hit.GetComponent<IVitalitySystem>();
                if (vitality == null || vitality.IsDead)
                    continue;

                Vector3 dir = hit.transform.position - self.position;
                float dist = dir.sqrMagnitude;

                if (Physics.Raycast(self.position + Vector3.up * 1.5f, dir.normalized, Mathf.Sqrt(dist), obstacleMask))
                    continue;

                if (dist < minDist)
                {
                    minDist = dist;
                    best = hit.transform;
                }
            }

            return best;
        }
    }
}