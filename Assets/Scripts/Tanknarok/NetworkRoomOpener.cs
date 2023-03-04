using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Fusion;
using FusionExamples.FusionHelpers;
using FusionExamples.Tanknarok;
using StaticEvents;
using UniRx;
using UnityEngine;

namespace Tanknarok
{

    /// <summary>
    /// Входит и выходит из сетевой комнаты.
    /// </summary>
    public class NetworkRoomOpener : UnityEngine.Object
    {
        public NetworkRoomOpener(NetworkRunnerCallbacksHandler launcherPrefab)
        {
            _launcherPrefab = launcherPrefab;
        }

        ~NetworkRoomOpener()
        {
            OnDestroy();
        }

        private NetworkRunnerCallbacksHandler _launcherPrefab;

    #region Слушатели событий

    private readonly IObservable<GameMode> _onGameModeUpdate = MainSceneEvents.OnGameModUpdate;
    private readonly IObservable<Unit> _onEnterRoom = MainSceneEvents.OnEnterRoom;
    private readonly IObservable<string> _onRoomNameUpdate = MainSceneEvents.OnRoomNameUpdate;
    private readonly IObservable<bool> _onProgressShowing = MainSceneEvents.OnProgressShowing;

    #endregion

    private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

    private NetworkRunnerCallbacksHandler.ConnectionStatus _status;
    
    /// <summary>
    /// Как создать комнату?
    /// Соло, Сервер, Хост, Клиент и тд
    /// </summary>
    private GameMode _gameMode;

    /// <summary>
    /// Идентифика
    /// </summary>
    private string _roomName;
    private NetworkRunnerCallbacksHandler _launcher;

    /// <summary>
    /// Почему-то UI ломается, если вызвать на Awake.
    /// Использовать на Start
    /// </summary>
    public void Init()
    {
        _launcher = FindObjectOfType<NetworkRunnerCallbacksHandler>();
        if (_launcher == null)
            _launcher = Instantiate(_launcherPrefab);
        SubscribeOnUIEvents();
    }

    private void OnDestroy()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription?.Dispose();
        }
    }

    private void SubscribeOnUIEvents()
    {
        IDisposable onGameModeUpdateSubscription = _onGameModeUpdate.Subscribe(gameMode => _gameMode = gameMode);
        _subscriptions.Add(onGameModeUpdateSubscription);

        IDisposable onEnterRoomSubscription = _onEnterRoom.Subscribe(_ => OnEnterRoom());
        _subscriptions.Add(onEnterRoomSubscription);

        IDisposable onRoomNameUpdateSubscription = _onRoomNameUpdate.Subscribe(newText => _roomName = newText);
        _subscriptions.Add(onRoomNameUpdateSubscription);

        IDisposable onProgressShowingSubscription = _onProgressShowing
            .Where(isUiProgressShowing => isUiProgressShowing)
            .Where(_ => Input.GetKeyUp(KeyCode.Escape))
            .Subscribe(_ => ShutdownNetwork());
        _subscriptions.Add(onProgressShowingSubscription);
    }

    private void OnEnterRoom()
    {
        _launcher = FindObjectOfType<NetworkRunnerCallbacksHandler>();
        if (_launcher == null)
            _launcher = Instantiate(_launcherPrefab);

        NetworkSceneLoader lm = FindObjectOfType<NetworkSceneLoader>();
        //lm.launcher = _launcher;

        _launcher.Launch(_gameMode, _roomName, lm);
    }

    private void ShutdownNetwork()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner != null && !runner.IsShutdown)
        {
            // Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
            runner.Shutdown(false);
        }
    }


    }
}