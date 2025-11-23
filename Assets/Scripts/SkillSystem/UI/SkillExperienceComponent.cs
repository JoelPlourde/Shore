using UnityEngine;
using UnityEngine.UI;

namespace SkillSystem {
	namespace UI
    {
        [RequireComponent(typeof(Slider))]
        public class SkillExperienceComponent : MonoBehaviour
        {
            private readonly float _skillTransparency = 0.25f;

            private Slider _slider;

            private Image _fillImage;
            private Image _fillArea;

            private void Awake()
            {
                _slider = GetComponent<Slider>();

                // This image will be the color of the skill
                _fillImage = transform.Find("InnerBorder/CenterArea").GetComponent<Image>();
                _fillArea = transform.Find("FillArea/Fill").GetComponent<Image>();
            }

            public void OnUpdateExperience(float value)
            {
                _slider.value = value;
            }

            public void Initialize(SkillData skillData, Level level)
            {
                _fillImage.color = new Color(skillData.Color.r, skillData.Color.g, skillData.Color.b, _skillTransparency);
                _fillArea.color = new Color(skillData.Color.r, skillData.Color.g, skillData.Color.b, 1.0f);

                _slider.maxValue = ExperienceTable.GetExperienceRequiredAt(level.Value + 1);
                _slider.value = level.Experience;
            }
        }
    }
}