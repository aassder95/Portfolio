using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Fields
    StageTemplate m_stageTemplate;
    bool m_isEndSpawn;
    Coroutine m_spawnEnemiesCoroutine;
    #endregion
    #region Properties
    public bool IsEndSpawn => m_isEndSpawn;
    #endregion
    #region Control
    public void StartSpawning(bool isBoss = false)
    {
        StopSpawning();

        m_stageTemplate = Managers.Stage.Template;

        m_isEndSpawn = false;
        m_spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies(isBoss));
    }

    public void StopSpawning()
    {
        if (m_spawnEnemiesCoroutine != null)
        {
            StopCoroutine(m_spawnEnemiesCoroutine);
            m_spawnEnemiesCoroutine = null;
        }
    }
    #endregion
    #region Coroutines
    IEnumerator SpawnEnemies(bool isBoss)
    {
        System.Random random = new System.Random();

        int enemyCount = 0;
        int specialCount = 0;

        for (int i = 0; i < m_stageTemplate.TotalEnemyCount; i++)
        {
            bool isSpecial = false;

            if (enemyCount >= m_stageTemplate.EnemyCount ||
                random.Next(0, 2) == 0 && specialCount < m_stageTemplate.SpecialEnemyCount)
                isSpecial = true;

            if (isSpecial)
                specialCount++;
            else
                enemyCount++;

            Managers.Enemy.Spawn(isSpecial ? m_stageTemplate.SpecialEnemyId : m_stageTemplate.EnemyId).transform.SetParent(transform);

            if (enemyCount == m_stageTemplate.EnemyCount && specialCount == m_stageTemplate.SpecialEnemyCount)
            {
                m_isEndSpawn = true;
                break;
            }

            yield return new WaitForSeconds(2.0f);
        }

        if (isBoss)
        {
            Managers.UI.FindUIScene<UIGame>().StartBlinkBoss();
            Managers.Enemy.Spawn(m_stageTemplate.BossId).transform.SetParent(transform);
        }

        StopSpawning();
    }
    #endregion
}