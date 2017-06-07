using UnityEngine;
using System.Collections;

public class FlipFailIndicator : MonoBehaviour {
	public Transform perspCam;

	public Collider testOverlap;

	public static Transform sharedPerspCam;
	Collider overlappingBlock;

	public float[] blinkDurations;
  int blinkFrame = 0;
	float blinkThresh = 0;
	float blinkTime = -1f;
	float minScale = .025f;

	void Start(){
		this.GetComponent<Renderer>().enabled = false;
		if(perspCam != null){
			//print("persp set");
			sharedPerspCam = perspCam;
		}
	}

	public void FixedUpdate(){
		if(blinkTime >= 0){
			blinkTime += Time.deltaTime;
		  if(blinkTime >= blinkThresh){
				toggleVisible();
				if(blinkFrame == blinkDurations.Length){
				  blinkTime = -1f;
				}else{
					blinkThresh+=blinkDurations[blinkFrame];
					blinkFrame++;
				}
			}
			if(!toggleEnabled()){
				disableVisible();
				blinkTime = -1f;
			}
		}
	}

	private void toggleVisible(){
		bool vis = this.GetComponent<Renderer>().enabled;
		vis = !vis;
		this.GetComponent<Renderer>().enabled = vis;
	}

	public void disableVisible(){
		this.GetComponent<Renderer>().enabled = false;
	}

	public void blink(){
		bool hasOverlap = overlappingBlock != null;
		if(hasOverlap){
			testOverlap = overlappingBlock;
		  updateZPosition();
			bindToOverlap();
			initBlinkVars();
		}else{
			testOverlap = null;
		}
	}

	private void updateZPosition(){
		Vector3 pos = transform.position;
		pos.z = sharedPerspCam.transform.position.z + 1;
		transform.position = pos;
	}

	private void bindToOverlap(){
		Bounds myBounds = transform.parent.gameObject.GetComponent<Collider>().bounds;
		Bounds ovBounds = overlappingBlock.bounds;

		float minX = Mathf.Max(myBounds.min.x,ovBounds.min.x);
		float maxX = Mathf.Min(myBounds.max.x,ovBounds.max.x);
		float minY = Mathf.Max(myBounds.min.y,ovBounds.min.y);
		float maxY = Mathf.Min(myBounds.max.y,ovBounds.max.y);

		Vector3 newPos = new Vector3((minX + maxX)/2f,(minY + maxY)/2f,transform.position.z);
		Vector3 newScale = new Vector3(
			Mathf.Max(minScale,(maxX - minX) / 10f),
			.1f,
			Mathf.Max(minScale,(maxY - minY) / 10f));

		transform.position = newPos;

		Vector3 parScale = transform.parent.transform.lossyScale;
		newScale.x /= parScale.x;
		newScale.y /= parScale.z; //not a typo, y/z is necessary due to the rotation on indicator
		newScale.z /= parScale.y;
		transform.localScale = newScale;
	}

	private void initBlinkVars(){
		blinkTime = 0;
		blinkFrame = 0;
		blinkThresh = 0;
	}

	public void setOverlappingBlock(Collider obj){
			overlappingBlock = obj;
	}

	public bool isBlinking(){
	  return blinkTime != -1f;
	}

	private bool toggleEnabled(){
		MobilePlatform plat = transform.parent.GetComponent<MobilePlatform>();
		if(plat != null && !plat.controlled){
			return false;
		}
		return true;
	}
}
