using System;
using System.Collections.Generic;
using Fusion;
using StaticEvents;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Abilities
{
    public class PlayerDefaultAbilities:  NetworkBehaviour
    {
        IObservable<AbilityType> _activated = AbilitiesEvents.Activated;

        [SerializeField] private BlinkAbility _blinkAbilityPattern;// = new BlinkAbility();
        private BlinkAbility _blinkAbility;
        private FireBallAbility _fireBallAbility = new FireBallAbility();
        
        

        public void BindAbilitiesModelsAndViews()
        {
            var views =FindObjectsOfType<AbilityView>();
            for (int i = 0; i < views.Length; i++)
            {
                switch (views[i].GetAbilityType)
                {
                    case AbilityType.Escape:
                        _blinkAbility.View = views[i];
                        break;
                    case AbilityType.Default:
                        _fireBallAbility.View = views[i];
                        break;
                    case AbilityType.Melee:
                        break;
                    default: break;
                }
            }
            
            // _activated.Where(x => x==AbilityType.Escape)
            //     .Subscribe(_ => _blinkAbility.Activate());
            
            // _activated.Where(x => x==AbilityType.Default)
            //     .Subscribe(_ => _fireBallAbility.Activate());
            
        }
        
        
        // public void InstallWeapon(PowerupElement powerup)
        // {
        //     int weaponIndex = GetWeaponIndex(powerup.powerupType);
        //     ActivateWeapon(powerup.weaponInstallationType, weaponIndex);
        // }
        public void InitAbilities(NetworkCharacterControllerPrototype characterController)
        {
            _blinkAbility = Instantiate(_blinkAbilityPattern);
            _blinkAbility.Init(characterController);
        }

        public void SetDirections(Vector2 moveDirection, Vector2 aimDirection)
        {
            _blinkAbility.SetDirections(moveDirection, aimDirection);
        }
    }
}