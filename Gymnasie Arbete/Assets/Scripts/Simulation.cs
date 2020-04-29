using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enter values and a press a button for a simulated throw.
/// Works but code is a mess.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Simulation : MonoBehaviour
{

	LineRenderer lr;
	List<Point> points = new List<Point>();
	List<Vector3> ghosts = new List<Vector3>();
	public GameObject ghost;

	[HideInInspector]
	public bool statsFoldout = true;

	[HideInInspector]
	public Vector3 startVelocities;
	[HideInInspector]
	public bool useZAxis = false;

	[ReadOnly]
	public float startVelocity;
	[ReadOnly]
	public float verticalAngle;
	[ReadOnly]
	public float horizontalAngle;

	float g = 9.82f;
	bool simulating = false;
	Vector3 startposition;
	

	void Awake()
	{
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 0;

		startposition = transform.position;

	}

	public void OnSimulationSettingsUpdated()
	{
		if (useZAxis)
		{
			float xy = Mathf.Sqrt(startVelocities.x * startVelocities.x + startVelocities.y * startVelocities.y);
			float xz = Mathf.Sqrt(startVelocities.x * startVelocities.x + startVelocities.z * startVelocities.z);

			startVelocity = Mathf.Sqrt(xz * xz + xy * xy);
			verticalAngle = Mathf.Asin(startVelocities.y / xy) * Mathf.Rad2Deg;
			horizontalAngle = Mathf.Asin(startVelocities.z / xz) * Mathf.Rad2Deg;

			if (startVelocities.x == 0 && startVelocities.y == 0)
				verticalAngle = 0;
			if (startVelocities.x == 0 && startVelocities.z == 0)
				horizontalAngle = 0;

		}
		else
		{
			startVelocity = Mathf.Sqrt(startVelocities.x * startVelocities.x + startVelocities.y * startVelocities.y);
			verticalAngle = Mathf.Asin(startVelocities.y / startVelocity) * Mathf.Rad2Deg;
			startVelocities.z = 0;
			horizontalAngle = 0;

			if (startVelocities.x == 0 && startVelocities.y == 0)
				verticalAngle = 0;

		}
		//verticalAngle = Mathf.Asin(startVelocities.y / startVelocity) * Mathf.Rad2Deg;

	}

	public void StartSimulation()
	{
		if (transform.position.y > 0)
		{ // Can't start under the floor value, potential fix later
			simulating = true;
			StartCoroutine(SimulateProjectile()); // Begin simulation
		}
		else
			Debug.Log("WARNING: Starting position has to be above ground.");
	}

	IEnumerator SimulateProjectile()
	{
		startposition = transform.position;
		float Vx = startVelocities.x;
		float Vy = startVelocities.y;
		float Vz = startVelocities.z;
		float elapsed_time = 0;
		float distance = 0;

		ghosts.Add(transform.position);

		while (gameObject.transform.position.y > 0)
		{
			Vector3 oldPosition = transform.position;
			Vy = startVelocities.y - (g * elapsed_time); // Update Vy
			transform.Translate(Vx * Time.deltaTime, Vy * Time.deltaTime, Vz * Time.deltaTime); // Move projectile

			// Create and add the point to the list of points
			if (distance > 1)
			{
				ghosts.Add(transform.position);
				distance = 0;
			}

			Vector3 velocities = new Vector3(Vx, Vy, Vz);
			Point newPoint = new Point(transform.position, velocities, elapsed_time);
			points.Add(newPoint);

			elapsed_time += Time.deltaTime; // Update current time
			distance += Vector3.Distance(oldPosition, transform.position);
			yield return null;
		}

		// Last ghost and gameobject is too close to eachother
		if (Vector3.Distance(ghosts[ghosts.Count - 1], transform.position) < .5f)
			ghosts.RemoveAt(ghosts.Count - 1);

		DrawLineRenderer(); // Draw the line renderer arc using all the points positions
							//points = ShortenList(points, Convert.ToInt32(distance)); // Shorten down the list to size calculated above
		DrawGhosts(); // Draw the ghosts
		simulating = false; // End simulation
	}

	void DrawLineRenderer()
	{
		// Set size
		lr.startWidth = 0.02f;
		lr.endWidth = 0.02f;

		// Set materials and color
		lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.startColor = new Color(0, 0, 0, .7f);
		lr.endColor = new Color(0, 0, 0, .7f);

		// Draw a line through every point
		lr.positionCount = points.Count;
		for (int i = 0; i < points.Count; i++)
		{
			lr.SetPosition(i, points[i].position);
		}
	}

	List<Point> ShortenList(List<Point> list, int newLength)
	{

		List<Point> shortenedList = new List<Point>(); // The new list
		int c = list.Count / newLength; // On what index's should be saved
		int i = 0; // Index memory
		shortenedList.Add(list[0]);
		foreach (Point p in list)
		{
			if (i == c)
			{ // If index match saved index
				shortenedList.Add(p); // Save point
				i = 0;
			}
			else // Else keep going, ignore the current point
				i++;
		}
		shortenedList.Add(list[list.Count - 1]);
		return shortenedList; // Send back the updated shortened list
	}

	void DrawGhosts()
	{
		// Set every ghost's parent to the script-objects' transform, in order to clear them easily later. 
		// Loop through points
		foreach (Vector3 position in ghosts)
		{
			// Instantiate ghost on every point
			var newGhost = Instantiate(ghost, position, Quaternion.identity);
			newGhost.transform.parent = gameObject.transform;
			//GUI.Label(new Rect(newGhost.transform.position, new Vector2(50, 50)), "Helo");
		}
	}

	public void Reset()
	{
		// Lock reset while simulating
		if (!simulating)
		{
			points.Clear(); // Clear points
			ghosts.Clear();
			lr.positionCount = 0; // Remove line renederer arc
			foreach (Transform child in transform)
			{ // Go through every ghost and destroy them
				GameObject.Destroy(child.gameObject);
			}
			transform.position = startposition; // Reset projectile position
		}
	}
}
