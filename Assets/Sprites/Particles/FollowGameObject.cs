using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour
{
    public GameObject target;
    public bool X;
    public bool Y;
    public bool Z;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target) {
            Vector3 newPos = transform.position;
            if (X) newPos.x = target.transform.position.x;
            if (Y) newPos.y = target.transform.position.y;
            if (Z) newPos.z = target.transform.position.z;
            transform.position = newPos;
        }
        
    }
}
