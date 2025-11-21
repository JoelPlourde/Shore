using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterSystem {
public class MonsterTarget : MonoBehaviour {

    private Collider _targetCollider;
    private Action _onTriggerEnter;

    public void SubscribeToTriggerEvents(Collider collider, Action onTriggerEnter) {
        _targetCollider = collider;
        _onTriggerEnter = onTriggerEnter;
    }

    public void OnTriggerEnter(Collider other) {
        if (other == _targetCollider) {
            _onTriggerEnter?.Invoke();

            // Destroy yourself after triggering
            Destroy(this.gameObject);
        }
    }
}
}
