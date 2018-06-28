using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeInput : MonoBehaviour {
	[SerializeField]
	GvrReticlePointer pointer;

	public void Start() {
		//pointer = GetComponent<GvrReticlePointer>();
	}

	public void Update() {
		
		/*RaycastHit hit;

		if(Physics.Raycast(transform.position, transform.forward, out hit, 100f)) {
			GameObject newObject = hit.collider.gameObject;
			IGazeInputHandler newGaze = (IGazeInputHandler)newObject.GetComponent(typeof(IGazeInputHandler));
			if((GazeTarget == null || newGaze != GazeTarget) && newGaze is IGazeInputHandler) {
				GazeTarget = newGaze;
				GazeTarget.OnGazeEnter();
			}
		} else {
			if(GazeTarget != null) {
				try {
					GazeTarget.OnGazeExit();
				} catch (MissingReferenceException) {
					Debug.Log("Missing Reference");
					GazeTarget = null;
				}
				GazeTarget = null;
			}
		}*/
	}
}
