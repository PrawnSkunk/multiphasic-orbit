﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorUI : MonoBehaviour {

	private float speed = 0.3f; // Closer to zero = faster
	private float bounceDistance = 2f; // Pixels to bounce left

	private float lastX = 0;
	private float lastY = 0;
	private float addX = 0;
	private float addY = 0;
		
	IEnumerator Start () {
		while (true) {
			Vector3 pointA = transform.position;
			Vector3 pointB = pointA + new Vector3(-bounceDistance, 0, 0);

			yield return StartCoroutine(MoveObject(transform, pointA, pointB, speed));
			yield return StartCoroutine(MoveObject(transform, pointB, pointA, speed));
		}
	}

	IEnumerator MoveObject (Transform thisTransform, Vector3 startPos, Vector3 endPos, float time) {
		float i = 0.0f;
		float rate = 1.0f / time;
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			thisTransform.position = Vector3.Lerp(startPos, endPos, i);
			yield return null;
		}
	}
}
