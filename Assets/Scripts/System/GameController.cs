﻿using UnityEngine;
using StatusEffectSystem;
using System.Collections.Generic;
using System;
using QuestSystem;
using SaveSystem;
using PointerSystem;
using GameSystem;
using ItemSystem;
using SkillSystem;
using EmoteSystem;
using ElementalRift;

[RequireComponent(typeof(ParticleSystemManager))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SoundManager))]
[RequireComponent(typeof(SoundSystem))]
[RequireComponent(typeof(SkillManager))]
[RequireComponent(typeof(IconManager))]
[RequireComponent(typeof(EmoteManager))]
[RequireComponent(typeof(ItemManager))]
[RequireComponent(typeof(SceneController))]
[RequireComponent(typeof(PointerManager))]
[RequireComponent(typeof(SaveManager))]
[RequireComponent(typeof(ActorManager))]
[RequireComponent(typeof(QuestManager))]
[RequireComponent(typeof(HungerStatusEffect))]
[RequireComponent(typeof(HealOverTimeEffect))]
[RequireComponent(typeof(HealthRegenerationStatusEffect))]
[RequireComponent(typeof(SlowOverTimeEffect))]
[RequireComponent(typeof(DamageOverTimeEffect))]
[RequireComponent(typeof(TimerManager))]
[RequireComponent(typeof(Foreman))]
[RequireComponent(typeof(Time))]
[RequireComponent(typeof(ElementManager))]
[RequireComponent(typeof(RiftManager))]
[RequireComponent(typeof(InteractionManager))]
public class GameController : SingletonBehaviour<GameController> {

	private List<IUpdatable> _updates = new List<IUpdatable>();
	private List<IUpdatable> _lateUpdates = new List<IUpdatable>();

	void Update() {
		for (int i = 0; i < _updates.Count; i++) {
			_updates[i].OnUpdate();
		}
	}

	void LateUpdate() {
		// float deltaTime = Time.deltaTime; // Could be used to yield CPU if framerate lowers.
		for (int i = 0; i < _lateUpdates.Count; i++) {
			_lateUpdates[i].OnUpdate();
		}
	}

	public void RegisterUpdatable(IUpdatable updatable) {
		if (!_updates.Contains(updatable)) {
			_updates.Add(updatable);
		}
	}

	public void DeregisterUpdatable(IUpdatable updatable) {
		if (_updates.Contains(updatable)) {
			_updates.Remove(updatable);
		}
	}

	public void RegisterLateUpdatable(IUpdatable updatable) {
		if (!_lateUpdates.Contains(updatable)) {
			_lateUpdates.Add(updatable);
		}
	}

	public void DeregisterLateUpdatable(IUpdatable updatable) {
		if (_lateUpdates.Contains(updatable)) {
			_lateUpdates.Remove(updatable);
		}
	}
}
