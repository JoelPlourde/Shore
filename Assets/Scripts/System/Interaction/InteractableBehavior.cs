using UnityEngine;
using System.Collections.Generic;
using TaskSystem;
using PointerSystem;
using UI;

public class InteractableBehavior : MonoBehaviour {

    private Renderer _renderer;

    private int RENDER_QUEUE = 3000;

    protected void Awake() {
        // Find the mesh renderer
        _renderer = GetComponent<MeshRenderer>();

        if (ReferenceEquals(_renderer, null)) {
            _renderer = GetComponentInChildren<MeshRenderer>();
        }

        // List all of the children to find a SkinnedMeshRenderer if no MeshRenderer found
        foreach (Transform child in transform) {
            if (child.GetComponent<SkinnedMeshRenderer>() != null) {
                _renderer = child.GetComponent<SkinnedMeshRenderer>();
                break;
            }
        }

        if (ReferenceEquals(_renderer, null)) {
            Debug.LogWarning("No MeshRenderer or SkinnedMeshRenderer found on this object.");
        }
    }

    public void OnMouseEnter() {
        string tooltip = GetActionLabel() + " " + GetEntityLabel();
        Tooltip.Instance.ShowTooltip(tooltip, 0f);

        PointerManager.Instance.SetPointer(GetPointerMode());

        if (ReferenceEquals(_renderer, null)) {
            Debug.LogWarning("No MeshRenderer found on this object.");
            return;
        }

        List<Material> materials = new List<Material>(_renderer.materials);
        foreach (Material material in materials) {
            material.renderQueue = RENDER_QUEUE + 1000;
        }
        materials.Add(new Material(InteractionManager.Instance.GetOutlineMaterial(GetOutlineType())) {
            renderQueue = RENDER_QUEUE
        });
        _renderer.materials =  materials.ToArray();
    }

    public void OnMouseExit() {
        Tooltip.Instance.HideTooltip();

        PointerManager.Instance.SetPointer(PointerMode.DEFAULT);

        List<Material> materials = new List<Material>(_renderer.materials);
        foreach (Material material in materials) {
            material.renderQueue = RENDER_QUEUE - 1000;
        }
        materials.RemoveAt(materials.Count - 1);
        _renderer.materials =  materials.ToArray();
    }

    protected virtual OutlineType GetOutlineType() {
        return OutlineType.INTERACTABLE;
    }

    protected virtual PointerMode GetPointerMode() {
        return PointerMode.DEFAULT;
    }

    protected virtual string GetActionLabel()
    {
        return I18N.GetValue("interact");
    }

    protected virtual string GetEntityLabel()
    {
        return I18N.GetValue("entity");
    }
}