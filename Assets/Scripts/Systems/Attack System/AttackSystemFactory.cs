using TowerDefence.Combat;

namespace TowerDefence.Systems
{
    public static class AttackSystemFactory
    {
        public static IAttackSystem Create(WeaponConfig config)
        {
            return config.Type switch
            {
                WeaponType.Rifle => new RifleAttackSystem(config),
                WeaponType.Shotgun => new ShotgunAttackSystem(config),
                _ => null
            };
        }
    }
}