using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecenterGrid : MonoBehaviour {
	// Update is called once per frame

	public Transform cam;

	bool ButtonPress() {
		return (Input.GetKeyUp("space") || Input.GetKeyUp("joystick button 0"));
	}

	void Update () {
		if(StateMachine.CurrentState == StateMachine.State.Starting) {
			if(ButtonPress()) {
    			transform.position = cam.transform.position + cam.transform.forward * 15;
				Vector3 direction = transform.position - cam.transform.position;
     			transform.rotation = Quaternion.LookRotation(direction);
			}
		} else {
			this.enabled = false;
		}
	}
}
