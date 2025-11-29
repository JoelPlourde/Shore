using CameraSystem;
using CraftingSystem.SaveSystem;
using CraftingSystem.UI;
using UnityEngine;

namespace CraftingSystem {
	public class ProcessorBehaviour : InteractableBehavior, IInteractable {

		[SerializeField]
		private float _interactionRadius;

		[SerializeField]
		private ProcessorData _processorData;

		[Header("General Parameters")]
		[Tooltip("When the Camera is zoomed in on the object, what is the minimum height the UI should be displayed at ?")]
		public int MinHeight = 1;

		[Tooltip("When the Camera is zoomed out on the object, what is the maximum height the UI should be displayed at ?")]
		public int MaxHeight = 4;

		private bool _isInteracting = false;
		private Vector3 _offset;
		private float _heightRatio;

		private void Start() {
			_heightRatio = (CameraController.Instance.MaxZoom - CameraController.Instance.MinZoom) / (MaxHeight - MinHeight);
			if (ReferenceEquals(Processor, null)) {
				Processor = new Processor(_processorData);
				Processor.OnInputEvent += OnItemInputs;
				Processor.OnFuelEvent += OnFuelInputs;
				Processor.OnOutputEvent += OnFuelInputs;
				Processor.OnInputBeginDragEvent += OnItemInputsBeginDrag;
			}

			// TODO Initialize the processor with a DTO.
		}

		private void OnDestroy() {
			if (!ReferenceEquals(Processor, null)) {
				Processor.OnInputEvent -= OnItemInputs;
				Processor.OnFuelEvent -= OnFuelInputs;
				Processor.OnInputBeginDragEvent -= OnItemInputsBeginDrag;
				Processor.OnOutputEvent -= OnFuelInputs;
				Processor.Destroy();
			}
		}

		private void OnFuelInputs() {
			// Verify if you can start it.
			if (!Processor.IsProcessing && Processor.StartProcess()) {
				InvokeRepeating(nameof(Routine), 1f, 1f);
			}
		}

		private void OnItemInputs() {
			// If the process is already processing, restart the progress.
			if (Processor.IsProcessing) {
				CancelInvoke();

				Processor.ResetProgress();
			}

			// Verify if you can start it again.
			if (!Processor.IsProcessing && Processor.StartProcess()) {
				InvokeRepeating(nameof(Routine), 1f, 1f);
			}
		}

		private void OnItemInputsBeginDrag() {
			if (Processor.IsProcessing) {
				CancelInvoke();
			}
		}

		private void Routine() {
			if (Processor.IsProcessing) {
				if (!Processor.ProcessRoutine()) {
					if (!Processor.StopProgress()) {
						CancelInvoke();
					}
					if (!Processor.StartProcess()) {
						CancelInvoke();
					}
				}
			}
		}

		#region Interact
		public void OnInteractEnter(Actor actor) {
			// To deactive Mouse Events.
			_isInteracting = true;

			// Display it.
			ProcessorHandler.Instance.DisplayProcessor(this);
		}

		public void OnInteractExit(Actor actor) {
			ProcessorHandler.Instance.HideProcessor(this);

			// To Reactivate Mouse Events.
			_isInteracting = false;
		}
		#endregion

		/*
		#region OnMouseEvents
		public void OnMouseEnter() {
			if (_isInteracting && ProcessorHandler.Instance.IsActive) {
				return;
			}

			ProcessorHandler.Instance.DisplayProcessor(this);
		}

		public void OnMouseExit() {
			if (_isInteracting) {
				return;
			}

			ProcessorHandler.Instance.HideProcessor(this);
		}
		#endregion
		*/

		public float GetInteractionRadius() {
			return _interactionRadius;
		}

		public override OutlineType GetOutlineType() {
			return OutlineType.INTERACTABLE;
		}

		public Vector3 WorldPosition { get { return transform.position + new Vector3(0, CameraController.Instance.Distance / _heightRatio, 0); } }
		public Processor Processor { get; private set; }
	}
}
