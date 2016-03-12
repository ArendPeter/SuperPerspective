using UnityEngine;
using System.Collections;

public class Lagger : MonoBehaviour {
	public int iterations;
	public GameObject prefab;

	void Update () {
		foreach(Dummy obj in GameObject.FindObjectsOfType<Dummy>()){
			GameObject.Destroy(obj.gameObject)	;
		}
		for(int i = 0; i < iterations; i++){
			Instantiate(prefab);
		}
	}


}
