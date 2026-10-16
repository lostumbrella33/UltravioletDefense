using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Tower Defense/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelIndex;
    public string levelName;
    public Vector3[] tileCoordinates;
    public Vector3[] enemyPathNodes;

}
