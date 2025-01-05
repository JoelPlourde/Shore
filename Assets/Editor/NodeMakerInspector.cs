using Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Tools.NodeMaker;

[CustomEditor(typeof(NodeMaker))]
public class NodeMakerInspector : Editor {
    
    public override VisualElement CreateInspectorGUI() {
        NodeMaker nodeMaker = (NodeMaker)target;

        VisualElement root = new VisualElement();

        // Add a text field
        string nodeKey = nodeMaker.gameObject.name;
        TextField textField = new TextField("Node Key");
        textField.RegisterValueChangedCallback((evt) => {
            nodeKey = evt.newValue.Replace(" ", "_").ToLower();
        });
        root.Add(textField);

        // Add a dropdown
        NodeCategory selectedCategory = NodeCategory.ORES;
        EnumField enumField = new EnumField("Node Category", NodeCategory.ORES);
        enumField.RegisterValueChangedCallback((evt) => {
            selectedCategory = (NodeCategory)evt.newValue;
        });

        root.Add(enumField);

        // Add a button
        Button button = new Button(() => {
            nodeMaker.MakeNode(nodeKey, selectedCategory);
        });

        button.text = "Make Node";

        root.Add(button);

        return root;
    }
}
