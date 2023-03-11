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

        private BlinkAbility _blinkAbility = new BlinkAbility();
        private FireBallAbility _fireBallAbility = new FireBallAbility();
        
        public void BindAbilitiesModelsAndViews()
        {
            _activated.Where(x => x==AbilityType.Escape)
                .Subscribe(_ => _blinkAbility.Activate());
            
            _activated.Where(x => x==AbilityType.Default)
                .Subscribe(_ => _fireBallAbility.Activate());
            
        }

        // public void InstallWeapon(PowerupElement powerup)
        // {
        //     int weaponIndex = GetWeaponIndex(powerup.powerupType);
        //     ActivateWeapon(powerup.weaponInstallationType, weaponIndex);
        // }
    }
}