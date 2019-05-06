using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraSpeed;
    public int minZoom;
    public int maxZoom;
    public GameObject mapController;
    MapBehavior mapBehavior;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        mapBehavior = mapController.GetComponent<MapBehavior>();
        cam = GetComponent<Camera>();
        StartCoroutine(StartPos());
    }

    IEnumerator StartPos()
    {
        yield return new WaitForSeconds(0.01f);
        Vector2 mapMid = mapBehavior.GetMapMiddle();
        transform.position = new Vector3(mapMid.x, mapMid.y, -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up") || Input.GetKey("w"))
        {
            transform.Translate(0, cameraSpeed * Time.deltaTime, 0, Space.World);
        }
        if (Input.GetKey("down") || Input.GetKey("s"))
        {
            transform.Translate(0, -cameraSpeed * Time.deltaTime, 0, Space.World);
        }
        if (Input.GetKey("left") || Input.GetKey("a"))
        {
            transform.Translate(-cameraSpeed * Time.deltaTime, 0, 0, Space.World);
        }
        if (Input.GetKey("right") || Input.GetKey("d"))
        {
            transform.Translate(cameraSpeed * Time.deltaTime, 0, 0, Space.World);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && (Input.GetAxis("Mouse ScrollWheel") + cam.orthographicSize) > minZoom)
        {
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) cam.orthographicSize--;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && (Input.GetAxis("Mouse ScrollWheel") + cam.orthographicSize) < maxZoom)
        {
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) cam.orthographicSize++;
        }

    }
}
