using UnityEngine;
using System.Collections;

public class Scroll : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 6 * Time.deltaTime, transform.localPosition.z);
    }
}
