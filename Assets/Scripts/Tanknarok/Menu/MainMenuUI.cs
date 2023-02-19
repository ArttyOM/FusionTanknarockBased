using System;
using Fusion;
using FusionExamples.FusionHelpers;
using FusionExamples.Tanknarok;
using FusionExamples.UIHelpers;
using StaticEvents;
using TMPro;
using UniRx;
using UnityEngine;

namespace Tanknarok
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _progress;
        
        [SerializeField] private Panel _uiCurtain;
        [SerializeField] private Panel _uiStart;
        [SerializeField] private Panel _uiProgress;
        [SerializeField] private Panel _uiRoom;
        [SerializeField] private GameObject _uiGame;
        
        FusionLauncher.ConnectionStatus _status;
        
        private readonly IObservable<FusionLauncher.ConnectionStatus> _connectionStatusChangedEvent =
            MainSceneEvents.OnConnectionStatusChanged;

        /// <summary>
        /// Методом OnNext() оповещает всех слушателей о смене GameMode
        /// </summary>
        private readonly IObserver<GameMode> _gameModeChangedBroadcaster = MainSceneEvents.GameModeChangedBroadcaster;
        private IDisposable _connectionStatusChangedSubscription;
        
        public void OnHostOptions()
        {
            BroadcastModeChanged(GameMode.Host);
            CloseStartThenShowUiRoom();
        }

        public void OnJoinOptions()
        {
            BroadcastModeChanged(GameMode.Client);
            CloseStartThenShowUiRoom();
        }

        public void OnSharedOptions()
        {
            BroadcastModeChanged(GameMode.Shared);
            CloseStartThenShowUiRoom();
        }

        private void Awake()
        {
            _connectionStatusChangedEvent.Subscribe(newStatus =>
            {
                _status = newStatus;
                UpdateUI();
            });
            Observable.EveryUpdate().Where(_ => _uiProgress.isShowing)
                .Subscribe(_ => UpdateUI());
        }

        private void OnDestroy()
        {
            _connectionStatusChangedSubscription?.Dispose();
        }

        /// <summary>
        /// Когда пользователь меняет GameMode, оповещаем потребителей
        /// </summary>
        /// <param name="gamemode"></param>
        private void BroadcastModeChanged(GameMode gamemode)
        {
            _gameModeChangedBroadcaster.OnNext(gamemode);
        }
        
        private void CloseStartThenShowUiRoom()
        {
            Close(_uiStart, out bool wasOpened);
            if (wasOpened)
            {
                _uiRoom.SetVisible(true);
            }
        }
        
        /// <summary>
        /// Закрывает UI-экран
        /// </summary>
        /// <param name="ui">UI экран, который требуется закрыть</param>
        /// <param name="wasOpened">false, в случае, если UI-экран был и так выключен </param>
        private void Close(Panel ui, out bool wasOpened)
        {
            if (ui.isShowing)
            {
                ui.SetVisible(false);
                wasOpened = true;
            }
            else
            {
                wasOpened = false;
            }
        }
        
        
        private void UpdateUI()
        {
            bool intro = false;
            bool progress = false;
            bool running = false;

           
            switch (_status)
            {
                case FusionLauncher.ConnectionStatus.Disconnected:
                    _progress.text = "Disconnected!";
                    intro = true;
                    break;
                case FusionLauncher.ConnectionStatus.Failed:
                    _progress.text = "Failed!";
                    intro = true;
                    break;
                case FusionLauncher.ConnectionStatus.Connecting:
                    _progress.text = "Connecting";
                    progress = true;
                    break;
                case FusionLauncher.ConnectionStatus.Connected:
                    _progress.text = "Connected";
                    progress = true;
                    break;
                case FusionLauncher.ConnectionStatus.Loading:
                    _progress.text = "Loading";
                    progress = true;
                    break;
                case FusionLauncher.ConnectionStatus.Loaded:
                    running = true;
                    break;
            }

            _uiCurtain.SetVisible(!running);
            _uiStart.SetVisible(intro);
            _uiProgress.SetVisible(progress);
            _uiGame.SetActive(running);
			
            if(intro)
                MusicPlayer.instance.SetLowPassTranstionDirection( -1f);
        }
    }
}

