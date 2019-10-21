using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SimulationSettings : ScriptableObject {
	// Settings for projectile simulation with 2 dimensions.

	[HideInInspector]
	public bool useZAxis = false;

	[HideInInspector]
	public SimValues simValues;

	[HideInInspector]
	public float verticalAngle;
	[HideInInspector]
	public float horizontalAngle;
	[HideInInspector]

	public float startVelocity;
	[HideInInspector]
	public float startVelocityX;
	[HideInInspector]
	public float startVelocityY;
	[HideInInspector]
	public float startVelocityZ;

	public Vector3 startPosition;
	public float floor = 0f;

}
