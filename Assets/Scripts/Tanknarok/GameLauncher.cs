using System;
using System.Collections.Generic;
using Fusion;
using FusionExamples.FusionHelpers;
using FusionExamples.Tanknarok;
using StaticEvents;
using Tanknarok.Menu;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tanknarok
{
	/// <summary>
	/// точка входа
	/// app entry point
	/// main()
	/// bootstrap
	/// </summary>
	public class GameLauncher : MonoBehaviour
	{
		[FormerlySerializedAs("_launcherPrefab")] [SerializeField] private NetworkRunnerCallbacksHandler _networkRoomOpenerPrefab;
		
		private MainMenuUI _mainMenuUI;
		
		
		private NetworkRoomOpener _networkRoomOpener;

		private NetworkRunner _networkRunner;

		private void Awake()
		{
			DontDestroyOnLoad(this);
			
			_mainMenuUI = FindObjectOfType<MainMenuUI>();
			DontDestroyOnLoad(_mainMenuUI);
			
			 _networkRoomOpener = new NetworkRoomOpener(_networkRoomOpenerPrefab);
		}

		private void Start()
		{
			_networkRoomOpener.Init();
		}
		
		
	}
}