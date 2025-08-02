using UnityEngine;

public class GuyController : MonoBehaviour
{
    public float speed = 2.0f;
    public Transform leftDown;
    public Transform rightUp;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private bool isMoveFinished = false;
    private bool hasCat = false;

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);

        // if finish
        if (Vector3.Distance(transform.position, endPoint) < 0.01f)
        {
            isMoveFinished = true;
            Spawn();
        }
    }

    void Spawn()
    {
        hasCat = false;
        isMoveFinished = false;
        int start = Random.Range(0, 3);
        int end = 0;
        switch (start)
        {
            case 0: startPoint = new Vector2(leftDown.position.x, Random.Range(leftDown.position.y, rightUp.position.y)); end = 1;  break;
            case 1: startPoint = new Vector2(rightUp.position.x, Random.Range(leftDown.position.y, rightUp.position.y)); end = 0; break;
            case 2: startPoint = new Vector2(Random.Range(leftDown.position.x, rightUp.position.x), leftDown.position.y); end = 3; break;
            case 3: startPoint = new Vector2(Random.Range(leftDown.position.x, rightUp.position.x), rightUp.position.y); end = 2; break;
        }
        
        switch (end)
        {
            case 0: endPoint = new Vector2(leftDown.position.x, Random.Range(leftDown.position.y, rightUp.position.y)); break;
            case 1: endPoint = new Vector2(rightUp.position.x, Random.Range(leftDown.position.y, rightUp.position.y)); break;
            case 2: endPoint = new Vector2(Random.Range(leftDown.position.x, rightUp.position.x), leftDown.position.y); break;
            case 3: endPoint = new Vector2(Random.Range(leftDown.position.x, rightUp.position.x), rightUp.position.y); break;
        }

        transform.position = startPoint;


        Debug.Log("Spawn");
        Debug.Log(start);
        Debug.Log(end);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !hasCat)
        {
            hasCat = true;
            Debug.Log("Start minigame");

            GameObject.Find("CoreGame").SendMessage("Pause");
        }
    }
}
