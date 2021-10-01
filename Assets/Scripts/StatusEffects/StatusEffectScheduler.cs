﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace StatusEffectSystem {
	public class StatusEffectScheduler : MonoBehaviour {

		private static readonly Dictionary<Guid, StatusEffectScheduler> _instances = new Dictionary<Guid, StatusEffectScheduler>();

		public event Action<Status> OnAddStatusEffectEvent;
		public event Action<string> OnRemoveStatusEffectEvent;
		public event Action<List<Status>> OnUpdateStatusEffectsEvent;
		public event Action<Status> OnUpdateStatusEffectEvent;

		private Guid _guid;
		private bool _started = false;
		private readonly HashSet<string> _currentStatuses = new HashSet<string>();
		private readonly List<Status> _statuses = new List<Status>();

		public static StatusEffectScheduler Instance(Guid guid) {
			if (_instances.TryGetValue(guid, out StatusEffectScheduler statusEffectScheduler)) {
				return statusEffectScheduler;
			}
			throw new UnityException("This instance of StatusEffectScheduler does not exists by Guid: " + guid.ToString());
		}

		public void Initialize(Guid guid) {
			_guid = guid;
			_instances.Add(guid, this);
		}

		public void Routine() {
			foreach (Status status in _statuses.ToList()) {
				if (status.StatusEffectData.Temporary == true) {
					status.ReduceDuration();

					if (status.Duration <= 0) {
						RemoveStatusEffect(status.StatusEffectData.Name);
					}
				}
			}
			OnUpdateStatusEffectsEvent?.Invoke(_statuses.Where(FilterStatus).ToList());
		}

		public void AddStatusEffect(Status status) {
			string key = status.StatusEffectData.Name;

			if (_currentStatuses.Contains(key)) {
				int index = _findStatusEffectIndexByKey(key);

				if (status.StatusEffectData.Reset == true) {
					_statuses[index].ResetDuration();
				} else {
					_statuses[index].IncreaseDuration(status.Duration);
				}

				if (status.StatusEffectData.CanStack == true) {
					_statuses[index].IncreaseStack();
				}

				OnUpdateStatusEffectEvent?.Invoke(_statuses[index]);
			} else {
				_currentStatuses.Add(key);
				status.ResetDuration();
				_statuses.Add(status);

				foreach (StatusEffectType statusEffectType in status.StatusEffectData.statusEffectTypes) {
					StatusEffectFactory.GetStatusEffect(statusEffectType).Apply(status);
				}

				if (status.StatusEffectData.Hidden == false) {
					Debug.Log("Here!");
					OnAddStatusEffectEvent?.Invoke(status);
				}
			}

			if (_started == false) {
				_started = true;
				InvokeRepeating(nameof(Routine), 1f, 1f);
			}
		}

		public void RemoveStatusEffect(string name) {
			int index = _findStatusEffectIndexByKey(name);
			if (index == -1) {
				return;
			}

			if (_started == true && _statuses.Count == 1) {
				_started = false;
				CancelInvoke();
			}

			foreach (StatusEffectType statusEffectType in _statuses[index].StatusEffectData.statusEffectTypes) {
				StatusEffectFactory.GetStatusEffect(statusEffectType).Unapply(_statuses[index]);
			}

			_currentStatuses.Remove(name);

			_statuses.RemoveAt(index);

			OnRemoveStatusEffectEvent?.Invoke(name);
		}

		private int _findStatusEffectIndexByKey(string name) {
			return _statuses.FindIndex(x => x.StatusEffectData.Name == name);
		}

		private readonly Func<Status, bool> FilterStatus = (Status status) => {
			return status.StatusEffectData.Hidden == false && (status.Duration < 60) || ((status.Duration + 1) % 60) == 0;
		};
	}
}
