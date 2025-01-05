﻿using UnityEngine;
using DropSystem;
using System.Collections.Generic;
using ItemSystem;
using UI;
using TaskSystem;
using JL.Splitting;

namespace NodeSystem {
	public class NodeBehaviour : InteractableBehavior, IInteractable {

		[Tooltip("The radius at which the player should stopped at.")]
		public float InteractionRadius;

		[SerializeField]
		public NodeData NodeData;

		protected Node _node;

		public void Start() {
			// Register the particle system if any.
			if (!ReferenceEquals(NodeData.OnHit, null)) {
				if (NodeData.OnHit.ParticleSystem != null) {
					ParticleSystemManager.Instance.RegisterParticleSystem(NodeData.OnHit.ParticleSystem?.name, NodeData.OnHit.ParticleSystem);
				}
			}

			OnStart();
		}

		/// <summary>
		/// On Mouse Over, opens the right click menu.
		/// </summary>
		private void OnMouseOver() {
            if (Input.GetMouseButtonDown(1)) {
                OptionsHandler.Instance.OpenRightClickMenu(this, NodeData, InteractCallback);
            }
        }

		/// <summary>
		/// Callback to create the Interact task.
		/// </summary>
		private void InteractCallback() {
			InteractArguments interactArguments = new InteractArguments(transform.position, this);
			if (Squad.FirstSelected(out Actor actor)) {
				actor.TaskScheduler.CreateTask<Interact>(interactArguments);
			}
		}

		/// <summary>
		/// On Interact Enter, starts the gathering process.
		/// </summary>
		/// <param name="actor">The actor that interacted with this node.</param>
		public void OnInteractEnter(Actor actor) {

			if (ReferenceEquals(_node, null)) {
				_node = new Node(NodeData);
				_node.Initialize(OnHarvest);
			}

			if (_node.IsDepleted()) {
				actor.Emotion.PlayEmote(EmoteSystem.EmoteType.SHRUG);
				actor.TaskScheduler.CancelTask();
				return;
			}

			if (actor.Skills.GetLevel(NodeData.SkillType).Value < NodeData.Requirement) {
				actor.Emotion.PlayEmote(EmoteSystem.EmoteType.WONDERING);
				actor.TaskScheduler.CancelTask();
				return;
			}

			if (!actor.Armory.HasWeaponEquipped(NodeData.WeaponType)) {
				actor.Emotion.PlayEmote(EmoteSystem.EmoteType.WONDERING);
				actor.TaskScheduler.CancelTask();
				return;
			}

			actor.Animator.SetFloat("Harvest Speed", GameplayUtils.CalculateRepeatRateBasedOnSpeed(actor.Statistics.GetStatistic(NodeData.SpeedStatistic)));
			actor.Animator.SetBool("Harvest", true);
			actor.OnHarvestEvent += OnHit;
		}

		/// <summary>
		/// Callback that will be called whenever the Actor.OnHarvestEvent is triggered.
		/// </summary>
		/// <param name="actor">The actor the event is called on.</param>
		protected virtual void OnHit(Actor actor) {
			if (NodeData.OnHit.ParticleSystem != null) {
				ParticleSystemManager.Instance.SpawnParticleSystem(NodeData.OnHit.ParticleSystem.name, actor.transform.position + NodeData.OnHit.RelativePosition);
			}

			if (!ReferenceEquals(NodeData.OnHit.Sound, null)) {
				actor.AudioPlayer.PlayOneShot(NodeData.OnHit.Sound);
			}

			if (_node.ReduceHealth(actor, actor.Statistics.GetStatistic(NodeData.DamageStatistic)) <= 0) {
				actor.TaskScheduler.CancelTask();
			}

			OnResponse(actor);
		}

		protected virtual void OnDepleted() {
			if (NodeData.DestroyOnDepletion) {
				if (NodeData.OnDestroy.ParticleSystem != null) {
					Instantiate(NodeData.OnDestroy.ParticleSystem, transform.position + NodeData.OnDestroy.RelativePosition, Quaternion.identity);
				}

				Splittable splittable = gameObject.AddComponent<Splittable>();
				splittable.targetMeshFilter = GetComponent<MeshFilter>();
				splittable.CapMaterial = GetComponent<MeshRenderer>().material;
				splittable.generateMeshColliders = true;

				splittable.SplitForce = 100f;

				Quaternion randomRotation = Random.rotation;
				Debug.Log(randomRotation);

				PointPlane plane = new PointPlane(transform.GetComponent<Collider>().bounds.center, randomRotation);

				SplitResult splitResult = splittable.Split(plane);

				Rigidbody rigidbody1 = splitResult.negObject.AddComponent<Rigidbody>();
				rigidbody1.AddForce(-plane.normal * 1f, ForceMode.Impulse);
				LeanTween.alpha(splitResult.negObject, 0f, 2f).setOnComplete(() => Destroy(splitResult.negObject));

				Rigidbody rigidbody2 =splitResult.posObject.AddComponent<Rigidbody>();
				rigidbody2.AddForce(plane.normal * 1f, ForceMode.Impulse);
				LeanTween.alpha(splitResult.posObject, 0f, 2f).setOnComplete(() => Destroy(splitResult.posObject));
			}
		}

		/// <summary>
		/// On Interact Exit, stops the gathering process.
		/// </summary>
		/// <param name="actor"></param>
		public void OnInteractExit(Actor actor) {
			actor.Animator.SetBool("Harvest", false);
			actor.OnHarvestEvent -= OnHit;
		}

		public virtual void OnStart() {}

		public virtual void OnResponse(Actor actor) {}
	

		private void OnHarvest(Actor actor) {
			Drop drop = NodeData.DropTable.GetRandomDrop();

			actor.Inventory.AddItemsToInventory(new List<Item> { drop.ToItem() }, out List<Item> remainingItems);
			if (remainingItems.Count > 0) {
				Debug.Log("Do something with the exceeding items!");
			}

			actor.Skills.GainExperience(NodeData.SkillType, NodeData.Experience);

			if (_node.IsDepleted()) {
				OnDepleted();
			}
		}

		public float GetInteractionRadius() {
			return InteractionRadius;
		}

		public string GetDefaultAction() {
			return NodeData.Action;
		}

		protected override OutlineType GetOutlineType() {
			return OutlineType.INTERACTABLE;
		}

		public void OnDrawGizmosSelected() {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, InteractionRadius);
		}
	}
}
