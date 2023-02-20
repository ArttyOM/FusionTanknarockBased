using System;
using System.Runtime.CompilerServices;
using Fusion;
using FusionExamples.FusionHelpers;
using UniRx;
using UnityEngine;

[assembly: InternalsVisibleTo("Assembly-CSharp")]
namespace StaticEvents
{
    public static class MainSceneEvents
    {
        private static readonly Subject<GameMode> GameModeSubject = new Subject<GameMode>();
        private static readonly Subject<FusionLauncher.ConnectionStatus> OnConnectionStatusChangedSubject = new Subject<FusionLauncher.ConnectionStatus>();
        private static readonly Subject<Unit> OnEnterRoomSubject = new Subject<Unit>();
        private static readonly Subject<string> OnRoomNameUpdateSubject = new Subject<string>();
        private static readonly Subject<bool> OnProgressShowingSubject = new Subject<bool>();

        public static IObserver<bool> OnProgressShowingBroadcaster => OnProgressShowingSubject;
        public static IObservable<bool> OnProgressShowing => OnProgressShowingSubject;
        
        
        public static IObserver<GameMode> GameModeChangedBroadcaster => GameModeSubject;
        public static IObservable<GameMode> OnGameModUpdate => GameModeSubject;

        
        public static IObserver<FusionLauncher.ConnectionStatus> OnConnectionStatusBroadcaster => OnConnectionStatusChangedSubject;
        public static IObservable<FusionLauncher.ConnectionStatus> OnConnectionStatusChanged => OnConnectionStatusChangedSubject;
        
        
        public static IObserver<Unit> OnEnterRoomBroadcaster => OnEnterRoomSubject;
        public static IObservable<Unit> OnEnterRoom => OnEnterRoomSubject;

        public static IObserver<string> OnRoomNameBroadcaster => OnRoomNameUpdateSubject;
        public static IObservable<string> OnRoomNameUpdate => OnRoomNameUpdateSubject;

    }
}