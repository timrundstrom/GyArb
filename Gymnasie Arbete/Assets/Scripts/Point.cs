using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {
	// Recording a projectiles values at point in time

	public Vector3 position;
	public Vector3 velocities;
    public float velocity;
	public float verticalAngle;
	public float horizontalAngle;
	public float time;
	public float x;
	public float y;
	public string text;
    int decimals = 1;
	
	public Point(Vector3 position, Vector3 velocities, float speed, float time, float x, float y)
	{
		this.position = position;
		this.velocities = velocities;
        velocities.x = Mathf.Abs(velocities.x);
        velocities.z = Mathf.Abs(velocities.z);
		this.time = time;
		this.x = x;
		this.y = y;

		float xz = Mathf.Sqrt(velocities.x * velocities.x + velocities.z * velocities.z);
		float xy = Mathf.Sqrt(velocities.x * velocities.x + velocities.y * velocities.y);
		verticalAngle = Mathf.Atan(velocities.y / xz) * Mathf.Rad2Deg;
		horizontalAngle = Mathf.Atan(velocities.z / xy) * Mathf.Rad2Deg;
        
		text = $"Total speed: {Math.Round(speed, decimals)}m/s\nSpeeds: {Math.Round(velocities.x, decimals)}m/s : {Math.Round(velocities.y, decimals)}m/s : " +
            $"{Math.Round(velocities.z, decimals)}m/s\nX: {Math.Round(x, decimals)}m\nY: {Math.Round(y, decimals)}m\nTime: {Math.Round(time, decimals)}s";

	}
	public Point(Vector3 position, Vector3 velocities, float time) {
		this.position = position;
		this.velocities = velocities;
		this.time = time;

		float xz = Mathf.Sqrt(velocities.x * velocities.x + velocities.z * velocities.z);
		float xy = Mathf.Sqrt(velocities.x * velocities.x + velocities.y * velocities.y);
		verticalAngle = Mathf.Atan(velocities.y / xz) * Mathf.Rad2Deg;
		horizontalAngle = Mathf.Atan(velocities.z / xy) * Mathf.Rad2Deg;

		text = $"Speeds: {velocities.x}-{velocities.y}-{velocities.z}\nTime: {time}";
	}

}
