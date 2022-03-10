using UnityEngine;
using System.Collections;

public class WaypointNodeScript : MonoBehaviour {

	public WaypointNodeScript nextNode;
	public float radius = 20f;
	public Color lineColor = Color.green;
	public Color sphereColor = Color.yellow;

	void OnDrawGizmos()
	{

		Gizmos.color = sphereColor;
		Gizmos.DrawWireSphere (transform.position, radius);

		if (nextNode) {
		
			Gizmos.color = lineColor;
			Gizmos.DrawLine (transform.position, nextNode.transform.position);
		
		}

	}

}
