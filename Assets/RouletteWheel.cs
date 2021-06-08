using UnityEngine;
using System.Linq; // Emunerable APIs.  Reference: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sum?view=net-5.0

public class RouletteWheel : MonoBehaviour, ISelection{

	/* Start selection.
	* @param     fitnesses     Contains all fitness values of the population. 
	* @param     selectionRate     Selection rate.
	*/
	public int[] process(float[] fitnesses, float selectionRate) {
		float[] cumulativeProbabilities = calculateCumulativeProbabilities(fitnesses);
		int[] selectedIndexes = select(cumulativeProbabilities, selectionRate);
		return selectedIndexes;
	}

	float[] calculateCumulativeProbabilities(float[] fitnesses) {
		
		int populationSize = fitnesses.Count();
		float[] cumulativeProbabilities = new float[populationSize];
		float sum = fitnesses.Sum();
		cumulativeProbabilities[0]  = fitnesses[0]/sum;
		for(int i = 1; i < populationSize; i++)
			cumulativeProbabilities[i] = cumulativeProbabilities[i-1] + fitnesses[i]/sum;
		return cumulativeProbabilities;
	}

	int[] select(float[] cumulativeProbabilities, float selectionRate) {

		int populationSize = cumulativeProbabilities.Count();
		int numToSelect = Mathf.RoundToInt(selectionRate*populationSize);
		int[] selectedIndexes = new int[numToSelect];
		for(int i = 0; i < numToSelect; i++) {
			float random = Random.Range(0f,1f);

			if(random <= cumulativeProbabilities[0]) {
				selectedIndexes[i] = 0;
				continue;
			}
			
			for(int c = 1; c < populationSize; c++) {
				if(cumulativeProbabilities[c-1] < random && random <= cumulativeProbabilities[c])
					selectedIndexes[i] = c;
			}
		}

		return selectedIndexes;
	}
}
