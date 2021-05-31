using UnityEngine;

public class Controller : MonoBehaviour
{
	/* Move the Car instance.
	* @param     acc     Acceleration.
	* @param     steering     Steering.
	*/
	public void move(float acc, float steering)
	{
		Vector3 delta = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, acc * 11.5f), 0.02f);
		delta = transform.TransformDirection(delta);
		transform.position += delta;
		transform.eulerAngles += new Vector3(0, (steering * 90) * 0.02f, 0);
	}
}
