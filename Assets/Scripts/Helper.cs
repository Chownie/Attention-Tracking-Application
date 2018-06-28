using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Helper {
	public static double StartTime = 0;

	public static double CurrentTime() {
		System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
 		return (System.DateTime.UtcNow - epochStart).TotalMilliseconds;
	}

	public static void Log(string file, params string[] values) {
		double logTime = CurrentTime() - StartTime;
		string output = string.Join(",", values) + "," + logTime.ToString() + "\n";

		Debug.Log(output);
		string filename = Application.persistentDataPath + "/" + file +".csv";
		if(!File.Exists(filename)) {
			File.Create(filename).Dispose();
		}
		File.AppendAllText(filename, output);
	}
}
