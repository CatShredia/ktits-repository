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

    [Header("Player Spawn")]
    [Tooltip("Позиция спавна игрока при выборе этого уровня")]
    public Vector3 playerSpawnPosition = Vector3.zero;

    [Header("Spawn Player Point")]
    [Tooltip("Точка спавна игрока (координаты). Если задана, используется вместо playerSpawnPosition.")]
    public Vector3 spawnPlayerPoint = Vector3.zero;

    [Tooltip("Использовать spawnPlayerPoint вместо playerSpawnPosition")]
    public bool useSpawnPlayerPoint = false;

    [Header("Distance Settings")]
    [Tooltip("Расстояние до игрока, на котором загружается уровень")]
    public float loadDistance = 10f;

    [Tooltip("Расстояние до игрока, на котором выгружается уровень")]
    public float unloadDistance = 15f;
}
