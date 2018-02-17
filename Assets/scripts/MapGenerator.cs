using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField]
    private int width, height;

    private MeshRenderer mapRen;
    private Tile [,] tiles;

    private void Awake ()
    {
        mapRen = transform.Find ("mapRenderer").GetComponent<MeshRenderer> ();
    }

    private void Start ()
    {
        GenerateMap ();
    }

    private void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {
            GenerateMap ();
        }
    }

    private void GenerateMap ()
    {
        tiles = new Tile [width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile t = new Tile ();
                t.x = x;
                t.y = y;
                t.tileType = TileType.Empty;

                tiles [x, y] = t;
            }
        }

        Building newBuilding = new Building (0, 0, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles [x, y] = newBuilding.tiles [x, y];
            }
        }


        mapRen.materials [0].mainTexture = TextureGenerator.GenerateTexture (width, height, tiles);
    }

}
