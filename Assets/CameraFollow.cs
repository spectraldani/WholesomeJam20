using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    private new Camera camera;
    [SerializeField] private Vector2 tolerance = new Vector2(0, 0);
    [SerializeField] private float cameraSpeed = 4f;
    public Transform follow = null;

    private bool mustMoveX = false;

    void Start() {
        camera = GetComponent<Camera>();
    }

    void Update() {
        var currentFollowPosition = follow.position;
        var currentPosition = camera.transform.position;

        var distance = currentFollowPosition - currentPosition;

        if (Mathf.Abs(distance.x) > tolerance.x && !mustMoveX) {
            mustMoveX = true;
        }

        if (mustMoveX) {
            cameraSpeed = 3;
            currentPosition.Set(distance.x * cameraSpeed * Time.deltaTime + currentPosition.x, currentPosition.y,
                currentPosition.z);
            if (Mathf.Abs(currentFollowPosition.x - currentPosition.x) < 0.01) {
                mustMoveX = false;
                currentPosition.Set(currentFollowPosition.x, currentPosition.y, currentPosition.z);
            }
        }

        if (Mathf.Abs(distance.y) > tolerance.y) {
            currentPosition.Set(currentPosition.x, currentPosition.y, currentPosition.z);
        }

        // currentPosition.Set(currentFollowPosition.x, currentFollowPosition.y, currentPosition.z);

        camera.transform.position = currentPosition;
    }
}
