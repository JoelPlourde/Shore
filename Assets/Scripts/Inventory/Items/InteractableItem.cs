using UnityEngine;
using System.Collections.Generic;
using UI;
using TaskSystem;
using System;
using SaveSystem;

namespace ItemSystem {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class InteractableItem : InteractableBehavior, IInteractable, IWorldSaveable {

        [UniqueIdentifier]
        public string UUID;

		[Tooltip("The radius at which the player should stopped at.")]
		public float InteractionRadius = 0.5f;

        [SerializeField]
        public ItemData ItemData;

        public void OnInteractEnter(Actor actor) {
			Item item = new Item(ItemData, 1);

			actor.Inventory.AddItemsToInventory(new List<Item> { item }, out List<Item> remainingItems);
            if (remainingItems.Count > 0) {
                Debug.Log("Inventory is full!");
                return;
            }

            OnInteractExit(actor);
        }

        public void OnInteractExit(Actor actor) {
            if (!this) {
                return;
            }
            Destroy(this.gameObject);
        }

        private void OnMouseOver() {
            if (Input.GetMouseButtonDown(1)) {
                OptionsHandler.Instance.OpenRightClickMenu(this, ItemData, InteractCallback);
            }
        }

        private void InteractCallback() {
            InteractArguments interactArguments = new InteractArguments(transform.position, this);
            if (Squad.FirstSelected(out Actor actor)) {
                actor.TaskScheduler.CreateTask<Interact>(interactArguments);
            }
        }

		public float GetInteractionRadius() {
			return InteractionRadius;
		}

        public bool IsPickup() {
            return true;
        }

        public string GetDefaultAction() {
            return "pick_up";
        }

        protected override OutlineType GetOutlineType() {
            return OutlineType.ITEM;
        }

        #region SaveSystem
        public void Load(Save save) {
            if (save.WorldItemDtos.TryGetValue(UUID, out WorldItemDto worldItemDto)) {
                // Replace the object where it was last saved.
                transform.position = worldItemDto.Position;
                transform.eulerAngles = worldItemDto.Rotation;

                // Get any IConditionaSaveable on this object or underneath it.
                IConditionalSaveable[] conditionalSaveables = GetComponentsInChildren<IConditionalSaveable>();
                foreach (IConditionalSaveable conditionalSaveable in conditionalSaveables) {
                    if (worldItemDto.conditionalData.TryGetValue(conditionalSaveable.GetType().Name, out string data)) {
                        conditionalSaveable.LoadSerializedData(data);
                    }
                }
            }
        }

        public void Save(Save save) {
            WorldItemDto worldItemDto = new WorldItemDto(GetUUID(), ItemData.ID, 1, transform.position, transform.eulerAngles);

            // Get any IConditionaSaveable on this object or underneath it.
            IConditionalSaveable[] conditionalSaveables = GetComponentsInChildren<IConditionalSaveable>();
            foreach (IConditionalSaveable conditionalSaveable in conditionalSaveables) {
                if (conditionalSaveable.IsSaveRequired()) {
                    worldItemDto.AppendData(conditionalSaveable.GetType().Name, conditionalSaveable.GetSerializedData());
                }
            }

            if (save.WorldItemDtos.ContainsKey(GetUUID())) {
                save.WorldItemDtos[GetUUID()] = worldItemDto;
            } else {
                save.WorldItemDtos.Add(GetUUID(), worldItemDto);
            }
        }

        public string GetUUID() {
            return UUID;
        }

        public void RegenerateUUID() {
            UUID = Guid.NewGuid().ToString();
        }

        public bool DetermineState(Save worldSave, Save playerSave) {
            bool existsInWorld = worldSave.WorldItemDtos.ContainsKey(UUID);
            bool existsInPlayer = playerSave.WorldItemDtos.ContainsKey(UUID);

            if (existsInWorld && !existsInPlayer) {
                // The item exists in the world but not in the player' save, therefore it has been picked up, delete it.
                Destroy(gameObject);
                return false;
            }

            if (existsInWorld && existsInPlayer) {
                // The item exists in both the world and the player's save, it may or may not have been interacted with, load it.
                return true;
            }

            return true;
        }
        #endregion
    }
}