using UnityEngine;

public class TextureGenerator {

    private static Color emptyColour = Color.white;
    private static Color wallColour = Color.black;
    private static Color cornerWallColour = Color.red;
    private static Color outerWallColour = Color.blue;


	public static Texture2D GenerateTexture (int width, int height, Tile [,] tiles)
    {
        Texture2D texture = new Texture2D (width, height);
        Color [] pixels = new Color [width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                switch (tiles [x, y].tileType)
                {
                    case TileType.Empty:
                        pixels [x + width * y] = emptyColour;
                        break;
                    case TileType.Wall:
                        pixels [x + width * y] = wallColour;
                        break;
                    case TileType.CornerWall:
                        pixels [x + width * y] = cornerWallColour;
                        break;
                    case TileType.OuterWall:
                        pixels [x + width * y] = outerWallColour;
                        break;
                }
            }
        }

        texture.SetPixels (pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.Apply ();

        return texture;
    }

}
