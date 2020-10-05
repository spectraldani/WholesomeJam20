using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    private new Camera camera;

    // [SerializeField] private Vector2 tolerance = new Vector2(0, 0);
    // [SerializeField] private float cameraSpeed = 10f;
    public Transform follow = null;

    // private bool mustMoveX = false;
    // private bool mustMoveY = false;

    // private const double eps = 0.001;
    private Vector2 cameraBounds;
    public Rect Bounds = new Rect(0,0,10,10);
    private float2 xLim;
    private float2 yLim;

    void Awake() {
        camera = GetComponent<Camera>();
        cameraBounds = new Vector2(Camera.main.orthographicSize * Screen.width / Screen.height,
            Camera.main.orthographicSize);
        xLim = new float2(Bounds.min.x, Bounds.max.x);
        yLim = new float2(Bounds.min.y, Bounds.max.y);


        xLim[0] += cameraBounds.x;
        xLim[1] -= cameraBounds.x;
        yLim[0] += cameraBounds.y;
        yLim[1] -= cameraBounds.y;

        camera.transform.position = new Vector3(follow.position.x, follow.position.y, camera.transform.position.z);
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }

    void FixedUpdate() {
        var currentFollowPosition = follow.position;
        // currentFollowPosition.y += 5f;
        var currentPosition = camera.transform.position;

        // var distance = currentFollowPosition - currentPosition;
        //
        // if (Mathf.Abs(distance.x) > tolerance.x && !mustMoveX) {
        //     mustMoveX = true;
        // }
        //
        // if (Mathf.Abs(distance.y) > tolerance.y && !mustMoveY) {
        //     mustMoveY = true;
        // }
        //
        // if (mustMoveX) {
        //     currentPosition.Set(distance.x * cameraSpeed * Time.deltaTime + currentPosition.x, currentPosition.y,
        //         currentPosition.z);
        //     if (Mathf.Abs(currentFollowPosition.x - currentPosition.x) < eps) {
        //         mustMoveX = false;
        //         currentPosition.Set(currentFollowPosition.x, currentPosition.y, currentPosition.z);
        //     }
        // }
        //
        // if (mustMoveY) {
        //     currentPosition.Set(currentPosition.x, distance.y * cameraSpeed * Time.deltaTime + currentPosition.y,
        //         currentPosition.z);
        //     if (Mathf.Abs(currentFollowPosition.y - currentPosition.y) < eps) {
        //         mustMoveY = false;
        //         currentPosition.Set(currentPosition.x, currentFollowPosition.y, currentPosition.z);
        //     }
        // }

        currentPosition.Set(
            Mathf.Clamp(currentFollowPosition.x, xLim[0], xLim[1]),
            Mathf.Clamp(currentFollowPosition.y, yLim[0], yLim[1]),
            currentPosition.z
        );

        camera.transform.position = currentPosition;
    }
}
