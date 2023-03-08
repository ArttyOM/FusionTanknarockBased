using System;
using System.Runtime.CompilerServices;
using StaticEvents;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static FusionExamples.FusionHelpers.NetworkRunnerCallbacksHandler;

namespace FusionExamples.Tanknarok
{
	public class CameraScreenFXBehaviour : MonoBehaviour
	{
		private readonly IObservable<ConnectionStatus> _onConnectionStatusChanged =
			MainSceneEvents.OnConnectionStatusChanged;

		private IDisposable _onConnectionStatusChangedSubscription;

		Kino.DigitalGlitch glitchEffect;
		[SerializeField] private float durationToTarget = 0.3f;
		float timer = 0;

		bool active = false;
		private float maxGlitch = 1;

		private void Awake()
		{
			GetComponent<PostProcessLayer>().enabled = !Application.isMobilePlatform;
			GetComponent<PostProcessVolume>().enabled = !Application.isMobilePlatform;
			
			_onConnectionStatusChangedSubscription = _onConnectionStatusChanged.Where(x => x== ConnectionStatus.Loading)
				.Subscribe(_=>ToggleGlitch(true));
		}

		private void OnDestroy()
		{
			_onConnectionStatusChangedSubscription.Dispose();
		}

		void Start()
		{
			glitchEffect = GetComponent<Kino.DigitalGlitch>();
			glitchEffect.enabled = false;
		}

		public void ToggleGlitch(bool value)
		{
			active = value;
		}

		// Update is called once per frame
		void Update()
		{
			float direction = active ? 1 : -1;
			if ((timer > 0 && direction == -1) || (timer < durationToTarget && direction == 1))
			{
				timer = Mathf.Clamp(timer + Time.deltaTime * direction, 0, durationToTarget);
				float t = timer / durationToTarget;
				glitchEffect.intensity = Mathf.Lerp(0, maxGlitch, t);

				if (timer == 0 && direction == -1 && glitchEffect.enabled)
				{
					glitchEffect.enabled = false;
				}
				else if (direction == 1 && !glitchEffect.enabled)
				{
					glitchEffect.enabled = true;
				}
			}
		}
	}
}