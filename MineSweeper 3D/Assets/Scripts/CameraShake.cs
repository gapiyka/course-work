using UnityEngine;

public class CameraShake : MonoBehaviour
{
	#region Attributes
	public float shakeDuration = 0f; // How long the object should shake for.

	// Amplitude of the shake. A larger value shakes the camera harder.
	private const float shakeAmount = 0.6f;
	private const float decreaseFactor = 1.0f;

	private Vector3 originalPos;
	#endregion

	void OnEnable()
	{
		originalPos = this.transform.localPosition;
	}

	public void StartShaking()
    {
		originalPos = this.transform.localPosition;
	}

	void Update()
	{
		if (shakeDuration > 0)
		{

			this.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
	}
}