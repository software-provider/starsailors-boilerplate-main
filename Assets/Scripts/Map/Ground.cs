/*
 *  This keeps track of the structures on the ground and has functions to see if there is build space : Chris
 */

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ground : InstanceMonoBehaviour<Ground>
{
    public List<GameObject> structure_prefabs;

    LayerMask structuresLayer;
    LayerMask groundLayer;
    LayerMask ignoreLayer;

    GameObject[,] grid;

    Vector3 groundSize;
    int width;
    int height;

    protected override void Awake()
    {
        base.Awake();

        structuresLayer = LayerMask.NameToLayer("Structures");
        groundLayer = LayerMask.NameToLayer("Ground");
        ignoreLayer = LayerMask.NameToLayer("Ignore");

        groundSize = GetComponent<MeshFilter>().mesh.bounds.size;

        width = (int)groundSize.x;
        height = (int)groundSize.y;

        grid = new GameObject[width, height];

        Debug.Log("grid size width " + width + " height " + height);

        RefreshGridObjects();

        //Debug_Print_Grid();
    }

    void RefreshGridObjects()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = null;
            }
        }

        List<GameObject> objects = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == structuresLayer) // only get objects that are in the layers structure
            {
                if (!objects.Contains(child.gameObject)) // if we don't already have this object recorded in the grid
                {
                    objects.Add(child.gameObject); // add the object so we don't add it again

                    AddObjectToGrid(child.gameObject);
                }
            }
        }
    }

    public (bool, float) GetGroundYAtPos(float x, float y)
    {
        // cast a ray from above and wait till it hits the ground, then get that y pos

        var direction = new Vector3(0, -1, 0);
        var start = new Vector3(x, 20, y);

        //Debug.DrawRay(start, direction, Color.red, 10);

        var ray = new Ray(start, direction);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, 1 << groundLayer))
        {
            var obj = hit.transform.gameObject;

            //Debug.Log("hit " + obj.name + " at " + obj.transform.position);
            if (obj.layer == groundLayer)
            {
                return (true, hit.point.y); // hit so return true
            }
        }

        return (false, -1); // didn't hit so return false
    }

    void AddObjectToGrid(GameObject obj) // add this object to the grid
    {
        int startx;
        int starty;
        int endx;
        int endy;

        var ob_pos = obj.transform.localPosition;

        (startx, starty, endx, endy) = GetGridSpaceForObject(obj.GetComponent<MeshFilter>().sharedMesh.bounds.size, new Vector3(ob_pos.x, ob_pos.z, -ob_pos.y));

        for (int x = startx; x <= endx; x++)
        {
            for (int y = starty; y <= endy; y++)
            {
                if (!ValidPos(x, y)) continue;

                grid[x, y] = obj;
            }
        }
    }

    (int, int, int, int) GetGridSpaceForObject(Vector3 size, Vector3 at)
    {
        //var size = obj.GetComponent<MeshFilter>().sharedMesh.bounds.size; // get the size
        int px, py;

        (px, py) = GroundToGridPos(at); // get starting position

        int startx = px - (int)(size.x / 2);
        int starty = py - (int)(size.y / 2);
        int endx = startx + (int)size.x + (size.x % 1 == 0 ? 0 : 1);
        int endy = starty + (int)size.y + (size.y % 1 == 0 ? 0 : 1);

        return (startx, starty, endx, endy);
    }

    public Vector3 GridToGroundPos(int x, int y)
    {
        float startx = -groundSize.x / 2;
        float starty = groundSize.y / 2;

        return new Vector3(startx + x, starty - y);
    }

    //public (int, int) GroundToGridPos(Vector3 pos)
    //{
    //    int x = (int)(50 + pos.x);
    //    int y = (int)(pos.y > 0 ? 50 - pos.y : 50 + -pos.y);

    //    return (x, y);
    //}

    public (int, int) GroundToGridPos(Vector3 pos)
    {
        int x = (int)(50 + pos.x);
        int y = (int)(pos.z > 0 ? 50 - pos.z : 50 + -pos.z);

        return (x, y);
    }

    public bool CanBuildStructureAtPos(Vector3 size, Vector3 where)
    {
        //int gx, gy;

        //(gx, gy) = GroundToGridPos(where);

        //int footx = (int)footprintx;
        //int footy = (int)footprinty;

        //Debug.Log("gx " + gx + " gy " + gy + " where " + where + " footx " + footx + " footy " + footy);

        int startx, starty, endx, endy;

        (startx, starty, endx, endy) = GetGridSpaceForObject(size, where);

        for (int x = startx; x <= endx; x++)
        {
            for (int y = starty; y <= endy; y++)
            {
                if (!ValidPos(x, y) || grid[x, y] != null)
                {
                    Debug.Log("x " + x + " y " + y + " ValidPos " + ValidPos(x, y) + " object " + grid[x, y]?.name);
                    return false;
                }
            }
        }

        return true;
    }


    public bool BuildStructure(GameObject structure, Vector3 size, Vector3 where)
    {
        int startx, starty, endx, endy;
        (startx, starty, endx, endy) = GetGridSpaceForObject(size, where);

        structure.transform.SetParent(gameObject.transform);
        structure.transform.localPosition = where;

        Debug.Log("spawned " + structure.name + " at " + where);

        for (int x = startx; x <= endx; x++)
        {
            for (int y = starty; y <= endy; y++)
            {
                if (!ValidPos(x, y)) continue;

                grid[x, y] = structure;
            }
        }

        return true;
    }

    public GameObject GetStructureForItem(Item item)
    {
        if (!item.IsValid())
            return null;

        int s = (int)item.type % structure_prefabs.Count;

        var structure = Instantiate(structure_prefabs[s]);
        structure.SetActive(true);

        return structure;
    }

    bool ValidPos(int x, int y)
    {
        if (x >= width || x < 0 || y >= height || y < 0) return false;

        return true;
    }

    void Debug_Print_Grid() // debugging purposes
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null)
                    Debug.Log("grid[" + x + "][" + y + "] = " + (grid[x, y] != null ? grid[x, y].gameObject.name : "null"));
            }
        }
    }

}
