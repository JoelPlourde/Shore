using System.Collections.Generic;
using AbilitySystem;
using SkillSystem;
using UI.AbilitySystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Canvas))]
    public class AbilityHandler : MonoBehaviour, IMenu
    {
        public static AbilityHandler Instance;

        private Dictionary<SkillType, AbilityBannerComponent> _abilityBannerComponents;

        private AbilityComponent[] _abilityComponents;

        private SkillType _currentSkillType = SkillType.FIGHTING;

        private Actor _currentActor;

        private void Awake()
        {
            Instance = this;

            Canvas = GetComponent<Canvas>();

            // Underneath the "Content" GameObject, there are AbilityComponent
            _abilityComponents = GetComponentsInChildren<AbilityComponent>(true);

            _abilityBannerComponents = new Dictionary<SkillType, AbilityBannerComponent>();
            AbilityBannerComponent[] bannerComponents = transform.Find("Banners").GetComponentsInChildren<AbilityBannerComponent>(true);
            foreach (AbilityBannerComponent bannerComponent in bannerComponents)
            {
                _abilityBannerComponents[bannerComponent.skillType] = bannerComponent;
            }
        }

        public void Open(Actor actor)
        {
            _currentActor = actor;
            Debug.Log("Opening Abilities Menu for Actor: " + actor.name);

            _abilityBannerComponents[_currentSkillType].BringForward();

            SkillData skillData = SkillManager.Instance.GetSkillData(_currentSkillType);

            // For each abilities in the skill data, update the corresponding AbilityComponent
            for (int i = 0; i < skillData.Abilities.Length; i++)
            {
                AbilityData abilityData = skillData.Abilities[i];
                AbilityComponent abilityComponent = _abilityComponents[i];

                // Update the AbilityComponent with the AbilityData
                abilityComponent.UpdateAbility(abilityData);

                _abilityComponents[i].gameObject.SetActive(true);
            }

            // If there are more AbilityComponents than Abilities, disable the extra components
            for (int i = skillData.Abilities.Length; i < _abilityComponents.Length; i++)
            {
                _abilityComponents[i].gameObject.SetActive(false);
            }

            Canvas.enabled = true;
        }

        public void SwitchSkill(SkillType skillType)
        {
            Debug.Log("Switching skill to: " + skillType);

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
            Canvas.enabled = false;
        }

        public Canvas Canvas { get; set; }
    }
}