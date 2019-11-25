using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour {

	public GameObject projectileObject;
	public GameObject projectileGhost;
	public SimulationSettings simulationSettings;
	[HideInInspector]
	public LineRenderer lr;
	public List<Point> points;

	SimValues simValues;
	bool useZAxis;

	[ReadOnly]public float verticalAngle;
	[ReadOnly] public float horizontalAngle;

	[ReadOnly]public float startVelocity;
	[ReadOnly]public float startVelocityX;
	[ReadOnly]public float startVelocityY;
	[ReadOnly] public float startVelocityZ;

	[ReadOnly]public float startHeight;
	[ReadOnly]public float time;
	Vector3 startPosition;
	float floor; // TODO: Custom ERROR (floor higher than startPosition.y)


	float g = 9.82f;

	[HideInInspector]
	public bool settingsFoldout; // For custom editor
	bool simulating = false;

	void Awake() {
		lr = GetComponent<LineRenderer>();
		points = new List<Point>();
	}

	void Start() {
		// Update simulation settings on start in case of changes outside of runtime
		SetSettings();
	}

	public void StartSimulation() {
		if (startPosition.y >= floor) { // Can't start under the floor value, potential fix later
			simulating = true;
			StartCoroutine(SimulateProjectile()); // Begin simulation
		} else
			Debug.Log("WARNING: Floor can't be figher than starting position!");
	}

	IEnumerator SimulateProjectile() {

		// Set velocities and the current time
		float Vx = startVelocityX;
		float Vy = startVelocityY;
		float Vz = startVelocityZ;
		float elapsed_time = 0;

		//Simulate movement
		while (projectileObject.transform.position.y >= floor) {
			Vy = startVelocityY - (g * elapsed_time); // Update Vy
			// Move projectile
			projectileObject.transform.Translate(Vx * Time.deltaTime, Vy * Time.deltaTime, Vz * Time.deltaTime); 

			// Set values for current point
			Vector3 currentPosition = projectileObject.transform.position;
			float xz = Mathf.Sqrt(Vx * Vx + Vz * Vz);
			float currentVelocity = Mathf.Sqrt(xz * xz + Vy * Vy);
			float currentVAngle = Mathf.Asin(Vy / currentVelocity) * Mathf.Rad2Deg;
			float currentHAngle = Mathf.Asin(Vz / xz) * Mathf.Rad2Deg;

			// Create and add the point to the list of points
			Point newPoint = new Point(currentPosition, currentVelocity, currentVAngle, currentHAngle, elapsed_time); // TODO: Göra färdigt
			points.Add(newPoint);

			elapsed_time += Time.deltaTime; // Update current time
			yield return null;
		}
		int newLenght = Mathf.FloorToInt(time * 10); // Shorten down the list (ghost gaps)

		DrawLineRenderer(); // Draw the line renderer arc using all the points positions
		points = ShortenList(points, newLenght); // Shorten down the list to size calculated above
		DrawGhosts(); // Draw the ghosts

		simulating = false; // End simulation
	}

	List<Point> ShortenList(List<Point> list, int newLength) {

		List<Point> shortenedList = new List<Point>(); // The new list
		int c = list.Count / newLength; // On what index's should be saved
		int i = 0; // Index memory
		shortenedList.Add(list[0]);
		foreach (Point p in list) {
			if (i == c) { // If index match saved index
				shortenedList.Add(p); // Save point
				i = 0;
			} else // Else keep going, ignore the current point
				i++;
		}
		shortenedList.Add(list[list.Count - 1]);
		return shortenedList; // Send back the updated shortened list
	}

	void DrawGhosts() {
		// Set every ghost's parent to the script-objects' transform, in order to clear them easily later. 
		// Loop through points
		foreach (Point p in points) {
			// Instantiate ghost on every point
			var newGhost = Instantiate(projectileGhost, p.position, Quaternion.identity); 
			newGhost.transform.parent = gameObject.transform;
		}
	}

	void DrawLineRenderer() {
		// Set size
		lr.startWidth = 0.02f;
		lr.endWidth = 0.02f;

		// Set materials and color
		lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.startColor = Color.black;
		lr.endColor = Color.black;

		// Draw a line through every point
		lr.positionCount = points.Count;
		for (int i = 0; i < points.Count; i++) {
			lr.SetPosition(i, points[i].position);
		}
	}

	public void OnSimulationSettingsUpdated() {
		// Update settings if it's not simulating
		if (!simulating)
			SetSettings();
	}
	void SetSettings() {
		simValues = simulationSettings.simValues; // Get current variation
		useZAxis = simulationSettings.useZAxis;
		switch (simValues) { // Set settings accordingly to the simVariation
			case SimValues.VAlpha: // Based on velocity and firing angle
				verticalAngle = simulationSettings.verticalAngle;
				startVelocity = simulationSettings.startVelocity;
				if (useZAxis) {
					horizontalAngle = simulationSettings.horizontalAngle;
					float Vxz = Mathf.Cos(verticalAngle * Mathf.Deg2Rad) * startVelocity;
					startVelocityX = Mathf.Cos(horizontalAngle * Mathf.Deg2Rad) * Vxz;
					startVelocityZ = Mathf.Sin(horizontalAngle * Mathf.Deg2Rad) * Vxz;
				} else {
					startVelocityX = Mathf.Cos(verticalAngle * Mathf.Deg2Rad) * startVelocity;
					horizontalAngle = 0;
					startVelocityZ = 0;
				}
				startVelocityY = Mathf.Sin(verticalAngle * Mathf.Deg2Rad) * startVelocity;
				break;
			case SimValues.VxAlpha: // Based on X's velocity and firing angle
				verticalAngle = simulationSettings.verticalAngle;
				startVelocityX = simulationSettings.startVelocityX;
				if (useZAxis) {
					startVelocityZ = simulationSettings.startVelocityZ;
					float xz = Mathf.Sqrt(startVelocityX * startVelocityX + startVelocityZ * startVelocityZ);
					startVelocity = xz / Mathf.Cos(verticalAngle * Mathf.Deg2Rad);
					horizontalAngle = Mathf.Asin(startVelocityZ / xz);
				} else {
					startVelocityZ = 0;
					horizontalAngle = 0;
					startVelocity = startVelocityX / Mathf.Cos(verticalAngle * Mathf.Deg2Rad);
				}
				startVelocityY = Mathf.Sin(verticalAngle * Mathf.Deg2Rad) * startVelocity;
				break;
			case SimValues.VyAlpha: // Based on Y's velocity and firing angle
				verticalAngle = simulationSettings.verticalAngle;
				startVelocityY = simulationSettings.startVelocityY;
				startVelocity = startVelocityY / Mathf.Sin(verticalAngle * Mathf.Deg2Rad);
				if (useZAxis) {
					horizontalAngle = simulationSettings.horizontalAngle;
					float xz = Mathf.Cos(verticalAngle * Mathf.Deg2Rad) * startVelocity;
					startVelocityX = Mathf.Cos(horizontalAngle * Mathf.Deg2Rad) * xz;
					startVelocityZ = Mathf.Sin(horizontalAngle * Mathf.Deg2Rad) * xz;
				} else {
					startVelocityX = Mathf.Cos(verticalAngle * Mathf.Deg2Rad) * startVelocity;
					startVelocityZ = 0;
					horizontalAngle = 0;
				}
				break;
			case SimValues.VxVy: // Based on X and Y's velocities
				startVelocityX = simulationSettings.startVelocityX;
				startVelocityY = simulationSettings.startVelocityY;
				if (useZAxis) {
					startVelocityZ = simulationSettings.startVelocityZ;
					float xz = Mathf.Sqrt(startVelocityX * startVelocityX + startVelocityZ * startVelocityZ);
					startVelocity = Mathf.Sqrt(xz * xz + startVelocityY * startVelocityY);
					horizontalAngle = Mathf.Asin(startVelocityZ / xz);
				} else {
					startVelocity = Mathf.Sqrt(startVelocityX * startVelocityX + startVelocityY * startVelocityY);
					startVelocityZ = 0;
					horizontalAngle = 0;
				}
				verticalAngle = Mathf.Asin(startVelocityY / startVelocity) * Mathf.Rad2Deg;
				break;
		}
		startPosition = simulationSettings.startPosition; // Set startposition
		projectileObject.transform.position = startPosition; // Move projectile to startposition
		floor = simulationSettings.floor; // Set floor
		startHeight = startPosition.y - floor; // Set projectiles starting Y, based on the floor
		time = CalcTime(); // Calculated total traveltime
	}

	float CalcTime() {
		float newTime = Mathf.Sqrt(2 * (startVelocityY + startHeight) / g); // Calculate the entire time of simulation
		return newTime;
	}

	public void Reset() {
		// Lock reset while simulating
		if (!simulating) {
			points.Clear(); // Clear points
			lr.positionCount = 0; // Remove line renederer arc
			foreach (Transform child in transform) { // Go through every ghost and destroy them
				GameObject.Destroy(child.gameObject);
			}
			projectileObject.transform.position = simulationSettings.startPosition; // Reset projectile position
		}
	}
}
