using UnityEngine;

using System.Linq; // To use 'Enumerable....()'. Reference: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sum?view=net-5.0

/* Fitness is one of Car's component.
* Calculate fitness of the Car instance.
*/
public class Fitness : MonoBehaviour
{

	float fitness; // Fitness value.
	float dis; // Distance.
	float aSpeed; // Average speed.
	public float runningTime = 0; // Running time.

	/* Initiated position
	* initPos: Initiated position.
	* initRot: Initiated rotation. 
	* lastPos: Last position. */
	public Vector3 initPos, initRot, lastPos;

	public NN nn; // Deep NN component.





	// Reset when crashing.
	public void reset()
	{
		lastPos = initPos;// ?
		resetFitness();
		resetPos();
	}

	// Reset fitness-related values.
	void resetFitness()
	{
		fitness = 0;
		dis = 0;
		aSpeed = 0;
		runningTime = 0;
	}

	// Reset position & rotation.
	void resetPos()
	{
		transform.position = initPos;
		transform.eulerAngles = initRot;
	}


	/* Calculate fitness.
	* @param     liDAR     LiDAR component of Car instance.	
	*/
	public void process(LiDAR liDAR)
	{
		calculateDis();
		calculateASpeed();
		calculateFitness(liDAR);
		processTimeout();
		processLoop();
	}

	/* Calculate fitness: 
	* - liDAR.sensors.Min(): Keep the Car instance in the middle of the track.
	*/
	void calculateFitness(LiDAR liDAR)
	{
		fitness = liDAR.sensors.Min() * Settings.SENSOR_WEIGHT + Settings.DISTANCE_WEIGHT * dis + Settings.AVERAGE_SPEED_WEIGHT * aSpeed;
	}

	// Sum distances changes on each frames (Could cause infinite loops)
	void calculateDis()
	{
		dis += Vector3.Distance(transform.position, lastPos);
	}

	// Calculate average speed.
	void calculateASpeed()
	{
		aSpeed = dis / runningTime;
	}

	// Some bugs make the Car instance too far from the track.
	void processTimeout()
	{
		if (fitness >= Settings.TIMEOUT_FITNESS) quit();
	}

	// Process infinite loops.
	void processLoop()
	{
		if (runningTime > Settings.TIME_UNDER_BOUND && fitness < Settings.FITNESS_UPPER_BOUND) quit();
	}

	public void quit()
	{
		GameObject.FindObjectOfType<Monitor>().quit(fitness, nn);
	}

}
