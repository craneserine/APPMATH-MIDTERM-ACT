using UnityEngine;

public abstract class TowerBehavior : MonoBehaviour
{
    [SerializeField] protected Transform target;
    [SerializeField] protected float turretRange = 4f;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected float bulletSpeed = 4f;
    [SerializeField] protected float fireInterval = 0.7f;
    [SerializeField] protected float bulletKillDistance = 0.8f;

    // Allowable angle tolerance for firing the turret
    [SerializeField] protected float maxRotationSpeed = 360f; // Maximum rotation speed in degrees per second
    [SerializeField] protected float fireAngleThreshold = 10f; // Angle in degrees that must be within range to fire

    protected float fireTimer = 0f;

    protected virtual void Update()
    {
        if (target == null || !IsTargetInRange())
        {
            FindTarget();
        }

        if (target != null)
        {
            RotateTowardsTarget();
            fireTimer += Time.deltaTime;
            CheckRange();
        }
    }

    protected void RotateTowardsTarget()
    {
        // Compute the angle to target
        Vector2 direction = target.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Gradually rotate towards the target angle
        float currentAngle = transform.eulerAngles.z;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        // Interpolate the rotation to make it smooth
        float angleToRotate = Mathf.Sign(angleDifference) * Mathf.Min(Mathf.Abs(angleDifference), maxRotationSpeed * Time.deltaTime);
        float newAngle = currentAngle + angleToRotate;

        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    protected void CheckRange()
    {
        if (target == null) return;

        float dx = transform.position.x - target.position.x;
        float dy = transform.position.y - target.position.y;
        float distanceSquared = dx * dx + dy * dy;

        // Only fire if the target is in range and the turret is aimed in the correct direction
        if (distanceSquared <= turretRange * turretRange && fireTimer >= fireInterval)
        {
            Vector2 directionToTarget = target.position - transform.position;
            float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            float currentAngle = transform.eulerAngles.z;

            // Check if the turret is within the firing angle threshold
            if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) <= fireAngleThreshold)
            {
                FireBullet();
                fireTimer = 0f;
            }
        }
    }

    protected abstract void FireBullet();

    protected void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closestEnemy = null;
        float closestDistanceSquared = turretRange * turretRange;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float dx = transform.position.x - enemy.transform.position.x;
            float dy = transform.position.y - enemy.transform.position.y;
            float distanceSquared = dx * dx + dy * dy;

            if (distanceSquared <= closestDistanceSquared)
            {
                closestEnemy = enemy.transform;
                closestDistanceSquared = distanceSquared;
            }
        }

        target = closestEnemy;
    }

    protected bool IsTargetInRange()
    {
        if (target == null) return false;

        float dx = transform.position.x - target.position.x;
        float dy = transform.position.y - target.position.y;
        float distanceSquared = dx * dx + dy * dy;

        return distanceSquared <= turretRange * turretRange;
    }

    public void UpgradeSpeed(float speedIncrease)
    {
        bulletSpeed += speedIncrease;
    }

    public void UpgradeRange(float rangeIncrease)
    {
        turretRange += rangeIncrease;
    }
}
