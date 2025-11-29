using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriggerManager
{
	private static readonly GameObject _selectorTemplate;

	static TriggerManager() {
		_selectorTemplate = Resources.Load<GameObject>("Prefabs/Selector");
		if (_selectorTemplate == null) {
			throw new UnityException("Please define a projector gameobject at: Assets/Resources/Prefabs/Selector");
		}
	}

	public static Trigger CreateTrigger(Transform parent, float radius, Func<Collider, bool> onTriggerEnterCondition, Action atDestination) {
		return CreateTrigger(parent, radius, onTriggerEnterCondition, atDestination, null);
	}

	public static Trigger CreateTrigger(Transform parent, float radius, Func<Collider, bool> onTriggerEnterCondition, Action atDestination, TriggerOptions triggerOptions = null) {
		Trigger @object = CreateTrigger(parent.position, radius, onTriggerEnterCondition, atDestination, triggerOptions);
		@object.transform.SetParent(parent);
		@object.transform.localPosition = Vector3.zero;
		return @object;
	}

	public static Trigger CreateTrigger(Vector3 position, float radius, Func<Collider, bool> onTriggerEnterCondition, Action atDestination) {
        return CreateTrigger(position, radius, onTriggerEnterCondition, atDestination, null);
    }

	public static Trigger CreateTrigger(Vector3 position, float radius, Func<Collider, bool> onTriggerEnterCondition, Action atDestination, TriggerOptions triggerOptions = null) {
		GameObject @object = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		@object.transform.position = position;
		Trigger trigger = @object.AddComponent<Trigger>();
		if (triggerOptions != null)
        {
            GameObject projector = UnityEngine.Object.Instantiate(_selectorTemplate, @object.transform);
			projector.transform.localPosition = Vector3.zero + 5f * triggerOptions.Radius * Vector3.up;
			var projectorComponent = projector.GetComponent<Projector>();
			projectorComponent.material = new Material(projectorComponent.material);
			projectorComponent.material.SetColor("_Color", triggerOptions.Color);
        }
		trigger.Initialize(radius, onTriggerEnterCondition);
		trigger.OnTriggerEnterEvent += atDestination;
		return trigger;
	}

	public class TriggerOptions
    {
        public float Radius;
		public Color Color;
    }
}