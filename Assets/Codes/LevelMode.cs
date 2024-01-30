using UnityEngine;
using System.Collections.Generic;

public class LevelMode : MonoBehaviour
{
    public GameObject[] planets; // Array to hold planets
    private Vector3[] startPositions; // To store start positions of planets
    public GameObject orbitTarget; // Target GameObject around which planets will rotate
    private GameObject draggedPlanet = null; // Currently dragged planet
    private Vector3 offset; // Offset between mouse position and planet center

    void Start()
    {
        // Store the initial positions of the planets
        startPositions = new Vector3[planets.Length];
        for (int i = 0; i < planets.Length; i++)
        {
            startPositions[i] = planets[i].transform.position;
        }
    }

    void Update()
    {
        // Convert mouse position to world position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Planet"))
            {
                draggedPlanet = hit.collider.gameObject;
                offset = draggedPlanet.transform.position - mousePos;
            }
        }

        if (draggedPlanet != null)
        {
            draggedPlanet.transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, draggedPlanet.transform.position.z);

            if (Input.GetMouseButtonUp(0))
            {
                Collider2D orbitCollider = GameObject.FindGameObjectWithTag("Orbit").GetComponent<Collider2D>();
                if (orbitCollider.OverlapPoint(draggedPlanet.transform.position)) // Check if inside orbit collider
                {
                    // Attach to orbit and start rotating
                    draggedPlanet.AddComponent<RotateAroundTarget>().Init(orbitTarget, 20f); // Assuming 20 is the speed
                }
                else
                {
                    // Return to start position
                    int index = System.Array.IndexOf(planets, draggedPlanet);
                    draggedPlanet.transform.position = startPositions[index];
                }

                draggedPlanet = null; // Reset dragged planet
            }
        }
    }
}

public class RotateAroundTarget : MonoBehaviour
{
    private GameObject target;
    private float speed;
    public Vector3 direction = Vector3.forward; // Change to Z-axis rotation

    public void Init(GameObject orbitTarget, float orbitSpeed)
    {
        target = orbitTarget;
        speed = orbitSpeed;
    }

    void Update()
    {
        transform.RotateAround(target.transform.position, direction, speed * Time.deltaTime);
    }
}
