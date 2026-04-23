using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxMove : MonoBehaviour
{
    [SerializeField] 
    private Vector2 velMovi;

    private Vector2 offset;
    private Material mat;
    private Rigidbody2D rb;

    private void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        offset = (rb.linearVelocity.x * 0.1f) * velMovi * Time.deltaTime;
        mat.mainTextureOffset += offset;
    }
}