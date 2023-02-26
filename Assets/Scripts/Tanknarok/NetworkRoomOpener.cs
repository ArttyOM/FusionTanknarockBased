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
    /// Хранит актуальный gameMode, roomName,
    /// 
    /// </summary>
    public class NetworkRoomOpener : UnityEngine.Object
    {
        public NetworkRoomOpener(FusionLauncher launcherPrefab)
        {
            _launcherPrefab = launcherPrefab;


        }

        ~NetworkRoomOpener()
        {
            OnDestroy();
        }

        [SerializeField] private FusionLauncher _launcherPrefab;

    #region Слушатели событий

    private readonly IObservable<GameMode> _onGameModeUpdate = MainSceneEvents.OnGameModUpdate;
    private readonly IObservable<Unit> _onEnterRoom = MainSceneEvents.OnEnterRoom;
    private readonly IObservable<string> _onRoomNameUpdate = MainSceneEvents.OnRoomNameUpdate;
    private readonly IObservable<bool> _onProgressShowing = MainSceneEvents.OnProgressShowing;

    #endregion

    private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

    private FusionLauncher.ConnectionStatus _status;
    private GameMode _gameMode;

    private string _roomName;
    private FusionLauncher _launcher;

    public void Init()
    {
        Start();
    }

    private void Start()
    {
        _launcher = FindObjectOfType<FusionLauncher>();
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
        _launcher = FindObjectOfType<FusionLauncher>();
        if (_launcher == null)
            _launcher = Instantiate(_launcherPrefab);

        NetworkSceneManager lm = FindObjectOfType<NetworkSceneManager>();
        lm.launcher = _launcher;

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