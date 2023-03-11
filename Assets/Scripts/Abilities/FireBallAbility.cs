using System;
using UniRx;
using UnityEngine;

namespace Abilities
{
    public class FireBallAbility : IAbility
    {
        public readonly AbilityType abilityType = AbilityType.Default;

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
                ActivateSubscriber = _view.AbilityActivated
                    .Subscribe(_=> Activate());
            }
        }

        private void Activate()
        {
            Debug.LogWarning("Fireball Activated");
        }
    }
}