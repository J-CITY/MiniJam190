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

    [SerializeField] private float unsucsessTimer = 2f;

    private float timer = 0;

    public AudioClip damageSound;


    private bool isDragging;

    private bool hasCollide = false;

    enum State
    {
        Start,
        Jump,
        Unsucsess,
        Sucsess,
        Win,
        Lose
    }

    State state = State.Start;

    void Start()
    {
        ResetPos();
        state = State.Start;
        UpdateVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                if (state == State.Unsucsess)
                {
                    state = State.Start;
                    ResetPos();
                    UpdateVisual();
                }
            }
        }

        // if jamp is end and we didnt collide with guy
        if (state == State.Jump && isReady())
        {
            //ResetPos();
            GameObject.Find("CoreGame").SendMessage("AddStress", -15);
            state = State.Unsucsess;
            timer = unsucsessTimer;
            AudioSource.PlayClipAtPoint(damageSound, transform.position);
            UpdateVisual();
        }
        if (state != State.Start)
        {
            return;
        }
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

    private void ResetPos()
    {
        transform.position = GameObject.Find("PlayerSpawnPoint").transform.position;
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

    private Vector2 saveDir;
    private void DragRelease(Vector2 pos)
    {
        state = State.Jump;
        float distance = Vector2.Distance((Vector2)transform.position, pos);
        isDragging = false;
        line.positionCount = 0;

        if (distance < 1f)
        {
            return;
        }

        Vector2 dir = (Vector2)transform.position - pos;

        saveDir = dir;

        body.linearVelocity = Vector2.ClampMagnitude(dir * power, maxPower);

        if (!hasCollide)
        {
            // we jumped without collide with guys
        }

        UpdateVisual();
       
    }

    void UpdateVisual()
    {

        transform.rotation = Quaternion.identity;

        var visual = transform.Find("Visual");
        if (state == State.Start || state == State.Sucsess || state == State.Win)
        {
            visual.Find("defaultCat").gameObject.SetActive(true);
            visual.Find("jumpCat").gameObject.SetActive(false);
            visual.Find("sadCat").gameObject.SetActive(false);
        }
        else if (state == State.Jump)
        {
            visual.Find("defaultCat").gameObject.SetActive(false);
            visual.Find("jumpCat").gameObject.SetActive(true);
            visual.Find("sadCat").gameObject.SetActive(false);
            float angle = Vector2.SignedAngle(Vector2.up, saveDir);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (state == State.Lose || state == State.Unsucsess)
        {
            visual.Find("defaultCat").gameObject.SetActive(false);
            visual.Find("jumpCat").gameObject.SetActive(false);
            visual.Find("sadCat").gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (state != State.Jump)
        {
            return;
        }
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
                state = State.Sucsess;
                UpdateVisual();
                // start minigame and clear field
                GameObject.Find("CoreGame").SendMessage("Pause");
                GameObject.Find("CoreGame").SendMessage("CreateMinigame");
            }
            else
            {
                GameObject.Find("CoreGame").SendMessage("AddStress", -5);
            }
        }
    }
}
