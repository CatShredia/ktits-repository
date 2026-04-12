using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Prefab")]
    [Tooltip("Префаб уровня для спавна")]
    public GameObject levelPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Смещение позиции спавна уровня относительно триггера")]
    public Vector3 spawnOffset = Vector3.zero;

    [Header("Distance Settings")]
    [Tooltip("Расстояние до игрока, на котором загружается уровень")]
    public float loadDistance = 10f;

    [Tooltip("Расстояние до игрока, на котором выгружается уровень")]
    public float unloadDistance = 15f;
}
