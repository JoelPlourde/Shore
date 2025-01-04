using ItemSystem;
using Tools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(ItemMaker))]
public class ItemMakerInspector : Editor {

    public override VisualElement CreateInspectorGUI() {
        ItemMaker itemMaker = (ItemMaker)target;

        VisualElement root = new VisualElement();

        // Add a dropdown
        ItemCategory selectedCategory = ItemCategory.RESOURCES;
        EnumField enumField = new EnumField("Item Category", ItemCategory.RESOURCES);
        enumField.RegisterValueChangedCallback((evt) => {
            selectedCategory = (ItemCategory)evt.newValue;
        });

        root.Add(enumField);

        // Add a button
        Button button = new Button(() => {
            itemMaker.MakeItem(selectedCategory);
        });

        button.text = "Make Item";

        root.Add(button);

        return root;
    }
}