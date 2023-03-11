using UnityEngine;

namespace Abilities
{
    public class FireBallAbility : IAbility
    {
        public readonly AbilityType abilityType = AbilityType.Default;

        private float _maxCooldown;
        private float _currentCooldown;

        public void Activate()
        {
            Debug.LogWarning("Fireball Activated");
        }
    }
}