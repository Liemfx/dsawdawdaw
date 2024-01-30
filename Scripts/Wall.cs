using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{


    //Store a component reference to the attached SpriteRenderer.
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        //Get a component reference to the SpriteRenderer.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


}