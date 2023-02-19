using System;
using Fusion;
using FusionExamples.FusionHelpers;
using FusionExamples.UIHelpers;
using StaticEvents;
using Tanknarok;
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
		[SerializeField] private MainMenuUI _mainMenuUI;
		
		[SerializeField] private GameManager _gameManagerPrefab;
		[SerializeField] private Player _playerPrefab;
		[SerializeField] private TMP_InputField _room;
		[SerializeField] private TextMeshProUGUI _progress;
		[SerializeField] private Panel _uiCurtain;
		[SerializeField] private Panel _uiStart;
		[SerializeField] private Panel _uiProgress;
		[SerializeField] private Panel _uiRoom;
		[SerializeField] private GameObject _uiGame;

		private readonly IObserver<FusionLauncher.ConnectionStatus> _connectionStatusChangedBroadcaster =
			MainSceneEvents.OnConnectionStatusBroadcaster;
		
		private readonly IObservable<GameMode> _gameModeEvent = MainSceneEvents.OnGameModChanged;
		private IDisposable _gameModeEventSubscription;

		private FusionLauncher.ConnectionStatus _status; 
		private GameMode _gameMode;
		
		
		private void Awake()
		{
			DontDestroyOnLoad(this);

			_gameModeEventSubscription = _gameModeEvent.Subscribe(gameMode => _gameMode = gameMode);
		}

		private void OnDestroy()
		{
			_gameModeEventSubscription?.Dispose();
		}

		private void Start()
		{
			OnConnectionStatusUpdate(null, FusionLauncher.ConnectionStatus.Disconnected, "");
		}

		private void Update()
		{
			if (_uiProgress.isShowing)
			{
				if (Input.GetKeyUp(KeyCode.Escape))
				{
					NetworkRunner runner = FindObjectOfType<NetworkRunner>();
					if (runner != null && !runner.IsShutdown)
					{
						// Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
						runner.Shutdown(false);
					}
				}
				
				//UpdateUI();
			}
		}
		
		public void OnEnterRoom()
		{
			if (GateUI(_uiRoom))
			{
				FusionLauncher launcher = FindObjectOfType<FusionLauncher>();
				if (launcher == null)
					launcher = new GameObject("Launcher").AddComponent<FusionLauncher>();

				LevelManager lm = FindObjectOfType<LevelManager>();
				lm.launcher = launcher;

				launcher.Launch(_gameMode, _room.text, lm, OnConnectionStatusUpdate, OnSpawnWorld, OnSpawnPlayer, OnDespawnPlayer);
			}
		}

		/// <summary>
		/// Call this method from button events to close the current UI panel and check the return value to decide
		/// if it's ok to proceed with handling the button events. Prevents double-actions and makes sure UI panels are closed. 
		/// </summary>
		/// <param name="ui">Currently visible UI that should be closed</param>
		/// <returns>True if UI is in fact visible and action should proceed</returns>
		private bool GateUI(Panel ui)
		{
			if (!ui.isShowing)
				return false;
			ui.SetVisible(false);
			return true;
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