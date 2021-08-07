﻿using PointerSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MouseButton {
	LEFT_MOUSE_BUTTON, RIGHT_MOUSE_BUTTON, MIDDLE_MOUSE_BUTTON
}

public class UserInputs : MonoBehaviour, IUpdatable {
	public static UserInputs Instance;

	public LayerMask LayerMask;

	private Dictionary<string, InputAction> _actions = new Dictionary<string, InputAction>();
	private RaycastHit _hit;
	private InputAction _value;

	private void Awake() {
		if (ReferenceEquals(Instance, null)) {
			Instance = this;
		} else {
			throw new UnityException("A single entity of UserInputs can exists at any time.");
		}
	}

	private void Start() {
		GameController.Instance.RegisterUpdatable(this);
	}

	public void OnUpdate() {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonUp(0)) {
				PointerManager.Instance.SetPointer(PointerMode.DEFAULT);
				OnMouseEvent(MouseButton.LEFT_MOUSE_BUTTON);
			} else if (Input.GetMouseButtonUp(1)) {
				OnMouseEvent(MouseButton.RIGHT_MOUSE_BUTTON);
			} else if (Input.GetMouseButton(0)) {
				PointerManager.Instance.SetPointer(PointerMode.DEFAULT_PRESSED);
			}
		} else {
			if (Input.GetMouseButtonUp(0)) {
				PointerManager.Instance.SetPointer(PointerMode.DEFAULT);
			} else if (Input.GetMouseButton(0)) {
				PointerManager.Instance.SetPointer(PointerMode.DEFAULT_PRESSED);
			}
		}

		if (Input.GetKeyUp(KeyCode.Escape)) {
			Squad.UnselectAll();
		}
	}

	private void OnMouseEvent(MouseButton mouseButton) {
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit, Mathf.Infinity, LayerMask)) {
			if (_actions.TryGetValue(_hit.collider.name, out _value) || _actions.TryGetValue(_hit.collider.tag, out _value)) {
				_value.Action?.Invoke(mouseButton, _hit);
			}
		}
	}

	/// <summary>
	/// Subscribe to a topic. The action will be triggered when the user clicks on an object with the same tag and/or name.
	/// </summary>
	/// <param name="topic">The topic to subscribe to.</param>
	/// <param name="action">The action to be triggered.</param>
	public void Subscribe(string topic, Action<MouseButton, RaycastHit> action, bool persistent = false) {
		if (_actions.TryGetValue(topic, out InputAction value)) {
			_actions[topic].Action += action;
		} else {
			_actions.Add(topic, new InputAction(action, persistent));
		}
	}

	/// <summary>
	/// Unsubscribe from a topic.
	/// </summary>
	/// <param name="topic">The topic to subscribe to.</param>
	/// <param name="action">The action to be triggered.</param>
	public void Unsubscribe(string topic, Action<MouseButton, RaycastHit> action) {
		if (_actions.TryGetValue(topic, out InputAction value)) {
			_actions[topic].Action -= action;
			if (_actions[topic].Action.GetInvocationList().Length == 0) {
				_actions.Remove(topic);
			}
		}
	}

	/// <summary>
	/// Unsubscribe all events to that are currently listening.
	/// </summary>
	public void UnsubscribeAll() {
		List<string> keys = new List<string>(_actions.Keys);
		foreach (string key in keys) {
			if (_actions[key].Persistent == false) {
				foreach (var d in _actions[key].Action.GetInvocationList()) {
					_actions[key].Action -= (d as Action<MouseButton, RaycastHit>);
				}
				_actions.Remove(key);
			}
		}
	}

	/// <summary>
	/// Disable the inputs by unregistering the update function.
	/// </summary>
	public void DisableInput() {
		if (GameController.Instance.Alive) {
			GameController.Instance.DeregisterUpdatable(this);
		}
	}

	/// <summary>
	/// Enable the inputs by registering the update function.
	/// </summary>
	public void EnableInput() {
		if (GameController.Instance.Alive) {
			GameController.Instance.RegisterUpdatable(this);
		}
	}

	private void OnDestroy() {
		DisableInput();
	}

	private class InputAction {

		public InputAction(Action<MouseButton, RaycastHit> action, bool persistent = false) {
			Action = action;
			Persistent = persistent;
		}

		public Action<MouseButton, RaycastHit> Action { get; set; }
		public bool Persistent { get; private set; }
	}
}