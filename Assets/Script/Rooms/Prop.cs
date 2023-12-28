using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    [Header("Variation")]
    public Sprite[] variants;
    public float scaleVariation = 0;
    public float rotationVariation = 0;
    private void Start()
    {
        GenerateRandom();
    }
    void GenerateRandom()
    {
        spriteRenderer.sprite =variants[Mathf.FloorToInt(Random.value * variants.Length)];
        transform.localScale *= 1 + Random.Range(-scaleVariation, scaleVariation);
        transform.rotation *= Quaternion.Euler(  Vector3.forward * Random.Range(-rotationVariation, rotationVariation));
    }
}
