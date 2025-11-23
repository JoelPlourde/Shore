using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Creature))]
public class CreatureEditor : Editor {
    public override void OnInspectorGUI() {
        Creature creature = (Creature)target;

        EditorGUILayout.LabelField("Combat", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Max Health: " + creature.MaxHealth.ToString());
        EditorGUILayout.LabelField("Health: " + creature.Health.ToString());
        EditorGUILayout.LabelField("Damage: " + creature.Damage.ToString());
        EditorGUILayout.LabelField("Attack Range: " + creature.AttackRange.ToString());
        EditorGUILayout.LabelField("Attack Speed: " + creature.AttackSpeed.ToString());
        EditorGUILayout.LabelField("In Combat: " + creature.InCombat.ToString());
        EditorGUILayout.LabelField("Timeout Duration: " + creature.TimeoutDuration.ToString("F2") + " seconds");
        EditorGUILayout.LabelField("Combat Targets: " + creature.CombatTargets.Count.ToString());
        // List all of the combat targets
        foreach (var target in creature.CombatTargets)
        {
            EditorGUILayout.LabelField(" - " + target.name);
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        base.OnInspectorGUI();
    }
}