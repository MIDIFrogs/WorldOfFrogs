using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueBehavior : MonoBehaviour
{
    float t = 0;

    void Update()
    {
        transform.position += transform.up * Time.deltaTime;
        Debug.Log(transform.position);
        t += Time.deltaTime;
        if (t > 10) Destroy(gameObject);
    }
}
