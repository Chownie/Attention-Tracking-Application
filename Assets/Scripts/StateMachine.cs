using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {
	public enum State {
		Starting,
		Locked,
		Unlocked,
		Finished
	}

	public static State CurrentState = State.Starting;
}
