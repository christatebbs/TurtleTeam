using UnityEngine;
using System.Collections;

public class cameraFollow : MonoBehaviour
{
    private GameObject objectToFollow;
    public float speed = 2.0f;

    void Update()
    {
        objectToFollow = GameObject.Find("Player");
        float interpolation = speed * Time.deltaTime;

        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.transform.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);

        this.transform.position = position;
    }
}