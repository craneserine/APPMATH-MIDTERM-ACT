using UnityEngine;

public class BasicTower : TowerBehavior
{
    [SerializeField] private float baseBulletSpeed = 4f; // Default bullet speed
    [SerializeField] private float baseTurretRange = 4f; // Default turret range

    protected override void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (target.position - transform.position).normalized;

        Bullet bulletBehavior = bullet.GetComponent<Bullet>();
        if (bulletBehavior != null)
        {
            bulletBehavior.Initialize(direction, bulletSpeed, target, bulletKillDistance);
        }
    }

    public void ApplyUpgrades(int speedLevel, int rangeLevel)
    {
        // Set bullet speed based on speed level
        bulletSpeed = baseBulletSpeed + (speedLevel - 1) * 1f; // Each level adds 1 to speed

        // Set turret range based on range level
        turretRange = baseTurretRange + (rangeLevel - 1) * 0.5f; // Each level adds 0.5 to range
    }
}