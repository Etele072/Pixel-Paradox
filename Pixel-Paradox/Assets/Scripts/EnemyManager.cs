using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyManager : MonoBehaviour
{
    EnemyPatrol[] enemies;

    void Start()
    {
        enemies = FindObjectsByType<EnemyPatrol>(FindObjectsSortMode.None);
    }

    public void ResetEnemies()
    {
        foreach (EnemyPatrol enemy in enemies)
        {
            enemy.ResetEnemy();
        }
    }
}