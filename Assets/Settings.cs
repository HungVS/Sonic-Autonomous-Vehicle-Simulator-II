using UnityEngine;

public class Settings : MonoBehaviour {

	public static int NUM_SIDE_SENSORS = 12; // Number of side sensors. Total sensors = 12 + 12 + 1.
	public static int HIDDEN_SIZE = 13; // Number of nodes in each Hidden layer.
	public static int NUM_HIDDENS = 2; // Number of Hidden layers.
	public static int OUTPUT_SIZE = 2; // Number of nodes in Output layer: Acceleration & steering.
	public static float DISTANCE_WEIGHT = 1.5f;
	public static float AVERAGE_SPEED_WEIGHT = 0.2f;
	public static float SENSOR_WEIGHT = 0.2f;
	public static int TIMEOUT_FITNESS = 500; // Some bugs make the Car instance too far from the track.

	// TIME_UNDER_BOUND & FITNESS_UPPER_BOUND detecs infinite loops.
	public static int TIME_UNDER_BOUND = 10; 
	public static int FITNESS_UPPER_BOUND = 15;
	public static int POPULATION_SIZE = 1000;
	public static float MUTATION_RATE = 0.01f;
	public static int MUTATION_DIVITION_RATE = 7; // 'Just' a random number.
	public static int NUM_BEST_SELECTION = 20;
	public static int NUM_WORST_SELECTION = 0;
	public static int NUM_CROSSOVER = 20; // Does not need to equal population. The remaining is random.
	public static int GENE_POOL_REPLICAS_RATE = 10; // E.g., [1 1 ... 1 2 2 ...2 ...]
}
