using System.Collections.Generic;
using AbilitySystem;
using SkillSystem;
using TMPro;
using UI.AbilitySystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Canvas))]
    public class AbilityBookHandler : MonoBehaviour, IMenu
    {
        public static AbilityBookHandler Instance;

        private Dictionary<SkillType, AbilityBannerComponent> _abilityBannerComponents;

        private AbilityEntry[] _abilityEntries;

        private SkillType _currentSkillType = SkillType.FIGHTING;

        private Actor _currentActor;

        private Button _exit;

        private void Awake()
        {
            Instance = this;

            Canvas = GetComponent<Canvas>();

            // Underneath the "Content" GameObject, there are AbilityComponent
            _abilityEntries = GetComponentsInChildren<AbilityEntry>(true);

            _abilityBannerComponents = new Dictionary<SkillType, AbilityBannerComponent>();
            AbilityBannerComponent[] bannerComponents = transform.Find("Banners").GetComponentsInChildren<AbilityBannerComponent>(true);
            foreach (AbilityBannerComponent bannerComponent in bannerComponents)
            {
                _abilityBannerComponents[bannerComponent.skillType] = bannerComponent;
            }

			_exit = transform.Find("Exit").GetComponent<Button>();
            _exit.onClick.AddListener(Close);
        }

        private void OnDestroy() {
            if (!ReferenceEquals(_exit, null)) {
                _exit.onClick.RemoveAllListeners();
            }
        }

        public void Open(Actor actor)
        {
            _currentActor = actor;

            _abilityBannerComponents[_currentSkillType].BringForward();

            SkillData skillData = SkillManager.Instance.GetSkillData(_currentSkillType);

            // For each abilities in the skill data, update the corresponding AbilityComponent
            for (int i = 0; i < skillData.Abilities.Length; i++)
            {
                AbilityData abilityData = skillData.Abilities[i];
                _abilityEntries[i].gameObject.SetActive(true);
                _abilityEntries[i].Initialize(actor, abilityData);
            }

            // If there are more AbilityComponents than Abilities, disable the extra components
            for (int i = skillData.Abilities.Length; i < _abilityEntries.Length; i++)
            {
                _abilityEntries[i].gameObject.SetActive(false);
            }

            Canvas.enabled = true;
        }

        public void SwitchSkill(SkillType skillType)
        {
            if (_currentSkillType == skillType)
            {
                return;
            }
            
            _abilityBannerComponents[_currentSkillType].SendBackward();

            _currentSkillType = skillType;

            Open(_currentActor);
        }

        public void Close(Actor actor)
        {
            Close();
        }

        public void Close() {
            // Reset the current skill banner position
            _abilityBannerComponents[_currentSkillType].SendBackward();
            
			Canvas.enabled = false;
		}

        public Canvas Canvas { get; set; }
    }
}