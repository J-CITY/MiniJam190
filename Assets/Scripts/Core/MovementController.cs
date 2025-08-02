using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private LineRenderer line;

    [SerializeField] private float maxPower = 10f;
    [SerializeField] private float power = 2f;
    [SerializeField] private float maxGoalSpeed = 4f;

    private bool isDragging;

    private bool hasCollide = false;

    void Start()
    {

    }

    void Update()
    {
        if (!isReady())
        {
            //return;
        }
        
        Vector2 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(transform.position, inputPos);

        if (Input.GetMouseButtonDown(0) && distance <= 0.5f)
        {
            DragStart();
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            DragChange(inputPos);
        }
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            DragRelease(inputPos);
        }
    }

    private bool isReady()
    {
        return body.linearVelocity.magnitude <= 0.2f;
    }

    private void DragStart()
    {
        isDragging = true;
        line.positionCount = 2;
    }

    private void DragChange(Vector2 pos)
    {
        Vector2 dir = (Vector2)transform.position - pos;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, (Vector2)transform.position + Vector2.ClampMagnitude((dir * power) / 2, maxPower / 2));
    }

    private void DragRelease(Vector2 pos)
    {
        float distance = Vector2.Distance((Vector2)transform.position, pos);
        isDragging = false;
        line.positionCount = 0;

        if (distance < 1f)
        {
            return;
        }

        Vector2 dir = (Vector2)transform.position - pos;

        body.linearVelocity = Vector2.ClampMagnitude(dir * power, maxPower);

        if (!hasCollide)
        {
            // we jumped without collide with guys
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Guy")
        {
            Debug.Log("Player collided with guy");
            hasCollide = true;

            bool isNPC = false;

            var component = collision.gameObject.GetComponent<GuyController>();
            if (component != null)
            {
                isNPC = component.IsNPC();
            }

            if (!isNPC)
            {
                // start minigame and clear field
                GameObject.Find("CoreGame").SendMessage("Pause");
            }
            else
            {
                //give damage

            }
        }
    }
}
