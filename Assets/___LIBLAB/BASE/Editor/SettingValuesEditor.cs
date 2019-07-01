using System.Collections;
using UnityEditor;
using UnityEngine;

namespace LibLabGames.SpeedBetRacing
{
    [CustomEditor(typeof(SettingValues))]
    public class SettingValuesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (SettingValues) target;

            if (GUILayout.Button("Update Values by CSV"))
            {
                script.UpdateValuesByCSV();
            }
        }
    }
}