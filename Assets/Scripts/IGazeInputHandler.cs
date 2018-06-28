using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGazeInputHandler {
	void OnGazeEnter();
	void OnGazeExit();
	void OnGazeHold();
}
