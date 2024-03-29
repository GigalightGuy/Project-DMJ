using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Terrain.MarchingCubes
{


    public class TerrainMC : MonoBehaviour
    {
        const int threadGroupSize = 8;

        private Vector3Int _numThreadsPerAxis;

        [SerializeField] private MCTerrainSO _terrainSO;
        [SerializeField] private DensityGenerator _generator;
        [SerializeField] private ComputeShader _marchingCubesCS;
        [SerializeField] private Material _debugMaterial;

        private ChunkMesh[] _chunks;

        private ComputeBuffer _chunkLocalPointsBuffer;
        private ComputeBuffer _triangleBuffer;
        private ComputeBuffer _triCountBuffer;

       

        public ComputeBuffer ChunkLocalPointsBuffer { get => _chunkLocalPointsBuffer; }
        public ComputeBuffer TriangleBuffer { get => _triangleBuffer; set => _triangleBuffer = value; }
        public ComputeBuffer TriCountBuffer { get => _triCountBuffer; set => _triCountBuffer = value; }
        public MCTerrainSO TerrainSO { get => _terrainSO; }
        public ComputeShader MarchingCubesCS { get => _marchingCubesCS; }
        public DensityGenerator Generator { get => _generator; set => _generator = value; }
        public Vector3Int NumThreadsPerAxis { get => _numThreadsPerAxis; set => _numThreadsPerAxis = value; }


        private void Start()
        {
            CreateBuffers();

            CreateLocalChunkPoints();

            NewChunks();

            _generator.SetDensityShader(_chunkLocalPointsBuffer, _numThreadsPerAxis);

            SetChunks();

            UpdateChunksMesh();

            void someDebugs()
            {
                Vector3Int points = _terrainSO.NumPointsPerAxis;

                Debug.Log("Points per Axis " + points);
                Debug.Log("Points per chunk " + points.x * points.y * points.z);
                Debug.Log("Chunks per Axis " + _terrainSO.NumChunksPerAxis);
                Debug.Log("Total Chunks " + _terrainSO.NumChunksPerAxis.x * _terrainSO.NumChunksPerAxis.y * _terrainSO.NumChunksPerAxis.z);
                Debug.Log("Total Points  " + points.x * _terrainSO.NumChunksPerAxis.x * points.y * _terrainSO.NumChunksPerAxis.y * points.z * _terrainSO.NumChunksPerAxis.z);

            }
            someDebugs();
        }


        void CreateLocalChunkPoints()
        {
            int indexFromCoord(Vector3Int id)
            {
                return id.z * _terrainSO.NumPointsPerAxis.y * _terrainSO.NumPointsPerAxis.x + id.y * _terrainSO.NumPointsPerAxis.x + id.x;
            }

            Vector3[] points = new Vector3[_terrainSO.TotalPoints];
            Vector3 boudsSize = _terrainSO.BoundsSize;

            for (int x = 0; x < _terrainSO.NumPointsPerAxis.x; x++)
                for (int y = 0; y < _terrainSO.NumPointsPerAxis.y; y++)
                    for (int z = 0; z < _terrainSO.NumPointsPerAxis.z; z++)
                    {
                        Vector3Int id = new(x, y, z);
                        points[indexFromCoord(id)] = (Vector3)id * _terrainSO.Spacing - boudsSize / 2;
                    }

            _chunkLocalPointsBuffer.SetData(points);
        }

        void NewChunks()
        {
            for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

            int nChunks = _terrainSO.TotalChunks;
            _chunks = new ChunkMesh[nChunks];

            for (int c = 0; c < nChunks; c++)
            {
                GameObject g = new();
                g.layer = LayerMask.NameToLayer("Terrain");
                g.transform.parent = transform;

                ChunkMesh chunk = g.AddComponent<ChunkMesh>();
                chunk.MeshFilter = g.AddComponent<MeshFilter>();
                chunk.MeshCollider = g.AddComponent<MeshCollider>();
                _chunks[c] = chunk;

                GameObject child = new();
                child.transform.parent = g.transform;
                child.layer = LayerMask.NameToLayer("Chunk");
                BoxCollider box = child.AddComponent<BoxCollider>();
                child.name = "ChunkBoxCollider";
                box.isTrigger = true;
                box.center = Vector3.zero;
                box.size = _terrainSO.BoundsSize;
                g.AddComponent<MeshRenderer>().material = _debugMaterial;
            }
        }

        void SetChunks()
        {
            Vector3Int terrainChunks = _terrainSO.NumChunksPerAxis;
            for (int x = 0; x < terrainChunks.x; x++)
                for (int y = 0; y < terrainChunks.y; y++)
                    for (int z = 0; z < terrainChunks.z; z++)
                    {
                        int index = z * terrainChunks.y * terrainChunks.x + y * terrainChunks.x + x;
                        _chunks[index].MeshFilter.transform.position = new Vector3(_terrainSO.BoundsSize.x * x, _terrainSO.BoundsSize.y * y, _terrainSO.BoundsSize.z * z);
                        _chunks[index].MeshFilter.gameObject.name = x.ToString() + y.ToString() + z.ToString();
                        _chunks[index].ChunkCoord = new Vector3Int(x, y, z);
                        _chunks[index].NewChunk(this, _generator);
                    }
        }

        void UpdateChunksMesh()
        {
            foreach (var chunk in _chunks) chunk.DrawChunk();
        }


        #region buffer creation


        private void CreateBuffers()
        {
            _numThreadsPerAxis = new(
               Mathf.CeilToInt(_terrainSO.NumPointsPerAxis.x / (float)threadGroupSize),
               Mathf.CeilToInt(_terrainSO.NumPointsPerAxis.y / (float)threadGroupSize),
               Mathf.CeilToInt(_terrainSO.NumPointsPerAxis.z / (float)threadGroupSize)
               );

            if (!Application.isPlaying || (_chunkLocalPointsBuffer == null || _terrainSO.TotalPoints != _chunkLocalPointsBuffer.count))
            {
                if (Application.isPlaying)
                {
                    ReleaseBuffers();
                }
                _triangleBuffer = new ComputeBuffer(_terrainSO.TotalVoxels * 5, sizeof(float) * 3 * 3, ComputeBufferType.Append);
                _chunkLocalPointsBuffer = new ComputeBuffer(_terrainSO.TotalPoints, sizeof(float) * 3);
                _triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
            }
        }

        void ReleaseBuffers()
        {  
            _triangleBuffer?.Release();
            _chunkLocalPointsBuffer?.Release();
            _triCountBuffer?.Release();   
        }

        private void OnDisable()
        {
            ReleaseBuffers();
        }

        #endregion
    }
}