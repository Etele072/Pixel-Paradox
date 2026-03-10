using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    EnemyPatrol[] enemies;

    void Start()
    {
        enemies = FindObjectsOfType<EnemyPatrol>();
    }

    public void ResetEnemies()
    {
        foreach (EnemyPatrol enemy in enemies)
        {
            enemy.ResetEnemy();
        }
    }
}