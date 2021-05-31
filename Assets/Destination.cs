using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{

	[Header("Monitor")]
	public Monitor monitor;

	private void Awake() {
		
 
	}
	
    // Start is called before the first frame update
    void Start()
    {

		//Fetch the Renderer component of the GameObject
        
    }

    // Update is called once per frame
    void Update()
    {

    }

	// Stop training when converges.
	private void OnCollisionEnter(Collision other) {
		monitor.doConverge();
	}
}


