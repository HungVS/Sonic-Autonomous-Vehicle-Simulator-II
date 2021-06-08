using UnityEngine;
using System.Linq; // To use 'Enumerable....()'. Reference: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sum?view=net-5.0

public class LiDAR : MonoBehaviour
{


	public static int NUM_SENSORS = Settings.NUM_SIDE_SENSORS * 2 + 1;
	public float[] sensors = new float[NUM_SENSORS]; // [WARNING]: "(non-sense error) Index was out of bounds of array?". Solution: Rename.
	public int[] x = new int[NUM_SENSORS];

	// Process sensors to track the environment.
	public void processSensors()
	{

	Vector3 rPos = new Vector3(transform.position.x, transform.position.y - 1.2f, transform.position.z);
	//	Vector3 rPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		Ray ray = new Ray(rPos, transform.forward);	
		RaycastHit raycastHit;	

		for (int i = -Settings.NUM_SIDE_SENSORS; i <= Settings.NUM_SIDE_SENSORS; i++)
		{
			ray.direction = transform.forward + i * transform.right;
			if (Physics.Raycast(ray, out raycastHit))
			{
				sensors[i + Settings.NUM_SIDE_SENSORS] = raycastHit.distance / 20; // ? /20
				Debug.DrawLine(ray.origin, raycastHit.point, Color.green);
			}
		}
	}

}
