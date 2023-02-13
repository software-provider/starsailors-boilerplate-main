/*
 *  A window for building structures on the map : Chris
 */

using UnityEngine;
using System;

public class BuildingScreen : InstanceMonoBehaviour<BuildingScreen>
{
    [SerializeField] private Material fadeMat;
    Material oldMat;
    Color FadeColor = new Color(1, 1, 1, 0.6f);
    Color ErrorColor = new Color(0.3f, 0.0f, 0.0f, 0.6f);

    public bool isOpen { get; private set; } = false;

    Item item = null;
    GameObject preview = null;
    Vector3 previewSize = new Vector3();

    protected override void Awake()
    {
        base.Awake();

        // init stuff here

        Close(true, true);
    }

    void Update()
    {
        if (!isOpen) return;

        // check for key commands

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close(false);
        }

        Vector3 origin = Player.Instance.transform.position;
        origin += Player.Instance.transform.forward * 7;

        bool success;
        float ypos;
        (success, ypos) = Ground.Instance.GetGroundYAtPos(origin.x, origin.z);

        if (!success) ypos = 0;

        preview.transform.localPosition = new Vector3(0, ypos, 7);

        var newpos = Player.Instance.transform.localPosition + Player.Instance.transform.forward * 7;

        bool fits = Ground.Instance.CanBuildStructureAtPos(previewSize, newpos);

        fadeMat.color = fits ? FadeColor : ErrorColor;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (fits && success)
            {
                if (Ground.Instance.BuildStructure(preview, previewSize, new Vector3(newpos.x, -newpos.z, ypos)))
                {
                    preview.GetComponent<Renderer>().material = oldMat;
                    preview = null;
                    item.Clear();
                    QuickSlots.Instance.window.SetBoxSelected(-1);
                    QuickSlots.Instance.Refresh();
                    Close(false);
                }
            }
        }
    }

    public void Open(bool instant, Item _item)
    {
        isOpen = true;

        item = _item;

        // make a preview structure and put it in front of the player so we can see how it fits in the game world

        preview = Ground.Instance.GetStructureForItem(item);

        preview.transform.SetParent(Player.Instance.gameObject.transform);

        preview.layer = LayerMask.NameToLayer("Structures");

        oldMat = preview.GetComponent<Renderer>().material; // save the old material
        preview.GetComponent<Renderer>().material = fadeMat; // setup the faded material

        previewSize = preview.GetComponent<MeshRenderer>().bounds.size;

        QuickSlots.Instance.Close(instant);

        Cursor.lockState = CursorLockMode.None;

        Vector3 openpos = new Vector3(0, 0, 0f);

        if (instant)
            gameObject.transform.localPosition = openpos;
        else
            LeanTween.move(gameObject.GetComponent<RectTransform>(), openpos, 0.3f).setDelay(0.1f);
    }

    public void Close(bool instant, bool force = false)
    {
        if (!isOpen && !force) return;

        isOpen = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (preview != null)
        {
            Destroy(preview); // get rid of the preview structure if there is one
        }

        Vector3 closedpos = new Vector3(-1000, 0, 0f);

        if (instant)
            gameObject.transform.localPosition = closedpos;
        else
            LeanTween.move(gameObject.GetComponent<RectTransform>(), closedpos, 0.3f).setDelay(0.1f);

        if(!PlayerBackpack.Instance.isOpen && !CraftingScreen.Instance.isOpen)
            QuickSlots.Instance.Open(instant);
    }

    
}
