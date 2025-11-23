using SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    namespace AbilitySystem
    {
        /// <summary>
        /// Component responsible for animating the multiple ability banner UI element.
        /// </summary>
        public class AbilityBannerComponent : MonoBehaviour
        {
            [Tooltip("The skill type this banner represents.")]
            public SkillType skillType;

            private float _anchoredPositionY;

            private RectTransform _rectTransform;

            private void Start()
            {
                _rectTransform = GetComponent<RectTransform>();

                SkillData skillData = SkillManager.Instance.GetSkillData(skillType);

                Image banner = GetComponent<Image>();
                banner.color = new Color(skillData.Color.r, skillData.Color.g, skillData.Color.b, 1.0f);

                Image icon = transform.Find("Icon").GetComponent<Image>();
                icon.sprite = skillData.Icon;

                // Get the Button and add a listener to it
                GetComponent<Button>().onClick.AddListener(() =>
                {
                    AbilityHandler.Instance.SwitchSkill(skillType);
                });
            }

            private void OnDestroy() {
                // Remove all listeners from the Button
                GetComponent<Button>().onClick.RemoveAllListeners();
            }

            /// <summary>
            /// Brings this banner forward with an animation.
            /// </summary>
            public void BringForward()
            {
                _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x, _rectTransform.localPosition.y - 20f, _rectTransform.localPosition.z);
            }

            /// <summary>
            /// Sends this banner backward with an animation.
            /// </summary>
            public void SendBackward()
            {
                _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x, _rectTransform.localPosition.y + 20f, _rectTransform.localPosition.z);
            }
        }
    }
}