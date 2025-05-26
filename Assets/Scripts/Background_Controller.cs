using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Controller : MonoBehaviour
{
    private float _startPosX;
    public float parallaxEffect;
    public Camera _cam;
    // Start is called before the first frame update
    void Start()
    {
        _startPosX = gameObject.transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float _distanceX = _cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(_startPosX + _distanceX, transform.position.y, transform.position.z); 
    }
}
