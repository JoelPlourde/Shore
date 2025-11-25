using AbilitySystem;
using SkillSystem;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    namespace AbilitySystem
    {
        public class AbilityComponent: MonoBehaviour
        {
            [SerializeField]
            private AbilityData _abilityData;

            private Image _background;
            private Image _icon;

            private void Awake()
            {
                _background = transform.Find("Background").GetComponent<Image>();
                _icon = _background.transform.Find("Icon").GetComponent<Image>();
            }

            public void Initialize(AbilityData abilityData)
            {
                if (ReferenceEquals(_background, null))
                {
                    _background = transform.Find("Background").GetComponent<Image>();
                }

                if (ReferenceEquals(_icon, null))
                {
                    _icon = _background.transform.Find("Icon").GetComponent<Image>();
                }

                SkillData skillData = SkillManager.Instance.GetSkillData(abilityData.SkillType);

                _abilityData = abilityData;
                _background.sprite = _abilityData.Background;
                _background.color = new Color(skillData.Color.r, skillData.Color.g, skillData.Color.b, 1f);
                _icon.sprite = _abilityData.Sprite;
            }

            public AbilityData AbilityData
            {
                get { return _abilityData; }
            }
        }
    }
}