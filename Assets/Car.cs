using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // To use 'Enumerable....()'. Reference: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sum?view=net-5.0

/* A Car instance needs: 
* Controller: Acceleration, steering.
* LiDAR: Sensors. 
* NN: Neural Network.
* Fitness: Fitness evaluation. */
[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(NN))]
[RequireComponent(typeof(LiDAR))]
[RequireComponent(typeof(Fitness))]
public class Car : MonoBehaviour
{

	[Header("Controller: ")]
	public Controller controller;

	[Header("LiDAR: ")]
	public LiDAR liDAR;

	[Header("Fitness service: ")]
	public Fitness fitness;

	[Header("Deep NN: ")]
	NN nn;
	float acc, steering; // acc: Acceleration.





	void Awake()
	{
		fitness.initPos = transform.position;
		fitness.initRot = transform.eulerAngles;
		nn = GetComponent<NN>();
		fitness.nn = nn;
	}

	/* Reset Car intance. 
	* nn: New DNN model to use. */
	public void reset(NN nn)
	{
		this.nn = nn; // Assign new DNN model for Car instance.
		fitness.reset();
	}

	void OnCollisionEnter(Collision collision)
	{
		fitness.quit();
	}

	void FixedUpdate()
	{
		liDAR.processSensors();
		fitness.lastPos = transform.position;
		(acc, steering) = nn.doForwardPropagation(liDAR.sensors);
		controller.move(acc, steering);
		fitness.runningTime += Time.deltaTime; // Time/frame.
		fitness.process(liDAR);
	}
}
