using System;
using StaticEvents;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using Unit = UniRx.Unit;

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

        [SerializeField] private KeyCode _hotKey = KeyCode.None;
        
        
        private IObserver<AbilityType> _onButtonClick = AbilitiesEvents.ActivatedBroadcaster;

        private Subject<Unit> _abilityActivated = new Subject<Unit>();
        public IObservable<Unit> AbilityActivated => _abilityActivated;

        private Button _button;

        private float _currentCooldown = 0;
        private IDisposable _cooldownSubscription;

        private IDisposable _hotkeySubscription;
        
        public AbilityType GetAbilityType=>abilityType;
        

        private void Awake()
        {
            Init();

            SetHotkey(_hotKey);


        }

        private void OnDestroy()
        {
            _cooldownSubscription?.Dispose();
            _hotkeySubscription?.Dispose();
        }

        // тупо для теста
        // private void Update()
        // {
        //     if (Input.GetKeyUp(KeyCode.Space)) SetCooldown(6);
        // }
        
        public void Init()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnAbilityActivated);
            
            if (cooldownText != null) cooldownText.enabled = false;
        }

        public void SetHotkey(KeyCode hotkey)
        {
            _hotkeySubscription?.Dispose();
            if (hotkey!= KeyCode.None)
            {
                _hotkeySubscription = Observable.EveryUpdate().Where(x => Input.GetKeyDown(hotkey))
                    .Subscribe(_ => _button.onClick.Invoke());

               
            }
            SetHotkeyHint(hotkey);
        }

        private void OnAbilityActivated()
        {
            _onButtonClick.OnNext(abilityType);
            _abilityActivated.OnNext(new Unit());
        }
        
        private void SetHotkeyHint(KeyCode newHotkey)
        {
            if (hotkeyHint != null)
            {
                switch (newHotkey)
                {
                    case KeyCode.LeftShift:
                        hotkeyHint.text = "Shift";
                        break;
                    case KeyCode.None :
                        hotkeyHint.text = "";
                        break;
                    default: 
                        hotkeyHint.text = newHotkey.ToString();
                        break;
                }
            }
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