using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "MC Terrain", order = 1)]
public class MCTerrainSO : ScriptableObject
{
    public Vector3Int NumPointsPerAxis;
    public float Spacing;
    public Vector3 BoundsSize { get => new Vector3(NumPointsPerAxis.x - 1, NumPointsPerAxis.y - 1, NumPointsPerAxis.z - 1) * Spacing; }
    public Vector3 BoundsPointsSize { get => new Vector3(NumPointsPerAxis.x , NumPointsPerAxis.y , NumPointsPerAxis.z) * Spacing; }
    public int TotalPoints { get => NumPointsPerAxis.x * NumPointsPerAxis.y * NumPointsPerAxis.z; }
    public int TotalVoxels{ get => (NumPointsPerAxis.x -1) *( NumPointsPerAxis.y-1) * (NumPointsPerAxis.z-1); }

    public float noiseResolution;
    public float aux;

   [Range(-1, 1)] public float IsoLevel;

    public Vector3Int NumChunksPerAxis;

}
