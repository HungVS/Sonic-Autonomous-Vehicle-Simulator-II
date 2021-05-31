using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System.Linq; // To use 'Enumerable....()'. Reference: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sum?view=net-5.0


// Process GA && NN to training the model.
public class Monitor : MonoBehaviour
{

	private int optimalID;
	public bool didConverge = false; // Converge status.

	[Header("Car")]
	public Car car;

	List<int> genes = new List<int>(); // Contains all selected individuals (networks) (ID storage).

	int numSelected; // Selected to crossover.

	NN[] population;

	[Header("Current generation: ")]
	public int currGeneration; // Track number of generation.
	public int currChromosomes = 0; // Chromosomes = Individuals.

	void Start()
	{
		init();
	}

	void init()
	{
		population = new NN[Settings.POPULATION_SIZE]; // Each individual (chromosome) = 1 NN. 
		initPopulation(population, 0);
		setupCurrDNNModel();
	}

	// Call this function when converges.
	public void doConverge() {
		didConverge = true;
	}

	// Set up the current DNN model for the Car instance.
	void setupCurrDNNModel()
	{
		car.reset(population[didConverge? optimalID : currChromosomes]); // Population = List of NNs.
	}

	/* Initiate population with random values.
	* @param     population     The population to be initiated.
	* @param     i                    Starting position to initiate (Not random all, depends on crossover).
	*/
	void initPopulation(NN[] population, int i)
	{
		while (i < Settings.POPULATION_SIZE)
		{
			population[i] = new NN();
			population[i++].init();
		}
	}


	/* Called by Fitness component of Car when the Car instance quits.
	* @param     fitness     Car instance's fitness.
	* @param     nn			  Car instance's NN.
	*/
	public void quit(float fitness, NN nn)
	{

		if(didConverge) {
			optimalID = currChromosomes - 1;
			setupCurrDNNModel();
			return;
		}

		if (currChromosomes < population.Length - 1) // '-1' because of '++'
		{
			population[currChromosomes++].fitness = fitness;
			setupCurrDNNModel(); // Test the next individual with the next NN. 
		}
		else spawn(); // Generate the next generation.

	}


	// Spawn the next generation.
	void spawn()
	{
		genes.Clear();
		currGeneration++;
		numSelected = 0;

		sort(); // Sort population descending.

		NN[] population = selectPopulation(); // Temporary for the original population.

		crossover(population);
		mutate(population);

		initPopulation(population, numSelected); // Init the remaining population after crossover and mutation.
		this.population = population;
		currChromosomes = 0; // Reset current chromosomes pointer to the beginning.
		setupCurrDNNModel(); // Setup the NN of the first individual of the new population to the Car instance to be able to do the test.
	}

	/* Mutate selected population.
	* @param     selectedPopulation     The selected population for crossover and mutation.
	*/
	void mutate(NN[] selectedPopulation)
	{

		for (int i = 0; i < numSelected; i++) // Only mutate selected individuals.
			for (int c = 0; c < selectedPopulation[i].weights.Count; c++)
				if (Random.Range(0.0f, 1.0f) < Settings.MUTATION_RATE)
					selectedPopulation[i].weights[c] = mutateWeights(selectedPopulation[i].weights[c]); // Weights is a matrix.

	}

	/* Mutate some value on weights matrix.
	* @param     weights     The weights matrix.
	*/
	Matrix<float> mutateWeights(Matrix<float> weights)
	{
		Matrix<float> t = weights;

		for (int i = 0; i < Random.Range(1, (weights.RowCount * weights.ColumnCount) / Settings.MUTATION_DIVITION_RATE); i++)
		{
			int randCol = Random.Range(0, t.ColumnCount);
			int randRow = Random.Range(0, t.RowCount);

			t[randRow, randCol] = Mathf.Clamp(t[randRow, randCol] + Random.Range(-1.0f, 1.0f), -1.0f, 1.0f); // Only allow weights between [-1; 1].
		}

		return t;

	}

