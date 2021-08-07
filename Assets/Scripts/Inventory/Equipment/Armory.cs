﻿using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem {
	namespace EquipmentSystem {
		public class Armory : MonoBehaviour {

			public event Action<Equipment> OnEquipmentAddedEvent;
			public event Action<Equipment> OnEquipmentRemovedEvent;

			private Actor _actor;

			private void Awake() {
				foreach (SlotType slotType in (SlotType[])Enum.GetValues(typeof(SlotType))) {
					Equipments.Add(slotType, new Attachment());
				}
			}

			public void Initialize(Actor actor, ArmoryDto armoryDto) {
				_actor = actor;

				foreach (ItemDto itemDto in armoryDto.EquipmentDtos) {
					if (itemDto.ID != "-1") {
						if (!Equip(new Equipment(ItemManager.Instance.GetEquipmentData(itemDto.ID), itemDto.Amount), out Equipment previousEquipment)) {
							throw new UnityException("Please investigate there shouldn't an equipment in this slot.");
						}
					}
				}
			}

			/// <summary>
			/// Equip the equipment in the slot.
			/// </summary>
			/// <param name="slotType">The slot of the equipment</param>
			/// <param name="equipment">The equipment to be equipped.</param>
			/// <param name="previousEquipment">The previous equipment that was equipped.</param>
			/// <returns>Return true if the equipment could be equipped properly. Exception if any error occured.</returns>
			public bool Equip(Equipment equipment, out Equipment previousEquipment) {
				if (Equipments.TryGetValue(equipment.EquipmentData.SlotType, out Attachment attachment)) {
					previousEquipment = null;

					if (attachment.IsAttached && !Unequip(attachment, out previousEquipment)) {
						throw new UnityException("The Unequip method returned false, there is a problem, please verify.");
					}

					attachment.Attach(transform, equipment);

					if (equipment.EquipmentData.HideBodyPart) {
						_actor.Body.DisplayBodyParts(equipment.EquipmentData.SlotType, false);
					}

					_actor.Statistics.UpdateStatistics(equipment.EquipmentData.EquipmentStats.Statistics, true);

					OnEquipmentAddedEvent?.Invoke(equipment);

					return true;
				} else {
					throw new UnityException("The slot is not defined in this Equipment, please verify.");
				}
			}

			/// <summary>
			/// Unequip the Equipment from the slot.
			/// </summary>
			/// <param name="slotType">The slot type</param>
			/// <param name="equipment">The equipment that will be unequipped.</param>
			/// <returns>Return true if there is an equipment attached. Else false.</returns>
			public bool Unequip(SlotType slotType, out Equipment equipment) {
				if (Equipments.TryGetValue(slotType, out Attachment attachment)) {
					return Unequip(attachment, out equipment);
				} else {
					throw new UnityException("The slot is not defined in this Equipment, please verify.");
				}
			}

			/// <summary>
			/// Unequip method that detach the equipment from the body.
			/// </summary>
			/// <param name="attachment">The attachment</param>
			/// <param name="equipment">The equipment that is attached, if successful.</param>
			/// <returns>Return true, if there is an equipment attached.</returns>
			private bool Unequip(Attachment attachment, out Equipment equipment) {
				if (!attachment.IsAttached) {
					equipment = null;
					return false;
				}

				equipment = attachment.Detach();

				if (!ReferenceEquals(equipment, null) && equipment.EquipmentData.HideBodyPart) {
					_actor.Body.DisplayBodyParts(equipment.EquipmentData.SlotType, true);
				}

				_actor.Statistics.UpdateStatistics(equipment.EquipmentData.EquipmentStats.Statistics, false);

				OnEquipmentRemovedEvent?.Invoke(equipment);

				return true;
			}

			public Dictionary<SlotType, Attachment> Equipments { get; private set; } = new Dictionary<SlotType, Attachment>();
		}
	}
}