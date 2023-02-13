/*
 *  Player object in the world, also holds player info : Chris
 */

using UnityEngine;

public class Player : InstanceMonoBehaviour<Player> // InstanceMonoBehaviour is so I can access the instance of this class from anywhere in the code : Chris
{
    public GameObject crateRef; // ref to the crate object
    public GameObject crateCheck; // used to check collisions with crates
    public float crateDistance; // distance to detect collisions with crates
    LayerMask crateLayer;

    //public GameObject LookAtTarget; // this is to point the camera at : Chris
    public CharacterController controller; 

	// temp for testing the inventory UI : Chris
	public Inventory backpack = new Inventory();
	// end temp ///////////////////////////////

	private Animator anim;
	public float mouse_sensitivity = 100;
	public float speed;
	public float turnSpeed;
	public float gravity;
	public float jumpHeight;
	float x_rotation = 0;
	Vector3 velocity = new Vector3();
    Collider[] hitColliders = new Collider[10];

    // temp for testing the inventory UI : Chris
    protected override void Awake()
	{
		base.Awake();

        crateLayer = LayerMask.NameToLayer("Crates");

        controller = GetComponent<CharacterController>();
		anim = gameObject.GetComponentInChildren<Animator>();

		backpack.Init(10 * 11);

        Cursor.lockState = CursorLockMode.Locked; // set the cursor state to hide the cursor
    }
    // end temp ///////////////////////////////

    float groundedTimer = 0;
    void Update()
    {
		if (PlayerBackpack.Instance.isOpen || CraftingScreen.Instance.isOpen) return; // if the player backpack is open return

		float mouseX = Input.GetAxis("Mouse X") * (mouse_sensitivity * 100) * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * (mouse_sensitivity * 25) * Time.deltaTime;

		x_rotation -= mouseY;
		x_rotation = Mathf.Clamp(x_rotation, -30, 30);
		ThirdPersonCamera.Instance.transform.localRotation = Quaternion.Euler(x_rotation, 0, 0);
		transform.Rotate(Vector3.up * mouseX);

        bool groundedPlayer = controller.isGrounded;
        if (groundedPlayer)
        {
            // cooldown interval to allow reliable jumping even whem coming down ramps
            groundedTimer = 0.2f;
        }

        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
        }

        // slam into the ground
        if (groundedPlayer && velocity.y < 0)
        {
            // hit ground
            velocity.y = 0f;
        }

        // apply gravity always, to let us track down ramps properly
        velocity.y -= gravity * Time.deltaTime;

        // Reading the Input
        float x = Input.GetAxis("Horizontal") * mouse_sensitivity * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * mouse_sensitivity * Time.deltaTime;

        // scale by speed
        Vector3 move = transform.right * x + transform.forward * y;

        // Moving
        if (move.magnitude > 0 && groundedPlayer)
        {
            anim.Play("infantry_combat_run", -1);
        }
        else
            anim.Play("infantry_combat_idle", -1);

        move *= speed;

        // allow jump as long as the player is on the ground
        if (Input.GetButtonDown("Jump"))
        {
            // must have been grounded recently to allow jump
            if (groundedTimer > 0)
            {
                // no more until we recontact ground
                groundedTimer = 0;

                // Physics dynamics formula for calculating jump up velocity based on height and gravity
                velocity.y += Mathf.Sqrt(jumpHeight * 2 * gravity);
            }
        }

        // inject Y velocity before we use it
        move.y = velocity.y;

        // call .Move() once only
        controller.Move(move * Time.deltaTime);

        #region Key_Commands
        // check for key commands

        if (Input.GetKeyDown(KeyCode.I))
        {
			PlayerBackpack.Instance.Open(false); // open player backpack
		}

        if (Input.GetKeyDown(KeyCode.C))
        {
            CraftingScreen.Instance.Open(false); // open crafting screen
        }

        //var origin = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        //var direction = transform.forward * 10;
        //Debug.DrawRay(origin, direction, Color.red);

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    var ray = new Ray(origin, direction);

        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        var obj = hit.transform.gameObject;

        //        Debug.Log("object hit " + obj.name);
        //        if (obj.layer == crateLayer)
        //        {
        //            PickupCrate(obj.GetComponent<Crate>());
        //        }
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.Q) && !BuildingScreen.Instance.isOpen)
        {
            // pick a random location

            float scalex = Ground.Instance.gameObject.transform.localScale.x * 5;
            float scaley = Ground.Instance.gameObject.transform.localScale.y * 5;

            float randx = Random.Range(-scalex, scalex);
            float randy = Random.Range(-scaley, scaley);

            SpawnCrate(Item.GetRandomType(), Random.Range(1, 99), new Vector3(randx, 30, randy)); // spawn a random crate
        }

        if (Input.GetKeyDown(KeyCode.B) && !BuildingScreen.Instance.isOpen) // build something at that spot based on the item selected in the quickslots
        {
            var item = QuickSlots.Instance.window.GetSelectedItem();

            if(QuickSlots.Instance.isOpen && item.IsValid())
            {
                BuildingScreen.Instance.Open(false, item);
            }
        }

        #endregion
        // End key commands

        for (int i = 0; i < Physics.OverlapSphereNonAlloc(crateCheck.transform.position, crateDistance, hitColliders, 1 << crateLayer); i++) // check for collisions with crates
        {
            PickupCrate(hitColliders[i].gameObject.GetComponent<Crate>());
        }
    }

    void PickupCrate(Crate crate)
    {
        var item = crate.Pickup(); // get the item
        backpack.AddItem(item.type, item.number); // add the item to our inventory

        QuickSlots.Instance.Refresh();
        Debug.Log("Picked up crate");
    }

    public void SpawnCrate(Item.Type type, int number, Vector3 pos)
    {
        var crate = Instantiate(crateRef);
        crate.SetActive(true);
        crate.GetComponent<Crate>().Spawn(type, number);

        crate.transform.position = pos;

        Debug.Log("new crate spawned at x: " + pos.x + " y: " + pos.y);
    }

}
