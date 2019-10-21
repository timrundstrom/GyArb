using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Simulation))]
public class SimulationEditor : Editor {

	Simulation simulation;
	Editor settingsEditor;
	SimulationSettings settings;
	public SimValues simulationType;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI(); // Draw base gui
		simulation = (Simulation)target; // Set simulation
		settings = simulation.simulationSettings; // Set settings

		// Start and Reset buttons
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Start Simulation")) {
			simulation.Reset();
			simulation.StartSimulation();
		}
		if (GUILayout.Button("Reset"))
			simulation.Reset();
		EditorGUILayout.EndHorizontal();

		// Display the settings
		DisplaySettingsEditor(ref simulation.settingsFoldout, ref settingsEditor);
	}

	void DisplaySettingsEditor(ref bool foldout, ref Editor editor) {

		if (settings != null) { // Settings has to exist
			foldout = EditorGUILayout.InspectorTitlebar(foldout, settings); // Fancy foldout
			using (var check = new EditorGUI.ChangeCheckScope()) { // Check if any changes has been made
				if (foldout) {

					CreateCachedEditor(settings, null, ref editor); // Only create a new editor if needed
					DisplaySimulationValues(); // Display the settings according to simulation type / variation
					editor.OnInspectorGUI();
					if (check.changed) // If something has changed, update settings
						simulation.OnSimulationSettingsUpdated();
				}
			}
		}
	}
	
	void DisplaySimulationValues() {
		//Display the needed values of the selected simulation type / variation

		simulationType = (SimValues)EditorGUILayout.EnumPopup("Simulation Values", simulationType); // Choose simulationtype
		settings.useZAxis = EditorGUILayout.Toggle("Use Z Axis", settings.useZAxis);
		switch (simulationType) {
			case SimValues.VAlpha: // Using velocity and firing angle
				settings.verticalAngle = EditorGUILayout.FloatField("Vertical Angle", settings.verticalAngle);
				if (settings.useZAxis)
					settings.horizontalAngle = EditorGUILayout.FloatField("Horizontal Angle", settings.horizontalAngle);
				settings.simValues = SimValues.VAlpha;
				settings.startVelocity = EditorGUILayout.FloatField("Starting Velocity", settings.startVelocity);
				break;
			case SimValues.VxAlpha: // Using X's velocity and firing angle
				settings.simValues = SimValues.VxAlpha;
				settings.verticalAngle = EditorGUILayout.FloatField("Vertical Angle", settings.verticalAngle);
				settings.startVelocityX = EditorGUILayout.FloatField("Starting X Velocity", settings.startVelocityX);
				if (settings.useZAxis)
					settings.startVelocityZ = EditorGUILayout.FloatField("Starting Z Velocity", settings.startVelocityZ);
				break;
			case SimValues.VyAlpha: // Using Y's velocity and firing angle
				settings.simValues = SimValues.VxAlpha;
				settings.verticalAngle = EditorGUILayout.FloatField("Vertical Angle", settings.verticalAngle);
				if (settings.useZAxis)
					settings.horizontalAngle = EditorGUILayout.FloatField("Horizontal Angle", settings.horizontalAngle);
				settings.startVelocityY = EditorGUILayout.FloatField("Starting Y Velocity", settings.startVelocityY);
				break;
			case SimValues.VxVy: // Using X and Y's velocities
				settings.simValues = SimValues.VxVy;
				settings.startVelocityX = EditorGUILayout.FloatField("Starting X Velocity", settings.startVelocityX);
				settings.startVelocityY = EditorGUILayout.FloatField("Starting Y Velocity", settings.startVelocityY);
				if (settings.useZAxis)
					settings.startVelocityZ = EditorGUILayout.FloatField("Starting Z Velocity", settings.startVelocityZ);
				break;
		}
	}

}
