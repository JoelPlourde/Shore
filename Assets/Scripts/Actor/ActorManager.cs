﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveSystem;

public class ActorManager : MonoBehaviour, ISaveable {

	public static ActorManager Instance;

	private GameObject _actorTemplate;

	private void Awake() {
		Instance = this;

		_actorTemplate = Resources.Load<GameObject>("Prefabs/Actor");
		if (ReferenceEquals(_actorTemplate, null)) {
			throw new UnityException("Please defined the Actor prefab under: Resources/Prefabs/Actor");
		}
	}

	public void Load(Save save) {
		foreach (ActorDto actorDto in save.ActorDtos) {
			SpawnActor(actorDto);
		}

		Squad.SelectActors(save.SquadDto.ActorGuids);
	}

	public void Save(Save save) {
		List<ActorDto> actorDtos = new List<ActorDto>();
		foreach (Actor actor in FindObjectsByType<Actor>(FindObjectsSortMode.None)) {
			actorDtos.Add(new ActorDto(actor));
		}
		save.ActorDtos = actorDtos;
		if (Application.isPlaying) {
			save.SquadDto.ActorGuids = Squad.GetSelectedActorIds();
		}
	}

	private void SpawnActor(ActorDto actorDto) {
		GameObject actorObj = Instantiate(_actorTemplate, actorDto.Position, Quaternion.identity);
		Actor actor = actorObj.GetComponent<Actor>();
		actor.Initialize(actorDto);
	}
}
