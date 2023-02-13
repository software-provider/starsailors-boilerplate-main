using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField]
    public Tilemap groundTilemap;

    [SerializeField]
    public Tilemap collisionTilemp;

    private PlayerMovement2D controls;

    private void Awake()
    {
        controls = new PlayerMovement2D();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        controls.Main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
    }

    private void Move(Vector2 direction)
    {
        if (CanMove(direction))
        {
            transform.position += (Vector3)direction;
        }
    }

    private bool CanMove(Vector2 direction)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position + (Vector3)direction);

        if (!groundTilemap.HasTile(gridPosition) || collisionTilemp.HasTile(gridPosition))
        {
            return false;
        } 
        return true;
    }
}
