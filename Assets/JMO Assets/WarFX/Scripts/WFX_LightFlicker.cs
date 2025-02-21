using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class WFX_LightFlicker : MonoBehaviour
{
	public float flickerDuration = 0.1f; // Duration for which the light is on
	private Light muzzleLight;
	private bool isFlickering = false; // Prevent multiple flickers at once

	void Start()
	{
		muzzleLight = GetComponent<Light>();
		muzzleLight.enabled = false; // Ensure the light starts off
	}

	// Call this method to trigger the flicker once
	public void FlickerOnce()
	{
		if (!isFlickering) // Only start if not already flickering
		{
			StartCoroutine(FlickerOnceCoroutine());
		}
	}

	IEnumerator FlickerOnceCoroutine()
	{
		isFlickering = true; // Set flag to prevent multiple flickers
		muzzleLight.enabled = true; // Turn on the light
		yield return new WaitForSeconds(flickerDuration); // Wait for the flicker duration
		muzzleLight.enabled = false; // Turn off the light after the flicker
		isFlickering = false; // Allow future flickers
	}
}
