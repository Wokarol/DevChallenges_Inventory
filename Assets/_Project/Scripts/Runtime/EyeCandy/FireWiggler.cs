using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWiggler : MonoBehaviour
{
    private Vector3 baseScale;
    private float seed;

    private void Awake()
    {
        baseScale = transform.localScale;

        seed = Random.value * 50f;
    }

    private void Update()
    {
        var noiseSX = Mathf.PerlinNoise(Time.time * 4f, seed);
        var noiseSY = Mathf.PerlinNoise(Time.time * 4f, seed + 20f);
        var noiseSZ = Mathf.PerlinNoise(Time.time * 4f, seed + 57.2f);
        transform.localScale = baseScale + new Vector3(noiseSX, noiseSY, noiseSZ) * 0.4f;
    }
}
