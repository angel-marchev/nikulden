using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteChanger : MonoBehaviour
{
    public bool isA;
    public bool isAB;
    public bool isABC;
    public bool isAC;

    public Sprite[] sprites;

    private Image image;

    // Use this for initialization
    void Start()
    {
        isA = true;
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isA)
        {
            image.sprite = sprites[0];
        }

        if (isAB)
        {
            image.sprite = sprites[1];
        }

        if (isAC)
        {
            image.sprite = sprites[2];
        }

        if (isABC)
        {
            image.sprite = sprites[3];
        }
    }
}
