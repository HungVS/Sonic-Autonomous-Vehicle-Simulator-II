using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System;

using Random = UnityEngine.Random;
using System.Linq; // To use 'Enumerable....()'. Reference: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sum?view=net-5.0

public class NN : MonoBehaviour
{
	public Matrix<float> input = Matrix<float>.Build.Dense(1, Settings.NUM_SIDE_SENSORS * 2 + 1);
	public List<Matrix<float>> hiddens = new List<Matrix<float>>(); // 2D matrixes activation values.
	public Matrix<float> output = Matrix<float>.Build.Dense(1, Settings.OUTPUT_SIZE);
	public List<Matrix<float>> weights = new List<Matrix<float>>();
	public List<float> biases = new List<float>();

	public float fitness;

	public void init()
	{
		clean();
		initHidden();
		initOutput();
		initWeights();

	}

	// Allocate space memories to activations, weights and initiate Bias of Hiddens.
	void initHidden()
	{
		for (int i = 0; i < Settings.NUM_HIDDENS; i++)
		{
			hiddens.Add(Matrix<float>.Build.Dense(1, Settings.HIDDEN_SIZE)); // Allocate space memories for Hidden (i).
			initBias(); // Initiate Bias for Hidden (i).
			if (i == 0) weights.Add(Matrix<float>.Build.Dense(Settings.NUM_SIDE_SENSORS * 2 + 1, Settings.HIDDEN_SIZE)); // The first Hidden.
			else weights.Add(Matrix<float>.Build.Dense(Settings.HIDDEN_SIZE, Settings.HIDDEN_SIZE));
		}
	}

	// Allocate space memories to weights and initiate Bias of Output.
	void initOutput()
	{
		weights.Add(Matrix<float>.Build.Dense(Settings.HIDDEN_SIZE, Settings.OUTPUT_SIZE));
		initBias();
	}

	// Initiate random Bias.
	void initBias()
	{
		biases.Add(getRandom());
	}

	// Clean all matrixes.
	void clean()
	{
		input.Clear();
		hiddens.Clear();
		output.Clear();
		weights.Clear();
		biases.Clear();
	}

	/* Clone this instance and return that replica.
	* @return     The replica.
	*/
	public NN clone()
	{
		NN nn = new NN();

		List<Matrix<float>> newWeights = new List<Matrix<float>>();

		for (int a = 0; a < this.weights.Count; a++)
		{
			Matrix<float> currWeights = Matrix<float>.Build.Dense(weights[a].RowCount, weights[a].ColumnCount);
			for (int b = 0; b < currWeights.RowCount; b++)
				for (int c = 0; c < currWeights.ColumnCount; c++)
					currWeights[b, c] = weights[a][b, c];
			newWeights.Add(currWeights);
		}

		List<float> newBiases = new List<float>();

		newBiases.AddRange(biases);

		nn.weights = newWeights;
		nn.biases = newBiases;

		nn.initOnlyHidden();

		return nn;
	}

	// Initiate only Hidden, used to clone.
	void initOnlyHidden()
	{
		cleanActivation();
		for (int i = 0; i < Settings.NUM_HIDDENS + 1; i++)
			this.hiddens.Add((Matrix<float>.Build.Dense(1, Settings.HIDDEN_SIZE)));
	}

	// Clean activation of this NN instance.
	void cleanActivation()
	{
		input.Clear();
		hiddens.Clear();
		output.Clear();
	}

	// Initiate random weights.
	void initWeights()
	{
		for (int a = 0; a < weights.Count; a++)
			for (int b = 0; b < weights[a].RowCount; b++)
				for (int c = 0; c < weights[a].ColumnCount; c++)
					weights[a][b, c] = getRandom();
	}

	// Initiate values for weights & Bias.
	float getRandom()
	{
		return Random.Range(-1.0f, 1.0f);
	}

	// Sigmoid activation.
	 float Sigmoid(float s)
	{
		return (1 / (1 + Mathf.Exp(-s)));
	}

	// Do forward propagation to calculate outputs: Acceleration and Steering.
	public (float, float) doForwardPropagation(float[] input)
	{
		for (int i = 0; i < input.Count(); i++)
			this.input[0, i] = input[i];

		this.input = this.input.PointwiseTanh();
		hiddens[0] = ((this.input * weights[0]) + biases[0]).PointwiseTanh(); // Tanh range: [-1; 1]. Acceleration: [0;1]. Steering: [-1; 1]. Pointwise is element-wise. Applies tanh for each value of the vector.

		for (int i = 1; i < hiddens.Count; i++)
			hiddens[i] = ((hiddens[i - 1] * weights[i]) + biases[i]).PointwiseTanh();

		output = ((hiddens[hiddens.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh(); // '-1': 0-based index.

		return (Sigmoid(output[0, 0]), output[0, 1]); // output[0,1] is already between [-1;1].
	}
}
