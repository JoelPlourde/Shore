using UnityEngine;
using System.Collections.Generic;
using System;

public class InteractionManager : MonoBehaviour {

    public static InteractionManager Instance;

    private Dictionary<OutlineType, Material> _outlineMaterials = new Dictionary<OutlineType, Material>();

    private void Awake() {
        Instance = this;

        Material template = Resources.Load<Material>("Materials/Outline");

        // Create a new Material for each OutlineType and add it to the Dictionary:
        foreach (OutlineType outlineType in Enum.GetValues(typeof(OutlineType))) {
            Material material = new Material(template);

            // Set the parameter "Outline Color" of the shader:
            Color outlineColor = getOutlineColor(outlineType);
            material.SetColor("_OutlineColor", outlineColor);
            _outlineMaterials.Add(outlineType, material);
        }
    }

    /**
     * Get the Material for the given OutlineType.
     */
    public Material GetOutlineMaterial(OutlineType outlineType) {
        if (!_outlineMaterials.TryGetValue(outlineType, out Material material)) {
            Debug.LogWarning("No material found for OutlineType: " + outlineType);
            return null;
        }
        return material;
    }

    /**
     * Get the Color for the given OutlineType.
     */
    private static Color getOutlineColor(OutlineType outlineType) {
        Color hexColor;
        switch (outlineType) {
            case OutlineType.INTERACTABLE:
                if (ColorUtility.TryParseHtmlString("#ffe912", out hexColor)) {
                    return hexColor;
                }
                return Color.white;
            case OutlineType.ITEM:
                if (ColorUtility.TryParseHtmlString("#20a7db", out hexColor)) {
                    return hexColor;
                }
                return Color.white;
            case OutlineType.PLAYER:
                if (ColorUtility.TryParseHtmlString("#32cd32", out hexColor)) {
                    return hexColor;
                }
                return Color.white;
            case OutlineType.NPC:
                if (ColorUtility.TryParseHtmlString("ff7603", out hexColor)) {
                    return hexColor;
                }
                return Color.white;
            case OutlineType.ENEMY:
                if (ColorUtility.TryParseHtmlString("#ff0000", out hexColor)) {
                    return hexColor;
                }
                return Color.white;
            default:
                return Color.white;
        }
    }
}