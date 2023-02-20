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
	/// App entry point and main UI flow management.
	/// </summary>
	public class GameLauncher : MonoBehaviour
	{
		private MainMenuUI _mainMenuUI;
		
		[SerializeField] private GameManager _gameManagerPrefab;
		[SerializeField] private Player _playerPrefab;

		private readonly IObserver<FusionLauncher.ConnectionStatus> _connectionStatusChangedBroadcaster =
			MainSceneEvents.OnConnectionStatusBroadcaster;

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
		
		private void Awake()
		{
			DontDestroyOnLoad(this);

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

		private void Start()
		{
			OnConnectionStatusUpdate(null, FusionLauncher.ConnectionStatus.Disconnected, "");
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

			FusionLauncher launcher = FindObjectOfType<FusionLauncher>();
			if (launcher == null)
				launcher = new GameObject("Launcher").AddComponent<FusionLauncher>();

			LevelManager lm = FindObjectOfType<LevelManager>();
			lm.launcher = launcher;

			launcher.Launch(_gameMode, _roomName, lm, OnConnectionStatusUpdate, OnSpawnWorld, OnSpawnPlayer, OnDespawnPlayer);
		}
		

		private void OnConnectionStatusUpdate(NetworkRunner runner, FusionLauncher.ConnectionStatus status, string reason)
		{
			if (!this)
				return;

			Debug.Log(status);

			if (status != _status)
			{
				switch (status)
				{
					case FusionLauncher.ConnectionStatus.Disconnected:
						ErrorBox.Show("Disconnected!", reason, () => { });
						break;
					case FusionLauncher.ConnectionStatus.Failed:
						ErrorBox.Show("Error!", reason, () => { });
						break;
				}
			}

			_status = status;
			_connectionStatusChangedBroadcaster.OnNext(status);
			//UpdateUI();
		}

		private void OnSpawnWorld(NetworkRunner runner)
		{
			Debug.Log("Spawning GameManager");
			runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity, null, InitNetworkState);
			void InitNetworkState(NetworkRunner runner, NetworkObject world)
			{
				world.transform.parent = transform;
			}
		}

		private void OnSpawnPlayer(NetworkRunner runner, PlayerRef playerref)
		{
			if (GameManager.playState != GameManager.PlayState.LOBBY)
			{
				Debug.Log("Not Spawning Player - game has already started");
				return;
			}
			Debug.Log($"Spawning tank for player {playerref}");
			runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, playerref, InitNetworkState);
			void InitNetworkState(NetworkRunner runner, NetworkObject networkObject)
			{
				Player player = networkObject.gameObject.GetComponent<Player>();
				Debug.Log($"Initializing player {player.playerID}");
				player.InitNetworkState(GameManager.MAX_LIVES);
			}
		}

		private void OnDespawnPlayer(NetworkRunner runner, PlayerRef playerref)
		{
			Debug.Log($"Despawning Player {playerref}");
			Player player = PlayerManager.Get(playerref);
			player.TriggerDespawn();
		}
	}
}