	// Initiate IDs of individuals on population for crossover.
	int[] initIDsForCrossover()
	{
		int[] IDs = new int[2];
		if (genes.Count >= 1)
		{
			for (int i = 0; i < Settings.POPULATION_SIZE; i++) // Settings.POPULATION_SIZE in this case is 'just' a random choice.
			{
				for (int o = 0; o < 2; o++)
					IDs[o] = genes[Random.Range(0, genes.Count)];
				if (IDs[0] != IDs[1]) break;
			}
		}
		else Debug.Log("[WARNING]: Genes pool size = 0 - This only happens when all individual have '0' fitness value.");
		return IDs;
	}

	// Initiate childs for crossover.
	NN[] initChildsForCrossover()
	{
		NN[] childs = new NN[2];
		foreach (var c in childs)
		{
			c.init();
			c.fitness = 0;
		}
		return childs;
	}

	/* Do crossover over the selected population.
	* @param     selectedPopulation     The selected population.
	*/
	void crossover(NN[] selectedPopulation)
	{
		for (int o = 0; o < Settings.NUM_CROSSOVER; o += 2) // '2': Crossover must be done with 2 individuals.
		{
			int[] IDs = initIDsForCrossover();
			NN[] childs = initChildsForCrossover(); // Each parents create 2 childs.

			for (int i = 0; i < childs[0].weights.Count; i++)
			{

				if (Random.Range(0.0f, 1.0f) < 0.5f) // 50%.
				{
					childs[0].weights[i] = population[IDs[0]].weights[i];
					childs[1].weights[i] = population[IDs[1]].weights[i];
				}
				else
				{
					childs[1].weights[i] = population[IDs[0]].weights[i];
					childs[0].weights[i] = population[IDs[1]].weights[i];
				}

			}


			for (int w = 0; w < childs[0].biases.Count; w++)
			{

				if (Random.Range(0.0f, 1.0f) < 0.5f)
				{
					childs[0].biases[w] = population[IDs[0]].biases[w];
					childs[1].biases[w] = population[IDs[1]].biases[w];
				}
				else
				{
					childs[1].biases[w] = population[IDs[0]].biases[w];
					childs[0].biases[w] = population[IDs[1]].biases[w];
				}

			}

			foreach (var c in childs)
				selectedPopulation[++numSelected] = c; // Add childs to the temporary next population.
		}
	}


	/* Select population for crossover and mutation.
	* @return     Population
	*/
	NN[] selectPopulation()
	{

		NN[] selectedPopulation = new NN[Settings.POPULATION_SIZE];

		for (int i = 0; i < Settings.NUM_BEST_SELECTION; i++)
		{
			selectedPopulation[numSelected] = this.population[i].clone(); // Avoid 2 references point to the same memory space. selectedPopulation'll be change in crossover, mutation,...
			selectedPopulation[numSelected].fitness = 0;
			numSelected++;

			int numReplicas = (int)Mathf.Round(this.population[i].fitness * Settings.GENE_POOL_REPLICAS_RATE); // Individual with higher fitness takes longer range.

			for (int j = 0; j < numReplicas; j++)
				genes.Add(i);

		}

		for (int i = 0; i < Settings.NUM_WORST_SELECTION; i++)
		{
			int lastID = population.Count() - 1 - i;

			int f = (int)Mathf.Round(this.population[lastID].fitness * Settings.GENE_POOL_REPLICAS_RATE);

			for (int j = 0; j < f; j++)
			{
				genes.Add(lastID);
			}

		}
		return selectedPopulation;
	}

	// Sort individual with respect to fitness: Higher fitness has 
	void sort() // TODO: Use better sort.
	{
		for (int i = 0; i < population.Length; i++)
			for (int j = i; j < population.Length; j++)
				if (population[i].fitness < population[j].fitness)
				{
					NN temp = population[i];
					population[i] = population[j];
					population[j] = temp;
				}
	}
}
