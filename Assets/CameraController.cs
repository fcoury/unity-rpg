using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Tilemap theMap;

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;
    private float halfHeight;
    private float halfWidth;
    void Start()
    {
        Debug.Log("Camera tile is " + theMap);
        target = FindObjectOfType<PlayerController>().transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        bottomLeftLimit = theMap.localBounds.min + new Vector3(halfWidth, halfHeight, 0);
        topRightLimit = theMap.localBounds.max - new Vector3(halfWidth, halfHeight, 0);

        PlayerController.instance.SetBounds(theMap.localBounds.min, theMap.localBounds.max);

        Debug.Log("MoveTo: " + PlayerController.instance.moveTo);
        if (PlayerController.instance.moveTo != Vector3.zero) {
            Debug.Log("Allowing to move");
            PlayerController.instance.moveToEnabled = true;
        }
    }

    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);
    }
}
