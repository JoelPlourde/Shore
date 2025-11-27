using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace ItemSystem {
	namespace EquipmentSystem {
		public class Armory : MonoBehaviour {

			public event Action<Equipment> OnEquipmentAddedEvent;
			public event Action<Equipment> OnEquipmentRemovedEvent;

			private Actor _actor;

			private WeaponType _currentWeaponType;
			private WeaponType _currentShieldType;

			private WeaponDamageType _currentWeaponDamageType;

			private void Awake() {
				foreach (SlotType slotType in (SlotType[])Enum.GetValues(typeof(SlotType))) {
					Equipments.Add(slotType, new Attachment());
				}
			}

			public void Initialize(Actor actor, ArmoryDto armoryDto) {
				_actor = actor;

				foreach (ItemDto itemDto in armoryDto.EquipmentDtos) {
					if (ReferenceEquals(itemDto, null)) {
						continue;
					}

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

					if (equipment.EquipmentData.SlotType == SlotType.WEAPON) {
						// TODO If two-hand, remove the shield, if any. TEST THIS

						_currentWeaponType = equipment.EquipmentData.WeaponType;
						_currentWeaponDamageType = equipment.EquipmentData.WeaponDamageType;

						_actor.Animator.SetBool("Weapon", true);
						_actor.Animator.SetInteger("Weapon Type", (int) _currentWeaponType);

						_actor.Emotion.PlayEmote(EmoteSystem.EmoteType.SHEATHE);

						_actor.Creature.DamageCategoryType = equipment.EquipmentData.DamageCategoryType;
					}

					if (equipment.EquipmentData.SlotType == SlotType.SHIELD) {
						_currentShieldType = equipment.EquipmentData.WeaponType;

						_actor.Animator.SetBool("Shield", true);
						_actor.Animator.SetInteger("Shield Type", (int)_currentShieldType);
					}

					attachment.Attach(transform, equipment);

					if (equipment.EquipmentData.HideBodyPart) {
						_actor.Body.DisplayBodyParts(equipment.EquipmentData.SlotType, false);
					}

					if (equipment.EquipmentData.SlotType == SlotType.BODY) {
						_actor.Body.DisplayBodyParts(BodySystem.BodyPartType.ARMS, !equipment.EquipmentData.HideArms);
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
			/// Check if the Weapon Type is equipped.
			/// </summary>
			/// <param name="weaponType">The Weapon type to check.</param>
			/// <returns>True if the weapon type corresponds, else false.</returns>
			public bool HasWeaponEquipped(WeaponType weaponType) {
				return _currentWeaponType == weaponType;
			}

			/// <summary>
            /// Check if the Weapon SubType is equipped.
            /// </summary>
            /// <param name="weaponDamageType"></param>
            /// <returns></returns>
			public bool HasWeaponDamageTypeEquipped(WeaponDamageType weaponDamageType) {
				return _currentWeaponDamageType == weaponDamageType;
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

				if (equipment.EquipmentData.SlotType == SlotType.WEAPON) {
					_currentWeaponType = WeaponType.NONE;

					_actor.Animator.SetBool("Weapon", false);
					_actor.Animator.SetInteger("Weapon Type", (int)_currentWeaponType);

					_actor.Emotion.PlayEmote(EmoteSystem.EmoteType.SHEATHE);
				}

				if (equipment.EquipmentData.SlotType == SlotType.SHIELD) {
					_currentShieldType = WeaponType.NONE;

					_actor.Animator.SetBool("Shield", false);
					_actor.Animator.SetInteger("Shield Type", (int) _currentShieldType);
				}

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
