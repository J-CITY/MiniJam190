using System.Collections.Generic;
using UnityEngine;

public class GuyController : MonoBehaviour
{
    [System.Serializable]
    public struct Line
    {
        public Vector2 start;
        public Vector2 end;
        public float thickness;
        
        public Line(Vector2 start, Vector2 end, float thickness)
        {
            this.start = start;
            this.end = end;
            this.thickness = thickness;
        }
    }
    
    // Статический список всех активных траекторий
    private static List<Line> activeLines = new List<Line>();

    public static int GuysCount = 10;


    public float speedMin = 1.0f;
    public float speedMax = 4.0f;

    public float speed = 2.0f;
    public Transform leftDown;
    public Transform rightUp;

    public int trashholdForDamage = 7;

    [SerializeField] private float lineThickness = 1.0f; // Толщина линии N
    [SerializeField] private int maxSpawnAttempts = 50; // Максимум попыток создания траектории
    
    private Vector2 startPoint;
    private Vector2 endPoint;
    private bool isMoveFinished = false;
    private bool hasCat = false;
    private Line currentLine;


    bool hasDamageForPlayer = false;
    Loot loot = Loot.Loot1;
    bool isNPC = false;

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
            // Удаляем текущую линию из активных
            RemoveLineFromActive();
            Spawn();
        }
    }

    void InitVisual()
    {
        int bodyId = Random.Range(0, 4);
        int headId = Random.Range(0, 4);
        int hairId = Random.Range(0, 4);

        var visual = transform.Find("Visual");
        for (int i = 0; i < 4; ++i)
        {
            var body = visual.Find("body" + i.ToString());
            body.gameObject.SetActive(i == bodyId);

            var head = visual.Find("head" + i.ToString());
            head.gameObject.SetActive(i == headId);

            var hair = visual.Find("hair" + i.ToString());
            hair.gameObject.SetActive(i == hairId);
        }

    }

    void Spawn()
    {
        InitVisual();
        hasDamageForPlayer = Random.Range(0, 11) > trashholdForDamage;
        loot = (Loot)Random.Range(0, System.Enum.GetNames(typeof(Loot)).Length + 1);

        if (GuysCount <= 0)
        {
            isNPC = true;
        }
        else
        {
            isNPC = false;
        }

        RemoveLineFromActive();

        hasCat = false;
        isMoveFinished = false;

        speed = Random.Range(speedMin, speedMax);
        // Удаляем предыдущую линию если была
        RemoveLineFromActive();
        
        bool validLineFound = false;
        int attempts = 0;
        
        while (!validLineFound && attempts < maxSpawnAttempts)
        {
            attempts++;
            
            // Генерируем случайную траекторию
            GenerateRandomLine();
            
            // Создаем временную линию для проверки
            Line tempLine = new Line(startPoint, endPoint, lineThickness);
            
            // Проверяем пересечения с активными линиями
            if (!IsLineIntersectingWithActive(tempLine))
            {
                validLineFound = true;
                currentLine = tempLine;
                AddLineToActive(currentLine);
            }
        }
        
        // Если не удалось найти безопасную траекторию, используем последнюю сгенерированную
        if (!validLineFound)
        {
            Debug.LogWarning($"Could not find non-intersecting line after {maxSpawnAttempts} attempts");
            currentLine = new Line(startPoint, endPoint, lineThickness);
            AddLineToActive(currentLine);
        }
        
        transform.position = startPoint;

        Debug.Log($"Spawn attempt {attempts}, active lines: {activeLines.Count}");
    }
    
    void GenerateRandomLine()
    {
        int start = Random.Range(0, 2);
        int end = 0;
        
        switch (start)
        {
            case 0: 
                startPoint = new Vector2(leftDown.position.x, Random.Range(leftDown.position.y, rightUp.position.y)); 
                end = 1;  
                break;
            case 1: 
                startPoint = new Vector2(rightUp.position.x, Random.Range(leftDown.position.y, rightUp.position.y)); 
                end = 0; 
                break;
            case 2: 
                startPoint = new Vector2(Random.Range(leftDown.position.x, rightUp.position.x), leftDown.position.y); 
                end = 3; 
                break;
            case 3: 
                startPoint = new Vector2(Random.Range(leftDown.position.x, rightUp.position.x), rightUp.position.y); 
                end = 2; 
                break;
        }
        
        switch (end)
        {
            case 0: 
                endPoint = new Vector2(leftDown.position.x, Random.Range(leftDown.position.y, rightUp.position.y)); 
                break;
            case 1: 
                endPoint = new Vector2(rightUp.position.x, Random.Range(leftDown.position.y, rightUp.position.y)); 
                break;
            case 2: 
                endPoint = new Vector2(Random.Range(leftDown.position.x, rightUp.position.x), leftDown.position.y); 
                break;
            case 3: 
                endPoint = new Vector2(Random.Range(leftDown.position.x, rightUp.position.x), rightUp.position.y); 
                break;
        }
    }
    
    // Проверка пересечения линии с активными линиями
    bool IsLineIntersectingWithActive(Line newLine)
    {
        foreach (Line activeLine in activeLines)
        {
            if (DoLinesIntersectWithThickness(newLine, activeLine))
            {
                return true;
            }
        }
        return false;
    }
    
    // Проверка пересечения двух линий с учетом толщины
    bool DoLinesIntersectWithThickness(Line line1, Line line2)
    {
        // Суммарная толщина для проверки
        float combinedThickness = (line1.thickness + line2.thickness) * 0.5f;
        
        // Находим расстояние между отрезками
        float distance = DistanceBetweenLineSegments(line1.start, line1.end, line2.start, line2.end);
        
        return distance < combinedThickness;
    }
    
    // Вычисление минимального расстояния между двумя отрезками
    float DistanceBetweenLineSegments(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        Vector2 u = a2 - a1;
        Vector2 v = b2 - b1;
        Vector2 w = a1 - b1;
        
        float a = Vector2.Dot(u, u);
        float b = Vector2.Dot(u, v);
        float c = Vector2.Dot(v, v);
        float d = Vector2.Dot(u, w);
        float e = Vector2.Dot(v, w);
        
        float D = a * c - b * b;
        float sc, sN, sD = D;
        float tc, tN, tD = D;
        
        if (D < 0.0001f)
        {
            sN = 0.0f;
            sD = 1.0f;
            tN = e;
            tD = c;
        }
        else
        {
            sN = (b * e - c * d);
            tN = (a * e - b * d);
            if (sN < 0.0f)
            {
                sN = 0.0f;
                tN = e;
                tD = c;
            }
            else if (sN > sD)
            {
                sN = sD;
                tN = e + b;
                tD = c;
            }
        }
        
        if (tN < 0.0f)
        {
            tN = 0.0f;
            if (-d < 0.0f)
                sN = 0.0f;
            else if (-d > a)
                sN = sD;
            else
            {
                sN = -d;
                sD = a;
            }
        }
        else if (tN > tD)
        {
            tN = tD;
            if ((-d + b) < 0.0f)
                sN = 0;
            else if ((-d + b) > a)
                sN = sD;
            else
            {
                sN = (-d + b);
                sD = a;
            }
        }
        
        sc = (Mathf.Abs(sN) < 0.0001f ? 0.0f : sN / sD);
        tc = (Mathf.Abs(tN) < 0.0001f ? 0.0f : tN / tD);
        
        Vector2 dP = w + (sc * u) - (tc * v);
        return dP.magnitude;
    }
    
    void AddLineToActive(Line line)
    {
        activeLines.Add(line);
    }
    
    void RemoveLineFromActive()
    {
        if (activeLines.Contains(currentLine))
        {
            activeLines.Remove(currentLine);
        }
    }

    void OnDestroy()
    {
        // Убираем линию при уничтожении объекта
        RemoveLineFromActive();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !hasCat)
        {
            hasCat = true;
            Debug.Log("Player collide");

            GuysCount--;
            //GameObject.Find("CoreGame").SendMessage("Pause");
        }

        if (collision.gameObject.tag == "Guy")
        {
            Debug.Log("Guy collide");

            //GameObject.Find("CoreGame").SendMessage("Pause");
        }
    }
    
    public bool IsNPC()
    {
        return isNPC;
    }

}
