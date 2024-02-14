using UnityEngine;

public class Planet : MonoBehaviour
{
    private GameObject orbitTarget;
    private Collider2D clickZoneCollider;
    private GameObject destroyZone;
    public PlayerHealth playerHealth; // Assign this in the inspector or find it dynamically
    private bool wasClicked = false;


    public void Initialize(GameObject orbitTarget, Collider2D clickZone, GameObject destroyZone)
    {
        this.orbitTarget = orbitTarget;
        this.clickZoneCollider = clickZone;
        this.destroyZone = destroyZone;
    }
    void Start()
{
    // Find the PlayerHealth component in the scene
    playerHealth = FindObjectOfType<PlayerHealth>();
}
    void Update()
    {
        // Orbit around the target
        OrbitAroundTarget();

        // Check for clicks
        if (Input.GetMouseButtonDown(0))
        {
            CheckForClick();
        }
    }

void OrbitAroundTarget()
{
    if (orbitTarget != null)
    {
        transform.RotateAround(orbitTarget.transform.position, Vector3.forward, 20 * Time.deltaTime); // Adjust speed as needed
    }
}
public void SetSkin(Sprite newSkin)
{
    SpriteRenderer renderer = GetComponent<SpriteRenderer>();
    if (renderer != null)
    {
        renderer.sprite = newSkin;
    }
}

void CheckForClick()
{
    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    if (clickZoneCollider.OverlapPoint(mousePosition) && GetComponent<Collider2D>().OverlapPoint(mousePosition))
    {
        wasClicked = true; // Mark as clicked
        playerHealth.IncreaseHealth(); // Increase health on successful click
        Destroy(gameObject);
    }
}

void OnTriggerEnter2D(Collider2D other)
{
    if (destroyZone != null && other.gameObject == destroyZone)
    {
        Destroy(gameObject);
    }
    // else
    // {
    //     Debug.Log($"Collided with something else: {other.gameObject.name}");
    // }
}
void OnTriggerExit2D(Collider2D other)
{
    if (other == clickZoneCollider && !wasClicked)
    {
        playerHealth.DecreaseHealth(); // Decrease health on miss
    }
}
}
