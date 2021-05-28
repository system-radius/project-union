using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The class to handle the masking properties for the level image.
 */
public class MaskManager : MonoBehaviour
{
    private static MaskManager instance;

    private Texture2D texture;

    private Color fillColor = Color.black;

    private Color emptyColor = Color.clear;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        // Create the mask.
        CreateTexture();

        // Create the rectangle for the sprite.
        Rect rect = new Rect(
            0, 0,
            texture.width,
            texture.height
        );

        // Create the sprite.
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

        //new GameObject("test sprite").AddComponent<SpriteRenderer>().sprite = sprite;

        // Get the sprite mask component.
        SpriteMask mask = GetComponent<SpriteMask>();

        // Set the sprite for the mask.
        mask.sprite = sprite;
    }

    private void CreateTexture()
    {
        // Create a new texture.
        texture = new Texture2D(
            DividerUtils.SIZE_X + 1,
            DividerUtils.SIZE_Y + 1,
            TextureFormat.ARGB32,
            false
        );

        texture.filterMode = FilterMode.Point;
        //texture.wrapMode = WrapMode.Clamp;

        ResetMask();
    }

    private void FillColor(Color color)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }

    private void ResetMask() {
        // Initialize the texture to contain all black.
        FillColor(emptyColor);
    }

    private void ExposeAll()
    {
        FillColor(fillColor);
    }

    /**
     * Change the color on the texture being used in the sprite mask.
     * The coordinates must be in world version.
     */
    public static void ExposeMask(int x, int y)
    {
        instance.texture.SetPixel(x, y, instance.fillColor);
        instance.texture.Apply();
    }

    public static void StaticReset()
    {
        instance.ResetMask();
    }

    public static void DisplayLevel()
    {
        instance.ExposeAll();
    }
}
