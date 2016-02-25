using UnityEngine;
using System.Collections;

public class LayerManager : MonoBehaviour {

	public bool collisionLayerVisible = false;
	public bool artLayerVisible = true;

	GameObject[] collisionParents;
	GameObject[] artParents;

	void Awake () {

		collisionParents = GameObject.FindGameObjectsWithTag("Collision_Layer");
		artParents = GameObject.FindGameObjectsWithTag("Art_Layer");
		if(!collisionLayerVisible){
			for(int i = 0; i < collisionParents.Length; i++)
				makeChildrenInvisible(collisionParents[i]);
		}
		if(!artLayerVisible){
			for(int i = 0; i < artParents.Length; i++)
				makeChildrenInvisible(artParents[i]);
		}
	}

	private void makeChildrenInvisible(GameObject par){
		bool parentExists = par!=null;
		if(parentExists){
			MeshRenderer[] renderableChildren = par.GetComponentsInChildren<MeshRenderer>();
			for(int i = 0; i < renderableChildren.Length; i++){
				renderableChildren[i].enabled = false;
			}
		}
	}

}
