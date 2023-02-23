using System;
using System.Collections.Generic;
using Fusion;
using FusionExamples.FusionHelpers;
using FusionExamples.UIHelpers;
using StaticEvents;
using Tanknarok;
using Tanknarok.Menu;
using Tanknarok.UI;
using TMPro;
using UniRx;
using UnityEngine;

namespace FusionExamples.Tanknarok
{
	/// <summary>
	/// точка входа
	/// app entry point
	/// main()
	/// bootstrap
	/// </summary>
	public class GameLauncher : MonoBehaviour
	{
		#region Вещатели событий
		private readonly IObserver<FusionLauncher.ConnectionStatus> _connectionStatusChangedBroadcaster =
			MainSceneEvents.OnConnectionStatusBroadcaster;
		#endregion
		
		#region Слушатели событий
		private readonly IObservable<GameMode> _onGameModeUpdate = MainSceneEvents.OnGameModUpdate;
		private readonly IObservable<Unit> _onEnterRoom = MainSceneEvents.OnEnterRoom;
		private readonly IObservable<string> _onRoomNameUpdate = MainSceneEvents.OnRoomNameUpdate;
		private readonly IObservable<bool> _onProgressShowing = MainSceneEvents.OnProgressShowing;
		#endregion
		
		private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
		
		private MainMenuUI _mainMenuUI;
		private FusionLauncher _launcher;

		[SerializeField] private FusionLauncher _launcherPrefab;

		private FusionLauncher.ConnectionStatus _status; 
		private GameMode _gameMode;

		private string _roomName;
		
		private void Awake()
		{
			DontDestroyOnLoad(this);
			
			_mainMenuUI = FindObjectOfType<MainMenuUI>();
			DontDestroyOnLoad(_mainMenuUI);

			SubscribeOnUIEvents();
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

		private void OnDestroy()
		{
			foreach (var subscription in _subscriptions)
			{
				subscription?.Dispose();
			}
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
		
		private void OnEnterRoom()
		{
			_launcher = FindObjectOfType<FusionLauncher>();
			if (_launcher == null)
				_launcher = Instantiate(_launcherPrefab);

			LevelManager lm = FindObjectOfType<LevelManager>();
			lm.launcher = _launcher;

			_launcher.Launch(_gameMode, _roomName, lm);
		}
		
		private void Start()
		{
			_launcher = FindObjectOfType<FusionLauncher>();
			if (_launcher == null)
				_launcher = Instantiate(_launcherPrefab);
			_launcher.Init();

		}
		
	}
}