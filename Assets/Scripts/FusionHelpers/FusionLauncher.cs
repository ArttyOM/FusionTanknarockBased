using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using FusionExamples.Tanknarok;
using StaticEvents;
using Tanknarok.UI;
using UnityEngine;

namespace FusionExamples.FusionHelpers
{
    /// <summary>
    ///     Small helper that provides a simple world/player pattern for launching Fusion
    /// </summary>
    public class FusionLauncher : MonoBehaviour, INetworkRunnerCallbacks
    {
        public enum ConnectionStatus
        {
            Disconnected,
            Connecting,
            Failed,
            Connected,
            Loading,
            Loaded
        }

        [SerializeField] private GameManager _gameManagerPrefab;
        [SerializeField] private Player _playerPrefab;

        #region Вещатели событий

        private readonly IObserver<ConnectionStatus> _connectionStatusChangedBroadcaster =
            MainSceneEvents.OnConnectionStatusBroadcaster;

        #endregion

        private bool _isWorldSpawned;
        private FusionObjectPoolRoot _pool;

        private NetworkRunner _runner;
        private ConnectionStatus _status;

        private void Awake()
        {
        }

public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("Connected to server");
            if (runner.GameMode == GameMode.Shared)
            {
                Debug.Log("Shared Mode - Spawning Player");
                InstantiatePlayer(runner, runner.LocalPlayer);
            }

            SetConnectionStatus(ConnectionStatus.Connected, "");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log("Disconnected from server");
            SetConnectionStatus(ConnectionStatus.Disconnected, "");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
            request.Accept();
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log($"Connect failed {reason}");
            SetConnectionStatus(ConnectionStatus.Failed, reason.ToString());
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log("Hosted Mode - Spawning Player");
                InstantiatePlayer(runner, player);
            }
//			SetConnectionStatus(ConnectionStatus.Connected, "");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("Player Left");
            DespawnPlayer(runner, player);

            SetConnectionStatus(_status, "Player Left");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("OnShutdown");
            var message = "";
            switch (shutdownReason)
            {
                case GameManager.ShutdownReason_GameAlreadyRunning:
                    message = "Game in this room already started!";
                    break;
                case ShutdownReason.IncompatibleConfiguration:
                    message = "This room already exist in a different game mode!";
                    break;
                case ShutdownReason.Ok:
                    message = "User terminated network session!";
                    break;
                case ShutdownReason.Error:
                    message = "Unknown network error!";
                    break;
                case ShutdownReason.ServerInRoom:
                    message = "There is already a server/host in this room";
                    break;
                case ShutdownReason.DisconnectedByPluginLogic:
                    message = "The Photon server plugin terminated the network session!";
                    break;
                default:
                    message = shutdownReason.ToString();
                    break;
            }

            SetConnectionStatus(ConnectionStatus.Disconnected, message);

            // TODO: This cleanup should be handled by the ClearPools call below, but currently Fusion is not returning pooled objects on shutdown, so...
            // Destroy all NOs
            var nos = FindObjectsOfType<NetworkObject>();
            for (var i = 0; i < nos.Length; i++)
                Destroy(nos[i].gameObject);

            // Clear all the player registries
            // TODO: This does not belong in here
            //PlayerManager.ResetPlayerManager();

            // Reset the object pools
            _pool.ClearPools();

            if (_runner != null && _runner.gameObject)
                Destroy(_runner.gameObject);
        }

        public async void Launch(GameMode mode, string room,
            INetworkSceneManager sceneLoader)
        {
            _isWorldSpawned = false;

            SetConnectionStatus(ConnectionStatus.Connecting, "");

            DontDestroyOnLoad(gameObject);

            if (_runner == null)
                _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.name = name;
            _runner.ProvideInput = mode != GameMode.Server;

            if (_pool == null)
                _pool = gameObject.AddComponent<FusionObjectPoolRoot>();

            await _runner.StartGame(new StartGameArgs
            {
                GameMode = mode,
                SessionName = room,
                ObjectPool = _pool,
                SceneManager = sceneLoader
            });
        }

        public void SetConnectionStatus(ConnectionStatus status, string message)
        {
            if (!this)
                return;

            Debug.Log(status);

            if (status != _status)
                switch (status)
                {
                    case ConnectionStatus.Disconnected:
                        ErrorBox.Show("Disconnected!", message, () => { });
                        break;
                    case ConnectionStatus.Failed:
                        ErrorBox.Show("Error!", message, () => { });
                        break;
                }

            _status = status;
            _connectionStatusChangedBroadcaster.OnNext(status);
        }

        private void SpawnWorld(NetworkRunner runner)
        {
            Debug.Log("Spawning GameManager");
            runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity, null, InitNetworkState);

            void InitNetworkState(NetworkRunner runner, NetworkObject world)
            {
                world.transform.parent = transform;
            }
        }

        private void SpawnPlayer(NetworkRunner runner, PlayerRef playerref)
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
                var player = networkObject.gameObject.GetComponent<Player>();
                Debug.Log($"Initializing player {player.playerID}");
                player.InitNetworkState(GameManager.MAX_LIVES);
            }
        }

        private void DespawnPlayer(NetworkRunner runner, PlayerRef playerref)
        {
            Debug.Log($"Despawning Player {playerref}");
            var player = PlayerManager.Get(playerref);
            player.TriggerDespawn();
        }

        private void InstantiatePlayer(NetworkRunner runner, PlayerRef playerref)
        {
            if (!_isWorldSpawned && (runner.IsServer || runner.IsSharedModeMasterClient))
            {
                SpawnWorld(runner);
                _isWorldSpawned = true;
            }

            SpawnPlayer(runner, playerref);
        }

        public void Init()
        {
            SetConnectionStatus(ConnectionStatus.Disconnected, "");
        }
    }
}