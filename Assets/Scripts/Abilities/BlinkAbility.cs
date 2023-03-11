using UnityEngine;

namespace Abilities
{
    public class BlinkAbility : UnityEngine.Object, IAbility 
    {
        public readonly AbilityType abilityType = AbilityType.Escape;

        private float _maxCooldown;
        private float _currentCooldown;

        public void Activate()
        {
            Debug.LogWarning("Blink Activated");
        }
        
        // public void Shift()
        // {
        //     if (!isActivated)
        //         return;
        //     if (AbilityShiftCoolDown > 0) return;
        //     AbilityShiftCoolDown = 3;
        //     var _current = _cc.ReadPosition();
        //     _current.x -= aimDirection.x * 2;
        //     _current.z -= aimDirection.y * 2;
        //     _cc.TeleportToPosition(_current);
        //     _shiftButton.active = false; //���������� �������� ������ � ������ ������, �� ������ �� ���qa
        // }
    }
}