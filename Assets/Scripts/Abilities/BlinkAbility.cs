using System;
using UniRx;
using UnityEngine;

namespace Abilities
{
    public class BlinkAbility : UnityEngine.Object, IAbility 
    {
        public readonly AbilityType abilityType = AbilityType.Escape;

        private float _maxCooldown;
        private float _currentCooldown;

        private IDisposable ActivateSubscriber;
        private AbilityView _view;
            
        public AbilityView View
        {
            get { return _view;}
            set
            {
                _view = value;
                ActivateSubscriber?.Dispose();
                ActivateSubscriber = _view.AbilityActivated.Subscribe(_=> Activate());
            }
        }

        private void Activate()
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