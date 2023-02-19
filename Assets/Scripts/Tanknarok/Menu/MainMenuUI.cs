using Fusion;
using FusionExamples.UIHelpers;
using StaticEvents;
using UniRx;
using UnityEngine;

namespace Tanknarok
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Panel _uiCurtain;
        [SerializeField] private Panel _uiStart;
        [SerializeField] private Panel _uiProgress;
        [SerializeField] private Panel _uiRoom;
        
        private Subject<GameMode> _gameModeEvent = MainSceneEvents.gameModeEvent;
        
        public void OnHostOptions()
        {
            BroadcastModeChanged(GameMode.Host);
            ShowUiRoom();
        }

        public void OnJoinOptions()
        {
            BroadcastModeChanged(GameMode.Client);
            ShowUiRoom();
        }

        public void OnSharedOptions()
        {
            BroadcastModeChanged(GameMode.Shared);
            ShowUiRoom();
        }

        /// <summary>
        /// Когда пользователь меняет GameMode, оповещаем потребителей
        /// </summary>
        /// <param name="gamemode"></param>
        private void BroadcastModeChanged(GameMode gamemode)
        {
            _gameModeEvent.OnNext(gamemode);
        }
        
        private void ShowUiRoom()
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
    }
}

