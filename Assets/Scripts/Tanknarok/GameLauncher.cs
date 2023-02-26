using System;
using System.Collections.Generic;
using Fusion;
using FusionExamples.FusionHelpers;
using FusionExamples.Tanknarok;
using StaticEvents;
using Tanknarok.Menu;
using UniRx;
using UnityEngine;

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
		[SerializeField] private FusionLauncher _launcherPrefab;
		
		private MainMenuUI _mainMenuUI;
		
		private NetworkRoomOpener _networkRoomOpener;
		
		private void Awake()
		{
			DontDestroyOnLoad(this);
			
			_mainMenuUI = FindObjectOfType<MainMenuUI>();
			DontDestroyOnLoad(_mainMenuUI);
			
			 _networkRoomOpener = new NetworkRoomOpener(_launcherPrefab);
		}

		private void Start()
		{
			_networkRoomOpener.Init();
		}
		
		
	}
}