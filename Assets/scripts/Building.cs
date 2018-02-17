using UnityEngine;
using System.Collections.Generic;

public class Building {

	public int x, y;
    public int width, height;
    public Tile [,] tiles;

    public Building (int _x, int _y, int _width, int _height)
    {
        x = _x;
        y = _y;
        width = _width;
        height = _height;

        InstantiateTiles ();
        Tree tree = new Tree (width, height, tiles);
        DrawOuterWall ();

        TextureGenerator.GenerateTexture (width, height, tiles);
        
    }

    private void InstantiateTiles ()
    {
        tiles = new Tile [width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile t = new Tile ();
                
                t.tileType = TileType.Empty;

                tiles [x, y] = t;
            }
        }
    }

    private void DrawOuterWall ()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    Tile t = tiles [x, y];
                    t.tileType = TileType.OuterWall;
                }
            }
        }
    }
}

public class Tree {
    
    private Vector2 treeSize;
    private Leaf root;
    private List<Leaf> leaves = new List<Leaf> ();
    private Tile [,] tiles;

    public Tree (int width, int height, Tile [,] _tiles)
    {
        treeSize = new Vector2 (width, height);
        tiles = _tiles;
        CreateLeaves (new Vector2 (width, height));
    }

    private void CreateLeaves (Vector2 size)
    {
        root = new Leaf (0, 0, (int) size.x, (int) size.y, null, tiles, null);
        root.root = root;
        leaves.Add (root);

        bool didSplit = true;
        while (didSplit)
        {
            didSplit = false;

            for (int i = 0; i < leaves.Count; i++)
            {
                // If we haven't already split the leaf
                if (leaves [i].firstChild == null && leaves [i].secondChild == null)
                {
                    // Attempt a split
                    if (leaves [i].Split ())
                    {
                        // If successful, add its children to the list
                        leaves.Add (leaves [i].firstChild);
                        leaves.Add (leaves [i].secondChild);

                        didSplit = true;
                    }
                }
            }
            root.CreateRooms ();
        }
        // SetGrid ();
    }

    private void SetGrid ()
    {
        tiles = new Tile [(int) treeSize.x, (int) treeSize.y];

        for (int x = 0; x < tiles.GetLength (0); x++)
        {
            for (int y = 0; y < tiles.GetLength (1); y++)
            {
                Tile t = new Tile ();
                t.tileType = TileType.Wall;
                t.x = x;
                t.y = y;
                tiles [x, y] = t;
            }
        }

        foreach (Leaf leaf in leaves)
        {
            if (leaf.hasRoom)
            {
                for (int x = 0; x < leaf.roomSize.x; x++)
                {
                    for (int y = 0; y < leaf.roomSize.y; y++)
                    {
                        Tile t = new Tile ();
                        t.tileType = TileType.Empty;
                        t.x = x + (int) leaf.roomPos.x;
                        t.y = y + (int) leaf.roomPos.y;
                        tiles [x, y] = t;
                    }
                }
            }
        }
    }

}

public class Leaf {

	private const int MIN_LEAF_SIZE = 8;

    public Leaf parent;
    public Leaf root;
    public Leaf firstChild;
    public Leaf secondChild;
    public Tile [,] tiles;

    public int x, y;
    public int width, height;
    public Room room;
    public bool hasRoom;

    public Vector2 roomSize, roomPos;
    public int roomMinSize = 8;

    public bool isFirstChild;

    public Leaf (int _x, int _y, int _width, int _height, Leaf _parent, Tile [,] _tiles, Leaf _root)
    {
        x = _x;
        y = _y;
        width = _width;
        height = _height;
        parent = _parent;
        tiles = _tiles;
        root = _root;

        hasRoom = false;
    }

    public bool Split ()
    {
        // If this leaf already has children, skip it
        if (firstChild != null || secondChild != null)
        {
            return false;
        }

        // Incorporate a small chance of the leaf not splitting (but not the root leaf, or its children)
        if (Random.Range (0f, 1f) < 0.2f && parent != null)
        {
            if (parent.parent != null)
            {
                return false;
            }
        }

        // 50:50 chance to split either horizontally or vertically
        bool splitH = Random.Range (0f, 1f) < 0.5f;

        // If the width is greater than 1.25 height, split vertically
        if (width / height > 1.25f)
        {
            splitH = false;
        }

        // If the height is greater than 1.25 width, split horizontally
        if (height / width > 1.25f)
        {
            splitH = true;
        }

        // Determine the max dimension (height or width) of the child leaf
        int max = (splitH ? height : width) - MIN_LEAF_SIZE;

        // If the max is less than the minimum size allowed, skip
        if (max < MIN_LEAF_SIZE)
        {
            return false;
        }

        // Generate split
        int split = Random.Range (MIN_LEAF_SIZE, max);

        if (splitH)
        {
            firstChild = new Leaf (x, y, width, split, this, tiles, root);
            secondChild = new Leaf (x, y + split, width, height - split, this, tiles, root);
        }
        else
        {
            firstChild = new Leaf (x, y, split, height, this, tiles, root);
            secondChild = new Leaf (x + split, y, width - split, height, this, tiles, root);
        }

        // If we've gotten to this point, the split was successful
        return true;
    }

    public void CreateRooms ()
    {
        if (firstChild != null || secondChild != null)
        {
            hasRoom = false;

            if (firstChild != null)
            {
                if (!firstChild.hasRoom)
                {
                    firstChild.CreateRooms ();
                }
            }
            if (secondChild != null)
            {
                if (!secondChild.hasRoom)
                {
                    secondChild.CreateRooms ();
                }
            }
        }
        else
        {
            // roomPos = new Vector2 (
            //     (int) Random.Range (0, width - roomMinSize),
            //     (int) Random.Range (0, height - roomMinSize)
            // );
            // roomSize = new Vector2 (
            //     (int) Random.Range (roomMinSize, width - roomPos.x),
            //     (int) Random.Range (roomMinSize, height - roomPos.y)
            // );
            roomPos = new Vector2 (
                0,
                0
            );
            roomSize = new Vector2 (
                width,
                height
            );

            Room room = new Room (x + (int) roomPos.x, y + (int) roomPos.y, (int) roomSize.x, (int) roomSize.y, this, tiles);
            // Room room = new Room (x, y, width, height, this, tiles);
            this.room = room;
            hasRoom = true;
        }
    }

}

public class Room {

	public int x, y;
    public int width, height;
    public Tile [,] tiles;
    public Leaf parent;
    public List<Tile> wallTiles;
    public List<Tile> cornerWallTiles;

    public Room (int _x, int _y, int _width, int _height, Leaf _parent, Tile [,] _tiles)
    {
        x = _x;
        y = _y;
        width = _width;
        height = _height;
        parent = _parent;
        tiles = _tiles;


        DrawRoom ();
    }

    private void DrawRoom ()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Tile t = new Tile ();
                // if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                if (i == 0 || j == 0)
                {
                    t.tileType = TileType.Wall;
                }
                else
                {
                    t.tileType = TileType.Empty;
                }
                tiles [x + i, y + j] = t;
            }
        }
    }

}
