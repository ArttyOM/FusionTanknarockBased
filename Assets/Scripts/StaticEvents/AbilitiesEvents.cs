using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Abilities;
using Fusion;
using UniRx;
using UnityEngine;

[assembly: InternalsVisibleTo("Assembly-CSharp")]

namespace StaticEvents
{
    public static class AbilitiesEvents
    {
        private static readonly Subject<AbilityType> ActivatedSubject = new Subject<AbilityType>();
        private static readonly Subject<AbilityType> ReleasedSubject = new Subject<AbilityType>();
        private static readonly Subject<AbilityType> ReadyToCastSubject = new Subject<AbilityType>();
        private static readonly Subject<AbilityType> DeactivatedSubject = new Subject<AbilityType>();
        

        public static IObserver<AbilityType> ActivatedBroadcaster => ActivatedSubject;
        public static IObservable<AbilityType> Activated => ActivatedSubject;
        
        public static IObserver<AbilityType> ReleasedBroadcaster => ReleasedSubject;
        public static IObservable<AbilityType> Released => ReleasedSubject;
        
        public static IObserver<AbilityType> DeactivatedBroadcaster => DeactivatedSubject;
        public static IObserver<AbilityType> Deactivated => DeactivatedSubject;
        
        public static IObserver<AbilityType> ReadyToCastBroadcaster => ReadyToCastSubject;
        public static IObserver<AbilityType> ReadyToCast => ReadyToCastSubject;
    }
}
