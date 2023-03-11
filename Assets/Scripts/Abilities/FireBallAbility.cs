using System;
using Fusion;
using UniRx;
using UnityEngine;

namespace Abilities
{
    public class FireBallAbility : NetworkBehaviour, IAbility
    {
        public readonly AbilityType abilityType = AbilityType.Default;

        [SerializeField] private float _maxCooldown = 0.3f;
        private float _currentCooldown = 0;

        private bool _canBeReleased = false;

        private IDisposable ActivateSubscriber;
        private AbilityView _view;

        [SerializeField] private string _hotKeyName;
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

        private void Awake()
        {
            _canBeReleased = true;
        }

        
        private void Activate()
        {
            if (_canBeReleased)
            {
                Debug.LogWarning("Fireball Activated and can be released");
                Release();
            }
            else
            {
                Debug.LogWarning("Fireball Activated but on cooldown");
                
            }
        }
        
        private void Release()
        {
            _canBeReleased = false;
            _view.SetCooldown(_maxCooldown);
            Observable.Timer(TimeSpan.FromSeconds((double) _maxCooldown))
                .Subscribe(_ => _canBeReleased = true);
        }
    }
}