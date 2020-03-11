using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Simulation))]
public class SimulationEditor : Editor
{

	Simulation simulation;
	Editor editor;

	public override void OnInspectorGUI()
	{

		simulation = (Simulation)target; // Set simulation

		DisplayBase(ref simulation.statsFoldout, ref editor);

		// Start and Reset buttons
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Start Simulation"))
		{
			simulation.Reset();
			simulation.StartSimulation();
		}
		if (GUILayout.Button("Reset"))
		{
			simulation.Reset();
		}
		EditorGUILayout.EndHorizontal();

	}

	void DisplayBase(ref bool foldout, ref Editor editor)
	{
		foldout = EditorGUILayout.InspectorTitlebar(foldout, simulation); // Fancy foldout
		using (var check = new EditorGUI.ChangeCheckScope())
		{ // Check if any changes has been made
			if (foldout)
			{
				base.OnInspectorGUI();
				simulation.useZAxis = EditorGUILayout.Toggle("Use Z Axis", simulation.useZAxis);

				simulation.startVelocities.x = EditorGUILayout.FloatField("Start X Velocity", simulation.startVelocities.x);
				simulation.startVelocities.y = EditorGUILayout.FloatField("Start Y Velocity", simulation.startVelocities.y);
				if (simulation.useZAxis)
					simulation.startVelocities.z = EditorGUILayout.FloatField("Start Z Velocity", simulation.startVelocities.z);
				if (check.changed)
					simulation.OnSimulationSettingsUpdated();
			}
		}
	}



}
