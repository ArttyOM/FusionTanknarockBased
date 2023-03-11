using System;
using StaticEvents;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

namespace Abilities
{
    /// <summary>
    /// Визуальное отображение способности на экране:
    /// иконка, кулдаун, подсказки к хоткеям и
    /// управление кнопкой способности
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class AbilityView: MonoBehaviour
    {
        [SerializeField] private AbilityType abilityType;
        
        [SerializeField] private Image cooldownVisualiser;
        [SerializeField] private TextMeshProUGUI cooldownText;
        
        [SerializeField] private TextMeshProUGUI hotkeyHint;
        [SerializeField] private TextMeshProUGUI abilityName;
        
        
        private IObserver<AbilityType> _onButtonClick = AbilitiesEvents.ActivatedBroadcaster;

        private Button _button;

        private float _currentCooldown = 0;
        private IDisposable _cooldownSubscription;
        
        public AbilityType AbilityType=>abilityType;
        

        private void Awake()
        {
            Init();
        }

        // тупо для теста
        // private void Update()
        // {
        //     if (Input.GetKeyUp(KeyCode.Space)) SetCooldown(6);
        // }
        
        public void Init()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => _onButtonClick.OnNext(abilityType));
            
            if (cooldownText != null) cooldownText.enabled = false;
        }

        public void SetHotkeyHint(string newHotkeyHint)
        {
            if (hotkeyHint != null) hotkeyHint.text = newHotkeyHint;
        }
        
        /// <summary>
        /// в секундах
        /// </summary>
        /// <param name="cooldown"></param>
        public void SetCooldown(float cooldown)
        {
            _currentCooldown = cooldown;
            _cooldownSubscription?.Dispose();
            _cooldownSubscription = Observable.EveryUpdate()
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds((double) cooldown)))
                .Select(x =>
                {
                    _currentCooldown -= Time.deltaTime;
                    return _currentCooldown;
                })
                .Subscribe(x=>ChangeCooldownTextTo(x, cooldown),
                    onCompleted:HideCooldown);
        }

        private void HideCooldown()
        {
            if (cooldownText != null) cooldownText.enabled = false;
            if (cooldownVisualiser != null) cooldownVisualiser.fillAmount = 1;
        }

        private void ChangeCooldownTextTo(float currentCooldown, float maxCooldown)
        {
            if (cooldownText != null)
            {
                cooldownText.text = currentCooldown.ToString("0.0");
                cooldownText.enabled = true;
            }
            if (cooldownVisualiser != null) cooldownVisualiser.fillAmount = currentCooldown / maxCooldown;
        }
    }
}