using UnityEngine;
using System.Collections;

public class SpiralPath : MonoBehaviour {

	private static float spiralRadiusThresh = 5;
	private static float spiralMinApproachSpeed = 3;
	private static float spiralVerticalSpeed = 1;
	private static float spiralHeight = 1;
	public static float spiralMaxRotSpeed = 720;
	private static float spiralAngle = -1f;
	private static float spiralRadius = -1f;
	private static float spiralRadialSpeed = -1f;
	private static float spiralY = -1f;
	private static float spiralRotSpeed = -1f;

	public static Vector3 SpiralPositionTo(Vector3 curPos, Vector3 targetPos){
		Vector3 initialTargetPos = targetPos + Vector3.down * spiralHeight;
		float dist2D = Vector2.Distance(
			new Vector2(curPos.x, curPos.z),
			new Vector2(initialTargetPos.x,initialTargetPos.z)
		);
		if(!SpiralAngleIsSet() && dist2D > spiralRadiusThresh){
			float dist = Vector3.Distance(curPos, initialTargetPos);
			float speed = Mathf.Max(spiralMinApproachSpeed,(dist/spiralRadiusThresh) * spiralMinApproachSpeed);
			return LerpPositionTo(curPos,initialTargetPos,speed);
		}else{
			//initialize
			if(!SpiralAngleIsSet()){
				Vector3 del3D = curPos - targetPos;
				Vector3 del2D = new Vector2(del3D.x,del3D.z);
				spiralAngle = Mathf.Deg2Rad * Vector2.Angle(Vector2.right,del2D);
				if(curPos.z < targetPos.z){
					spiralAngle = 2 * Mathf.PI - spiralAngle;
				}
				spiralRadius = del2D.magnitude;
				Vector3 temp_dir = targetPos - curPos;
				spiralY = temp_dir.y;
				spiralRadialSpeed = spiralRadius * spiralVerticalSpeed / spiralY;
				spiralRotSpeed = 0f;
			}else{
				//update angle
				spiralRotSpeed = Mathf.Min(spiralMaxRotSpeed,spiralRotSpeed+1500*Time.deltaTime);
				spiralAngle += spiralRotSpeed * Mathf.Deg2Rad * Time.deltaTime;
				//update radius
				if(Mathf.Abs(spiralRadius) < spiralRadialSpeed * Time.deltaTime){
					spiralRadius = 0;
					spiralAngle = -1;
					return targetPos;
				}else{
					spiralRadius -= spiralRadialSpeed * Time.deltaTime;
				}
				//update y
				spiralY -= spiralVerticalSpeed * Time.deltaTime;
			}
			//update position
			return new Vector3(
				targetPos.x + spiralRadius * Mathf.Cos(spiralAngle),
				targetPos.y - spiralY,
				targetPos.z + spiralRadius * Mathf.Sin(spiralAngle)
			);
		}
	}

	private static Vector3 LerpPositionTo(Vector3 curPos, Vector3 newPos, float speed){
		if(!GameStateManager.is3D()){
			curPos.z = newPos.z;
		}
		float dist = Vector3.Distance(curPos, newPos);
		float percent = speed * Time.deltaTime / dist;
		return Vector3.Lerp(curPos, newPos, percent);
	}

	private static bool SpiralAngleIsSet(){
		return spiralAngle != -1;
	}
}
