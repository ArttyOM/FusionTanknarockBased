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
		private MainMenuUI _mainMenuUI;

		private FusionLauncher.ConnectionStatus _status; 
		private GameMode _gameMode;

		private string _roomName;
		
		private void Awake()
		{
			DontDestroyOnLoad(this);
			
			_mainMenuUI = FindObjectOfType<MainMenuUI>();
			DontDestroyOnLoad(_mainMenuUI);
			
		}
	}
}