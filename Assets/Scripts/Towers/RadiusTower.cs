using UnityEngine;
using System.Collections;

public class RadiusTower : MonoBehaviour
{
    [Header("Radius Tower Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireInterval = 2f; 
    [SerializeField] private int bulletCount = 4; 
    [SerializeField] private float bulletSpeed = 2f; 
    [SerializeField] private float bulletLifetime = 3f; 
    [SerializeField] private float bulletKillDistance = 0.8f; 
    [SerializeField] private float range = 5f; // Default range value

    private void Start()
    {
        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            FireBullets();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void FireBullets()
    {
        float angleIncrement = (2 * Mathf.PI) / bulletCount; // Divide 360 degrees (2π radians) by the bullet count

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleIncrement; // Calculate the angle for each bullet
        
            // Convert angle to direction using Mathf.Cos and Mathf.Sin to get a vector in the direction
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)); 

            // Instantiate the bullet at the tower's position
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletBehavior = bullet.GetComponent<Bullet>();

            if (bulletBehavior != null)
            {
                // Find the closest enemy and assign it as the target to the bullet
                Transform target = FindClosestEnemy();
                bulletBehavior.Initialize(direction, bulletSpeed, target, bulletKillDistance);
            }

            // Destroy the bullet after the specified lifetime
            Destroy(bullet, bulletLifetime);
        }
    }


    private Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closestEnemy = null;
        float closestDistanceSquared = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float dx = transform.position.x - enemy.transform.position.x;
            float dy = transform.position.y - enemy.transform.position.y;
            float distanceSquared = dx * dx + dy * dy;

            if (distanceSquared < closestDistanceSquared)
            {
                closestEnemy = enemy.transform;
                closestDistanceSquared = distanceSquared;
            }
        }

        return closestEnemy;
    }

    public void IncreaseBulletCount(int amount)
    {
        bulletCount += amount;
    }
    
    public void UpgradeSpeed(float speedIncrease)
    {
        bulletSpeed += speedIncrease;
    }
    
    public void UpgradeRange(float rangeIncrease)
    {
        range += rangeIncrease; // Increase the range by the specified amount
    }

}