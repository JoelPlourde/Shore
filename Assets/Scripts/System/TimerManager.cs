using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : SingletonBehaviour<TimerManager> {

	private static readonly SortedList<DateTime, Action> _actions = new SortedList<DateTime, Action>();

	public void Enqueue(DelayedAction delayedAction) {
		_actions.Add(delayedAction.ReadyTime, delayedAction.Action);
		if (!IsInvoking()) {
			InvokeRepeating(nameof(Routine), 0f, 0.016f);
		}
	}

	private void Routine() {
		if (Time.frameCount % 2 == 0) {
			if (_actions.Count > 0) {
				var first = _actions.Keys[0];
				if (first <= DateTime.Now) {
					var action = _actions[first];
					_actions.RemoveAt(0);
					action();
				}
			} else {
				CancelInvoke();
			}
		}
	}
}

public class DelayedAction {
	public Action Action { get; private set; }
	public DateTime ReadyTime { get; private set; }

	public DelayedAction(Action callback, float seconds) {
		Action = callback;
		ReadyTime = DateTime.Now.Add(TimeSpan.FromSeconds(seconds));
	}
}
