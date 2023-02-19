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

        public static IObserver<GameMode> GameModeChangedBroadcaster => GameModeSubject;
        public static IObservable<GameMode> OnGameModChanged => GameModeSubject;

        public static IObservable<FusionLauncher.ConnectionStatus> OnConnectionStatusChanged => OnConnectionStatusChangedSubject;
        public static IObserver<FusionLauncher.ConnectionStatus> OnConnectionStatusBroadcaster => OnConnectionStatusChangedSubject;
    }
}