using System.Collections.Generic;
using UnityEngine;
using TaskSystem;
using UnityEngine.SceneManagement;
using CameraSystem;

public static class Squad {

	private static readonly List<Unit> _units = new List<Unit>();

	private static readonly GameObject _selectorTemplate;

	static Squad() {
		UserInputs.Instance.Subscribe("Terrain", MoveSquad, true);
		UserInputs.Instance.Subscribe("Interactable", Interact, true);

		_selectorTemplate = Resources.Load<GameObject>("Prefabs/Selector");
		if (_selectorTemplate == null) {
			throw new UnityException("Please define a projector gameobject at: Assets/Resources/Prefabs/Selector");
		}
	}

	/// <summary>
	/// Add an actor from the squad.
	/// </summary>
	/// <param name="actor">An actor.</param>
	public static void AddToSquad(Actor actor) {
		int index = _units.FindIndex(x => x.Actor.Guid == actor.Guid);
		if (index == -1) {
			Object.DontDestroyOnLoad(actor);
			_units.Add(new Unit(actor, CreateSelector(actor)));
		} else {
			throw new UnityException("This actor has already been added to the Squad, something is wrong.");
		}

		PortraitManager.InstantiateActorPortrait(actor);
		UserInputs.Instance.Subscribe(actor.Guid.ToString(), SelectActor, true);
	}

	/// <summary>
	/// Remove an actor from the squad.
	/// </summary>
	/// <param name="actor">An actor.</param>
	public static void RemoveFromSquad(Actor actor) {
		SceneManager.MoveGameObjectToScene(actor.gameObject, SceneManager.GetActiveScene());
		_units.RemoveAll(x => x.Actor.Guid == actor.Guid);
		DeleteSelector(actor);
		PortraitManager.DeleteActorPortrait(actor);
		UserInputs.Instance.Unsubscribe(actor.Guid.ToString(), SelectActor);
	}

