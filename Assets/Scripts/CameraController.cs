﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class CameraController : MonoBehaviour {

	public Transform target;
	public float distance = 0.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = .5f;
	public float distanceMax = 15f;

	public float sizeMin = 100f;
	public float sizeMax = 500f;
	public float scrollAcceleration = 1000f;

	float x = 0.0f;
	float y = 0.0f;

	// Use this for initialization
	void Start () 
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}

	void LateUpdate () 
	{
		if (target && Input.GetMouseButton(1)) 
		{
			target.position = new Vector3 (0, 0, 0);

			x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

			y = ClampAngle(y, yMinLimit, yMaxLimit);

			Quaternion rotation = Quaternion.Euler(y, x, 0);

			// Disable scroll wheel
			//distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
			distance = 5f;

			RaycastHit hit;
			if (Physics.Linecast (target.position, transform.position, out hit)) 
			{
				distance -=  hit.distance;
			}
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + (target.position);

			transform.rotation = rotation;
			// Multiply by 100 to prevent orbit path from clipping
			transform.position = position * 100;
		}
		// Scroll in and out with the mouse scroll wheel
		var scroll = Input.GetAxis ("Mouse ScrollWheel");
		if (scroll != 0) {
			// Clamp values if camera is too close or too far away

			// Change the orthographic camera size
			Camera.main.orthographicSize -= (scroll * scrollAcceleration);
			if (Camera.main.orthographicSize < sizeMin) {
				Camera.main.orthographicSize = sizeMin;
			} else if (Camera.main.orthographicSize > sizeMax) {
				Camera.main.orthographicSize = sizeMax;
			}
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		// Prevent camera view from crossing y=0
		if (angle < 10) {
			angle = 10f;
		}

		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}