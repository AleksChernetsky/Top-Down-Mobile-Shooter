using UnityEngine;

namespace TowerDefence.Combat
{
    public enum WeaponType { Rifle, Shotgun }

    [CreateAssetMenu(menuName = "Combat/WeaponConfig")]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Weapon Info")]
        public WeaponType Type;

        [Header("Combat Stats")]
        public int Damage;
        public float Range;
        public float Cooldown;

        [Header("Projectile")]
        public float ProjectileSpeed;
        public Projectile ProjectilePrefab;

        [Header("VFX")]
        public ParticleSystem ImpactEffect;

        [Header("Pooling")]
        public string PoolKey => ProjectilePrefab.name;


        // TODO: Custom editor to show relevant fields based on weapon type
        [Header("Shotgun")]
        public int PelletCount = 3;
        public float SpreadAngle = 10f;
    }
}