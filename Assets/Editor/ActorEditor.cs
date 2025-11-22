using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Actor))]
public class ActorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Actor actor = (Actor)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Health Regeneration (/5s):" + actor.Attributes.HealthRegeneration.ToString());
        EditorGUILayout.LabelField("Speed: " + actor.Attributes.Speed.ToString());
        EditorGUILayout.LabelField("Hunger Rate (/5s): " + actor.Attributes.HungerRate.ToString());
        EditorGUILayout.LabelField("Food: " + actor.Attributes.Food.ToString());
        EditorGUILayout.LabelField("Temperature: " + actor.Attributes.Temperature.ToString());
        EditorGUILayout.Space();

        // Add an horizontal line
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Dead: " + actor.Status.Dead.ToString());
        EditorGUILayout.LabelField("Stunned: " + actor.Status.Stunned.ToString());
        EditorGUILayout.LabelField("Fleeing: " + actor.Status.Fleeing.ToString());
        EditorGUILayout.LabelField("Sheltered: " + actor.Status.Sheltered.ToString());
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }
}