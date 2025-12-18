using UnityEngine;

namespace TowerDefence.Combat
{
    public enum WeaponType { Rifle, Shotgun }

    [CreateAssetMenu(menuName = "Combat/WeaponConfig")]
    public class WeaponConfig : ScriptableObject
    {
        public WeaponType Type;
        public int Damage;
        public float Range;
        public float Cooldown;

        public Projectile ProjectilePrefab;
        public string PoolKey => ProjectilePrefab.name;

        // TODO: Custom editor to show relevant fields based on weapon type
        [Header("Shotgun")]
        public int PelletCount = 3;
        public float SpreadAngle = 10f;
    }
}