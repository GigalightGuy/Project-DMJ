using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Terrain.MarchingCubes
{
    public class DensityGenerator : MonoBehaviour
    {
        [SerializeField] private MCTerrainSO _terrainSO;
        [SerializeField] private ComputeShader _densityShader;

        private ComputeBuffer _gradients;
        private ComputeBuffer _noisePoints;

        private static Vector2 GetRandomDirection() => new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
       

        Vector3Int _numThreadsPerAxis;

        public void SetDensityShader(ComputeBuffer chunkLocalPointsBuffer, Vector3Int numThreadsPerAxis)
        {
            _numThreadsPerAxis = numThreadsPerAxis;


            _densityShader.SetInt("nPointsX", _terrainSO.NumPointsPerAxis.x);
            _densityShader.SetInt("nPointsY", _terrainSO.NumPointsPerAxis.y);
            _densityShader.SetInt("nPointsZ", _terrainSO.NumPointsPerAxis.z);

            _densityShader.SetVector("voxelBoundsSize", new Vector4(_terrainSO.BoundsSize.x, _terrainSO.BoundsSize.y, _terrainSO.BoundsSize.z));
            //_densityShader.SetVector("voxelBoundsSize", _terrainSO.BoundsSize);
            _densityShader.SetVector("halfPointsBoundsSize", _terrainSO.BoundsPointsSize / 2);
            _densityShader.SetFloat("spacing", _terrainSO.Spacing);

            _densityShader.SetFloat("noiseRes", _terrainSO.noiseResolution);
            _densityShader.SetFloat("aux", _terrainSO.aux);




            _densityShader.SetBuffer(2, "chunkPoints", chunkLocalPointsBuffer);
            _densityShader.SetBuffer(3, "chunkPoints", chunkLocalPointsBuffer);

            _gradients?.Dispose();
            _gradients = new(256, sizeof(float) * 2);
            _gradients.SetData(Enumerable.Range(0, 256).Select((i) => GetRandomDirection()).ToArray());
            _densityShader.SetBuffer(0, "gradients", _gradients);

            _noisePoints?.Dispose();
            _noisePoints = new ComputeBuffer(_terrainSO.NumPointsPerAxis.x * _terrainSO.NumPointsPerAxis.z, sizeof(float));

        }

        public void Generate(ComputeBuffer _densities, ComputeBuffer chunkLocalPointsBuffer, Vector3Int chunkCoords)
        {
            //_densityShader.SetInts("chunkCoords", new int[] { chunkCoords.x, chunkCoords.y, chunkCoords.z });
            _densityShader.SetVector("centre", new Vector4(_terrainSO.BoundsSize.x * chunkCoords.x, _terrainSO.BoundsSize.y * chunkCoords.y, _terrainSO.BoundsSize.z * chunkCoords.z));

            _densityShader.SetBuffer(0, "noisePoints", _noisePoints);
            _densityShader.Dispatch(0, _numThreadsPerAxis.x, 1, _numThreadsPerAxis.z);

            _densityShader.SetBuffer(1, "densities", _densities);
            _densityShader.SetBuffer(1, "chunkPoints", chunkLocalPointsBuffer);
            _densityShader.SetBuffer(1, "noisePoints", _noisePoints);
            _densityShader.Dispatch(1, _numThreadsPerAxis.x, _numThreadsPerAxis.y, _numThreadsPerAxis.z);
        }

        public void Carve(ComputeBuffer densitiesBuffer, Vector3 carvingPoint,float carvingRange,int value, bool smooth)
        {
            _densityShader.SetVector("carvingPosition", carvingPoint);
            _densityShader.SetFloat("carvingRange", carvingRange);
            _densityShader.SetInt("addRemove", value);

            if (smooth)
            {
                _densityShader.SetBuffer(3, "densities", densitiesBuffer);
                _densityShader.Dispatch(3, _numThreadsPerAxis.x, _numThreadsPerAxis.y, _numThreadsPerAxis.z);
            }
            else
            {
                _densityShader.SetBuffer(2, "densities", densitiesBuffer);
                _densityShader.Dispatch(2, _numThreadsPerAxis.x, _numThreadsPerAxis.y, _numThreadsPerAxis.z);
            }
        }


        private void OnDisable()
        {
            _gradients?.Release();
            _noisePoints?.Release();
        }

    }
}