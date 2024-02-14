
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DensityGenerator : MonoBehaviour
{
    [SerializeField] MCTerrainSO _terrainSO;
    const int threadGroupSize = 8;
    public ComputeShader _densityShader;
    public ComputeShader _noiseShader;

    ComputeBuffer _gradients;

    Vector3 _boundsSize;


    private static Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
    }

    Vector3Int _numThreadsPerAxis;

    public void SetUpGradientsInformation()
    {
        _gradients?.Dispose();
        _gradients = new(256, sizeof(float) * 2);
        _gradients.SetData(Enumerable.Range(0, 256).Select((i) => GetRandomDirection()).ToArray());
        _densityShader.SetBuffer(0, "gradients", _gradients);
        _noiseShader.SetBuffer(0, "gradients", _gradients);

       
    }
   
    public void SetComputeShaderDefaultParameters() {

        _boundsSize = _terrainSO.BoundsSize;
        _densityShader.SetVector("boundsSize", new Vector4(_boundsSize.x, _boundsSize.y, _boundsSize.z));
        _densityShader.SetFloat("spacing", _terrainSO.Spacing);
        _densityShader.SetFloat("noiseRes", _terrainSO.noiseResolution);
        _densityShader.SetFloat("aux", _terrainSO.aux);
        _densityShader.SetInt("nPointsX", _terrainSO.NumPointsPerAxis.x);
        _densityShader.SetInt("nPointsY", _terrainSO.NumPointsPerAxis.y);
        _densityShader.SetInt("nPointsZ", _terrainSO.NumPointsPerAxis.z);

        _noiseShader.SetInt("nPointsX", _terrainSO.NumPointsPerAxis.x);
        _noiseShader.SetInt("nPointsY", _terrainSO.NumPointsPerAxis.y);
        _noiseShader.SetInt("nPointsZ", _terrainSO.NumPointsPerAxis.z);
        _noiseShader.SetVector("boundsSize", new Vector2(_boundsSize.x,  _boundsSize.z));
        _noiseShader.SetFloat("spacing", _terrainSO.Spacing);
        _noiseShader.SetFloat("noiseRes", _terrainSO.noiseResolution);
        _noiseShader.SetFloat("aux", _terrainSO.aux);
    }

    public ComputeBuffer Generate(ComputeBuffer _densities, ComputeBuffer chunkLocalPointsBuffer, ComputeBuffer noisePoints , Vector3Int chunkCoords)
    {

        SetComputeShaderDefaultParameters();

        _numThreadsPerAxis = new(
            Mathf.CeilToInt(_terrainSO.NumPointsPerAxis.x / (float)threadGroupSize),
            Mathf.CeilToInt(_terrainSO.NumPointsPerAxis.y / (float)threadGroupSize),
            Mathf.CeilToInt(_terrainSO.NumPointsPerAxis.z / (float)threadGroupSize)
            );

        _densityShader.SetInts("chunkCoords", new int[] { chunkCoords.x, chunkCoords.y, chunkCoords.z });
        _densityShader.SetVector("centre", new Vector4(_boundsSize.x * chunkCoords.x, _boundsSize.y * chunkCoords.y, _boundsSize.z * chunkCoords.z));
        
        _noiseShader.SetVector("centre", new Vector2(_boundsSize.x * chunkCoords.x,  _boundsSize.z * chunkCoords.z));



        _noiseShader.SetBuffer(0, "noisePoints", noisePoints);
        _noiseShader.Dispatch(0, _numThreadsPerAxis.x,1, _numThreadsPerAxis.z);
        
        _densityShader.SetBuffer(0, "densities", _densities);
        _densityShader.SetBuffer(0, "chunkPoints", chunkLocalPointsBuffer);
        _densityShader.SetBuffer(0, "noisePoints", noisePoints);
        _densityShader.Dispatch(0, _numThreadsPerAxis.x, _numThreadsPerAxis.y, _numThreadsPerAxis.z);

   

        //float[] aux2 = new float[7 * 7];
        //noisePoints.GetData(aux2, 0, 0, noisePoints.count);


        return _densities;
    }
}
