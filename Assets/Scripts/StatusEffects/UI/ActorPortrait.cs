using StatusEffectSystem.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
	namespace Portrait {
		public class ActorPortrait : MonoBehaviour, IPointerClickHandler {

			public void Initialize(Actor actor) {
				Actor = actor;
				HealthBar = transform.Find("HealthBar").GetComponent<HealthBar>();
				if (HealthBar == null) {
					throw new UnityException("Please verify the structure of the ActorPortrait: HealthBar is missing.");
				}
				HealthBar.Initialize(actor);

				FoodBar = transform.Find("FoodBar").GetComponent<FoodBar>();
				if (FoodBar == null) {
					throw new UnityException("Please verify the structure of the ActorPortrait: FoodBar is missing.");
				}
				FoodBar.Initialize(actor);

				StatusHandler = transform.Find("StatusesHandler").GetComponent<StatusHandler>();
				if (StatusHandler == null) {
					throw new UnityException("Please verify the structure of the ActorPortrait: StatusesHandler is missing.");
				}
				StatusHandler.Initialize(actor);

				MiniPortrait = transform.Find("Mask/MiniPortrait").GetComponent<RawImage>();
				if (MiniPortrait == null) {
					throw new UnityException("Please verify the structure of the ActorPortrait: Mask/MiniPortrait is missing.");
				}
				RenderTexture renderTexture = new RenderTexture(256, 256, 8, RenderTextureFormat.ARGB32);
				renderTexture.Create();
				MiniPortrait.texture = renderTexture;
				actor.MiniCamera.Initialize(renderTexture);

				Selection = transform.Find("Selection").gameObject;
				if (Selection == null) {
					throw new UnityException("Please verify the structure of the ActorPortrait: Selection is missing.");
				}
				Selection.SetActive(false);
				actor.OnSelectionEvent += EnableSelection;
			}

			public void OnDelete(Actor actor) {
				HealthBar.OnDelete(actor);
				FoodBar.OnDelete(actor);
				actor.OnSelectionEvent -= EnableSelection;
			}

			private void EnableSelection(bool value) {
				Selection.SetActive(value);
			}

			public void OnPointerClick(PointerEventData eventData) {
				SoundSystem.Instance.PlaySound("hover", 0.2f);

				Squad.SelectActor(Actor);
			}

			public Actor Actor { get; private set; }
			public HealthBar HealthBar { get; private set; }
			public StatusHandler StatusHandler { get; private set; }
			public FoodBar FoodBar { get; private set; }
			public GameObject Selection { get; private set; }
			public RawImage MiniPortrait { get; private set; }
		}
	}
}
