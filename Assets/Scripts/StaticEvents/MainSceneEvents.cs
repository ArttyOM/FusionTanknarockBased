using System;
using System.Runtime.CompilerServices;
using Fusion;
using UniRx;
using UnityEngine;

[assembly: InternalsVisibleTo("Assembly-CSharp")]
namespace StaticEvents
{
    public static class MainSceneEvents
    {
        internal readonly static Subject<GameMode> gameModeEvent = new Subject<GameMode>();

        public static IObservable<GameMode> GameModeEvent => gameModeEvent;
    }
}