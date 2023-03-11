using System;
using Fusion;
using UniRx;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Abilities
{
    public class BlinkAbility : NetworkBehaviour, IAbility 
    {
        public readonly AbilityType abilityType = AbilityType.Escape;

        [SerializeField] private float _maxCooldown = 3;
        private float _currentCooldown = 0;

        private IDisposable ActivateSubscriber;
        private AbilityView _view;

        private bool _canBeReleased = false;
        
        private NetworkCharacterControllerPrototype _cc;
        private Vector2 _aimDirection;
        private Vector2 _moveDirection;

        public void Init(NetworkCharacterControllerPrototype characterController)
        {
            _cc = characterController;

            _canBeReleased = true;
        }
        
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

        private void Release()
        {
            Shift();
            _canBeReleased = false;
            _view.SetCooldown(_maxCooldown);
            Observable.Timer(TimeSpan.FromSeconds((double) _maxCooldown))
                .Subscribe(_ => _canBeReleased = true);
        }
        
        private void Activate()
        {
            if (_canBeReleased)
            {
                Debug.LogWarning("Blink Activated and can be released");
                Release();
            }
            else
            {
                Debug.LogWarning("Blink Activated but on cooldown");
                
            }
        }
        
        private void Shift()
         {
             if (!_canBeReleased)
                 return;
             var _current = _cc.ReadPosition();
             _current.x -= _aimDirection.x * 2;
             _current.z -= _aimDirection.y * 2;
             _cc.TeleportToPosition(_current);
         }

        public void SetDirections(Vector2 moveDirection, Vector2 aimDirection)
        {
            this._aimDirection = aimDirection;
            this._moveDirection = moveDirection;
        }
    }
}