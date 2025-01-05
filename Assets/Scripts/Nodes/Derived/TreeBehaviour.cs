using UnityEngine;

namespace NodeSystem {
	public class TreeBehaviour : NodeBehaviour {

		private Vector3 _direction;
		private TreeData _treeData;

		public override void OnStart() {
			_treeData = (TreeData)NodeData;
			if (!ReferenceEquals(_treeData.OnResponse, null)) {
				ParticleSystemManager.Instance.RegisterParticleSystem(_treeData.OnResponse.ParticleSystem.name, _treeData.OnResponse.ParticleSystem);
			}
		}

		public override void OnResponse(Actor actor) {
			// Spawn a Particle System in response.
			if (!ReferenceEquals(_treeData.OnResponse.ParticleSystem, null)) {
				ParticleSystemManager.Instance.SpawnParticleSystem(_treeData.OnResponse.ParticleSystem.name, transform.position + _treeData.OnResponse.RelativePosition);
			}

			// Calculate the direction to shake.
			_direction = transform.position - actor.transform.position;
			_direction.y = 0;

			LeanTween.rotateAroundLocal(gameObject, _direction, 2f, 0.05f).setOnComplete(RotateBack);
		}

		private void RotateBack() {
			LeanTween.rotateAroundLocal(gameObject, _direction, -2f, 0.05f);
		}
	}
}