	/// <summary>
	/// Return any Actor in the squad.
	/// </summary>
	/// <param name="actor">The Actor will be initialized only if any actor exists in the squad.</param>
	/// <returns>Whether or not the operation was successful</returns>
	public static bool Any(out Actor actor) {
		actor = null;
		if (_units.Count > 0) {
			actor = _units[0].Actor;
			_units[0].EnableSelector(true);
			HasSelected = true;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Return the first selected actor in the squad.
	/// </summary>
	/// <param name="actor">The Actor will be initialized as out, if any actor is in the squad.</param>
	/// <returns>Whether or not the operation was successful</returns>
	public static bool FirstSelected(out Actor actor) {
		actor = null;
		if (_units.Count > 0) {
			Unit unit = _units.Find(x => x.Actor.Selected == true);
			if (ReferenceEquals(unit, null)) {
				unit = _units[0];
				unit.EnableSelector(true);
			}
			actor = unit.Actor;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Teleport the Squad to a new position.
	/// </summary>
	/// <param name="position">The new position</param>
	public static void TeleportSquad(Vector3 position) {
		foreach (Unit unit in _units) {
			unit.Actor.NavMeshAgent.Warp(position);
		}
	}

	public static void TeleportSquadToSpawnPoint() {
		Transform spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
		TeleportSquad(spawnPoint.position);
	}

	/// <summary>
	/// Select the following actor only.
	/// </summary>
	/// <param name="actor">The actor to select.<param>
	public static void SelectActor(Actor actor) {
		_units.ForEach(x => {
			x.EnableSelector(false);
		});

		Unit selectedUnit = _units.Find(x => x.Actor.Guid == actor.Guid);
		selectedUnit.EnableSelector(true);
		HasSelected = true;

		CameraSystem.CameraController.Instance.FollowTarget(actor.transform);
	}

	/// <summary>
	/// Unselect all the actors
	/// </summary>
	public static void UnselectAll() {
		_units.ForEach(x => {
			x.EnableSelector(false);
		});
		HasSelected = false;

		CameraSystem.CameraController.Instance.StopFollow();
	}

	/// <summary>
	/// Move the selected actors to the position.
	/// </summary>
	/// <param name="mouseButton">Which mouse button it is used.</param>
	/// <param name="raycastHit">The RaycastHit information</param>
	private static void MoveSquad(MouseButton mouseButton, RaycastHit raycastHit) {
		if (mouseButton == MouseButton.LEFT_MOUSE_BUTTON) {
			MoveArguments moveArguments = new MoveArguments(raycastHit.point);
			_units.ForEach(x => {
				if (x.Actor.Selected) {
					x.Actor.TaskScheduler.CreateTask<Move>(moveArguments);
				}
			});

			if (HasSelected) {
				// Create an indicator at the position.
				ParticleSystemManager.Instance.SpawnParticleSystem("vfx_Indicator", moveArguments.Position);
			}
		}
	}

	/// <summary>
	/// Create the Interact task for the selected actors.
	/// </summary>
	/// <param name="mouseButton">Which button it is used.</param>
	/// <param name="raycastHit">The RaycastHit information</param>
	private static void Interact(MouseButton mouseButton, RaycastHit raycastHit) {
		if (mouseButton == MouseButton.LEFT_MOUSE_BUTTON) {
			IInteractable interactable = raycastHit.collider.gameObject.GetComponent<IInteractable>();

			_units.ForEach(x => {
				if (x.Actor.Selected) {
					// Calculate the Vector from the Actor to the Interactable.
					Vector3 direction = raycastHit.collider.bounds.center - x.Actor.transform.position;

					InteractArguments interactArguments = new InteractArguments(raycastHit.collider.transform, interactable);

					x.Actor.TaskScheduler.CreateTask<Interact>(interactArguments);
				}
			});
		}
	}

	/// <summary>
	/// Event triggered when the user clicks on this actor.
	/// </summary>
	/// <param name="mouseButton">Which Mouse button it used.</param>
	/// <param name="raycastHit">The RaycastHit information</param>
	private static void SelectActor(MouseButton mouseButton, RaycastHit raycastHit) {
		if (mouseButton == MouseButton.LEFT_MOUSE_BUTTON) {
			if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) {
				_units.ForEach(x => {
					x.EnableSelector(false);
				});

			}
			Unit selectedUnit = _units.Find(x => x.Actor.Guid.ToString() == raycastHit.collider.name);
			selectedUnit.EnableSelector(true);
			HasSelected = true;
		}
	}

	/// <summary>
	/// Select the actors with the given actor IDs.
	/// </summary>
	/// <param name="actorIds"></param>
	public static void SelectActors(List<string> actorIds) {
		_units.ForEach(x => {
			x.EnableSelector(false);
		});

		HasSelected = false;

		for (int i = 0; i < actorIds.Count; i++) {
			Unit selectedUnit = _units.Find(y => y.Actor.Guid.ToString() == actorIds[i]);
			selectedUnit.EnableSelector(true);

			if (i == 0) {
				CameraController.Instance.FollowTarget(selectedUnit.Actor.transform);
			}
		}

		HasSelected = true;
	}

	/// <summary>
	/// Get the selected actor IDs.
	/// </summary>
	/// <returns></returns>
	public static List<string> GetSelectedActorIds() {
		List<string> ids = new List<string>();
		_units.ForEach(x => {
			if (x.Actor.Selected) {
				ids.Add(x.Actor.Guid.ToString());
			}
		});
		return ids;
	}

	public static bool HasSelected { get; private set; }

	#region Selector
	private static GameObject CreateSelector(Actor actor) {
		GameObject selectorObj = GameObject.Instantiate(_selectorTemplate);
		selectorObj.SetActive(false);
		selectorObj.name = "Selector";
		selectorObj.transform.SetParent(actor.transform);
		selectorObj.transform.localPosition = new Vector3(0, 5, 0);
		return selectorObj;
	}

	private static void DeleteSelector(Actor actor) {
		UnityEngine.Object.Destroy(actor.transform.Find("Selector").gameObject);
	}
	#endregion

	public class Unit {

		public Actor Actor { get; private set; }
		public GameObject Selector { get; private set; }

		public Unit(Actor actor, GameObject selector) {
			Actor = actor;
			Selector = selector;
		}

		public void EnableSelector(bool value) {
			Selector.SetActive(value);
			Actor.SetSelected(value);
		}
	}
}
