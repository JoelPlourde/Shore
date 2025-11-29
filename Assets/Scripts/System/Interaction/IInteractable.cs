using UnityEngine;

public interface IInteractable
{
	void OnInteractEnter(Actor actor);

	void OnInteractExit(Actor actor);

	// To determine how close the Actor has to be to interact with this.
	float GetInteractionRadius();

	virtual bool IsPickup() {
		return false;
	}

	virtual string GetDefaultAction() {
		return "Interact";
	}

	virtual OutlineType GetOutlineType() {
		return OutlineType.INTERACTABLE;
	}
}