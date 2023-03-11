using UnityEngine;
using UnityEngine.SceneManagement;

namespace FusionExamples.Tanknarok
{
	// Toggle when the lobby info text should show
	// Ready text shows if there are more than one tank in the lobby
	// Shutdown text shows when in the lobby
	public class InfoButtons : MonoBehaviour
	{
		[SerializeField] private GameObject _disconnectInfoText;
		[SerializeField] private GameObject _readyupInfoText;

		private float _delay;

		private void Update()
		{
			_delay -= Time.deltaTime;
			if (_delay<0)
			{
				_delay = 1.0f;

				int readyCount = 0;
				foreach (Player player in PlayerManager.allPlayers)
				{
					if (player.ready)
						readyCount++;
				}

				_disconnectInfoText.SetActive(readyCount<PlayerManager.allPlayers.Count);
				bool showReadyHint = false;
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					if (SceneManager.GetSceneAt(i).name.Equals("Lobby"))
					{
						showReadyHint = (readyCount < PlayerManager.allPlayers.Count);
						break;
					}
				}

				_readyupInfoText.SetActive(showReadyHint);
			}
		}
	}
}