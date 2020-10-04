using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    private new Camera camera;
    [SerializeField] private Vector2 tolerance = new Vector2(0, 0);
    [SerializeField] private float cameraSpeed = 4f;
    public Transform follow = null;

    private bool mustMoveX = false;
    private bool mustMoveY = false;

    private const double eps = 0.01;

    void Start() {
        camera = GetComponent<Camera>();
    }

    void Update() {
        var currentFollowPosition = follow.position;
        currentFollowPosition.y += 5f;
        var currentPosition = camera.transform.position;

        var distance = currentFollowPosition - currentPosition;

        if (Mathf.Abs(distance.x) > tolerance.x && !mustMoveX) {
            mustMoveX = true;
        }

        if (Mathf.Abs(distance.y) > tolerance.y && !mustMoveY) {
            mustMoveY = true;
        }

        if (mustMoveX) {
            currentPosition.Set(distance.x * cameraSpeed * Time.deltaTime + currentPosition.x, currentPosition.y,
                currentPosition.z);
            if (Mathf.Abs(currentFollowPosition.x - currentPosition.x) < eps) {
                mustMoveX = false;
                currentPosition.Set(currentFollowPosition.x, currentPosition.y, currentPosition.z);
            }
        }

        if (mustMoveY) {
            currentPosition.Set(currentPosition.x, distance.y * cameraSpeed * Time.deltaTime + currentPosition.y,
                currentPosition.z);
            if (Mathf.Abs(currentFollowPosition.y - currentPosition.y) < eps) {
                mustMoveY = false;
                currentPosition.Set(currentPosition.x, currentFollowPosition.y, currentPosition.z);
            }
        }

        // currentPosition.Set(currentFollowPosition.x, currentFollowPosition.y, currentPosition.z);

        camera.transform.position = currentPosition;
    }
}
