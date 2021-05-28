using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    // The array that contains all of the levels to be displayed.
    public GameObject[] levelImages;

    public Texture2D levelMap;

    private GameObject levelObject;

    private int level;

    void Start()
    {
        instance = this;
        level = 0;
    }

    public GridValue GetTileValue(int x, int y)
    {
        // Change the x according to the current level.
        x += DividerUtils.SIZE_X * level;

        Color color = levelMap.GetPixel(x, y);

        if (color.Equals(Color.red))
        {
            return GridValue.BOUNDS;
        }
        else if (color.Equals(Color.green))
        {
            return GridValue.SPACE;
        }

        return GridValue.VOID;
    }

    public static GridValue[,] CreateLevel(GridValue[,] field = null)
    {
        if (field == null)
        {
            field = new GridValue[DividerUtils.SIZE_X + 1, DividerUtils.SIZE_Y + 1];
        }

        /*
        for (int x = 0; x <= DividerUtils.SIZE_X; x++)
        {
            for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
            {
                field[x, y] = instance.GetTileValue(x, y);
            }
        }
        /**/

        TextAsset text = (TextAsset)Resources.Load("" + instance.level, typeof(TextAsset));

        if (text == null)
        {
            text = (TextAsset)Resources.Load("Default", typeof(TextAsset));
        }
        //TextAsset text = null;

        if (text != null)
        {
            string content = text.text;

            // Remove the new line.
            content = content.Replace("\n", "");
            content = content.Replace("\r", "");

            Debug.Log(content.Length + " == " + ((DividerUtils.SIZE_X + 1) * (DividerUtils.SIZE_Y + 1)));

            for (int i = 0; i < content.Length; i++)
            {
                int row = i / (DividerUtils.SIZE_X + 1);
                int col = i % (DividerUtils.SIZE_X + 1);

                if (col > DividerUtils.SIZE_X + 1)
                {
                    break;
                }

                switch (content[i])
                {
                    case '0':
                        field[col, row] = GridValue.BOUNDS;
                        break;
                    case '1':
                        field[col, row] = GridValue.VOID;
                        break;
                    case '2':
                        field[col, row] = GridValue.SPACE;
                        LineGenerator.CreateSpacePoint(DividerUtils.GridToUnitPoint(new Vector2(col, row)));
                        break;
                }
            }

        }

        return field;
    }

    /**
     * Advance the current level.
     */
    public static int AdvanceLevel()
    {
        instance.level = instance.level + 1 >= instance.levelImages.Length ? 1 : instance.level + 1;

        if (instance.levelObject != null)
        {
            GameObject.Destroy(instance.levelObject);
        }

        instance.levelObject = GameObject.Instantiate(instance.levelImages[instance.level]);

        return instance.level;
    }

    public static LevelManager GetInstance()
    {
        return instance;
    }

    public static int GetLevel()
    {
        return instance.level;
    }

}
