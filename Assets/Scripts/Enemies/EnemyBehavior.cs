using UnityEngine;

public enum MovementType { Quadratic, Cubic, DoubleCubic }

public class EnemyBehavior : MonoBehaviour
{
    private Transform target;
    private MovementType movementType;
    private float startTime;
    private Vector3 startPoint;
    private Vector3 control1, control2, midPoint, control3, control4;

    [SerializeField] private float quadraticSpeed = 4f;  
    [SerializeField] private float cubicSpeed = 4f;      
    [SerializeField] private float quadraticJourneyTime = 4.5f; 
    [SerializeField] private float cubicJourneyTime = 5f;   
    
    public void Initialize(Transform targetLocation, MovementType moveType)
    {
        target = targetLocation;
        movementType = moveType;
        startPoint = transform.position;
        startTime = Time.time;
        
        float offsetX = (target.position - startPoint).magnitude * 0.13f;
        float offsetY = offsetX * 1.5f;

        control1 = startPoint + new Vector3(offsetX, -offsetY * 1f, 0);
        control2 = startPoint + new Vector3(offsetX * 2, offsetY * 2.5f, 0);
        midPoint = (startPoint + target.position) / 2;
        control3 = startPoint + new Vector3(offsetX * 4, -offsetY * 1f, 0);
        control4 = startPoint + new Vector3(offsetX * 6, offsetY * 2.5f, 0);
    }

    void Update()
    {
        float journeyTime = (movementType == MovementType.Quadratic) ? quadraticJourneyTime : cubicJourneyTime;
        float speed = (movementType == MovementType.Quadratic) ? quadraticSpeed : cubicSpeed;
        
        float t = (Time.time - startTime) / journeyTime;
        t = Mathf.Clamp01(t);

        if (t >= 1f)
        {
            ReachTarget();
            return;
        }

        switch (movementType)
        {
            case MovementType.Quadratic:
                transform.position = QuadraticBezier(startPoint, control1, target.position, t);
                break;
            case MovementType.Cubic:
                transform.position = CubicBezier(startPoint, control1, control2, target.position, t);
                break;
            case MovementType.DoubleCubic:
                transform.position = DoubleCubicBezier(startPoint, control1, control2, midPoint, control3, control4, target.position, t);
                break;
        }
    }

    private Vector3 QuadraticBezier(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        return Mathf.Pow(1 - t, 2) * start +
               2 * (1 - t) * t * control +
               t * t * end;
    }

    private Vector3 CubicBezier(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
    {
        return Mathf.Pow(1 - t, 3) * start +
               3 * Mathf.Pow(1 - t, 2) * t * control1 +
               3 * (1 - t) * t * t * control2 +
               t * t * t * end;
    }
    
    private Vector3 DoubleCubicBezier(Vector3 start, Vector3 control1, Vector3 control2, Vector3 midPoint,
        Vector3 control3, Vector3 control4, Vector3 end, float t)
    {
        if (t < 0.5f)
        {
            float newT = t * 2;
            return CubicBezier(start, control1, control2, midPoint, newT);
        }
        else
        {
            float newT = (t - 0.5f) * 2;
            return CubicBezier(midPoint, control3, control4, end, newT);
        }
    }

    private void ReachTarget()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage();
            Destroy(gameObject);
        }
        
    }
    
}