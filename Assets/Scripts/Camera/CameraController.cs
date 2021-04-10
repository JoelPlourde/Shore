﻿using UnityEngine;

namespace CameraSystem {
	public class CameraController : MonoBehaviour, IUpdatable {

		public static CameraController Instance;

		public CameraTarget Target;

		[Header("LayerMask")]
		public LayerMask ignoreLayer;
		public LayerMask blockedByLayer;

		[Header("Sensitivity Parameters")]
		public int CameraSensitivity = 300;
		public int ScrollSensitivity = 2000;
		public float SmoothSensitivity = 0.5f;
		public int MovingSpeed = 3;

		[Header("Zoom Parameters")]
		public int MinZoom = 5;
		public int MaxZoom = 30;

		private RaycastHit _hit;
		protected private Vector3 _localRotation;
		protected private Vector3 _smoothedPosition;
		protected private float _desiredDistance;

		private void Awake() {
			Instance = this;
			Camera = GetComponent<Camera>();
			Distance = (MinZoom + MaxZoom) / 2;

			if (ReferenceEquals(Target, null)) {
				throw new UnityException("Please assign a CameraTarget to the CameraController object.");
			}
			Target.Initialize(blockedByLayer);
		}

		private void Start() {
			GameController.Instance.RegisterLateUpdatable(this);
		}

		public void OnUpdate() {
			_desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.smoothDeltaTime * ScrollSensitivity;

			Vector3 direction = (Target.transform.position - transform.position);
			if (Physics.Raycast(transform.position, direction, out _hit, _desiredDistance, ignoreLayer, QueryTriggerInteraction.UseGlobal)) {
				Distance = (_hit.point - Target.transform.position).magnitude + 0.2f;
			} else {
				Distance = _desiredDistance;
			}
			Distance = Mathf.Clamp(Distance, MinZoom, MaxZoom);

			if (Input.GetMouseButton(2)) {
				_localRotation.x += Input.GetAxis("Mouse X") * Time.smoothDeltaTime * CameraSensitivity;
				_localRotation.y += Input.GetAxis("Mouse Y") * Time.smoothDeltaTime * CameraSensitivity;
				_localRotation.y = Mathf.Clamp(_localRotation.y, -60, 60);
			}

			Target.transform.Translate(Input.GetAxis("Horizontal") * transform.right * (MovingSpeed * ((int)Distance >> 1)) * Time.deltaTime, Space.World);
			Target.transform.Translate(Input.GetAxis("Vertical") * transform.forward * (MovingSpeed * ((int)Distance >> 1)) * Time.deltaTime, Space.World);

			transform.position = Vector3.Lerp(transform.position, Target.transform.position + Quaternion.Euler(_localRotation.y, _localRotation.x, 0f) * (Distance * -Vector3.back), SmoothSensitivity);
			transform.LookAt(Target.transform.position, Vector3.up);
		}

		public void FollowTarget(Transform target) {
			Target.FollowTarget(target);
		}

		public void StopFollow() {
			Target.CancelFollow();
		}

		public void FocusOnTarget(Transform target) {
			Target.transform.position = target.transform.position;
		}

		public void OnDestroy() {
			if (GameController.Instance.Alive) {
				GameController.Instance.DeregisterLateUpdatable(this);
			}
		}

		public Camera Camera { get; private set; }
		public float Distance { get; private set; } = 20;
	}
}

