using System;
using System.Collections.Generic;
using Fusion;
using FusionExamples.FusionHelpers;
using FusionExamples.Tanknarok;
using StaticEvents;
using Tanknarok.Menu;
using UniRx;
using Unity.VisualScripting;
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
		[SerializeField] private NetworkRunnerCallbacksHandler _networkRoomOpenerPrefab;
		
		private MainMenuUI _mainMenuUI;

		private NetworkLobbyBuilder _networkLobbyBuilder;
		private NetworkSceneLoader _networkSceneLoader;

		private NetworkRunner _networkRunner;
		private ScoreManager _scoreManager;
		private CountdownManager _countdownManager;
		private ReadyupManager _readyupManager;


		private void Awake()
		{
			DontDestroyOnLoad(this);
			
			_mainMenuUI = FindObjectOfType<MainMenuUI>();
			DontDestroyOnLoad(_mainMenuUI);
			
			 _networkLobbyBuilder = new NetworkLobbyBuilder(_networkRoomOpenerPrefab);
			 
			 _scoreManager = FindObjectOfType<ScoreManager>();
			 _scoreManager.Init();
			 
			 _countdownManager = FindObjectOfType<CountdownManager>(true);
			 _countdownManager.Init();
			 
			 _readyupManager = FindObjectOfType<ReadyupManager>(true);
			 _readyupManager.Init();
			 
			 _networkSceneLoader = FindObjectOfType<NetworkSceneLoader>();
			 _networkSceneLoader.Init();
		}

		private void Start()
		{
			_networkLobbyBuilder.Init();
		}
		
		
	}
}