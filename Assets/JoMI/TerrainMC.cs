using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainMC : MonoBehaviour
{
    [SerializeField] MCTerrainSO _terrainSO;


    [SerializeField] DensityGenerator _generator;
    [SerializeField] ComputeShader _marchingCubesCS;
    [SerializeField] ComputeShader _carvingCS;


    [SerializeField] private Vector3Int _terrainChunks;
    ChunkMesh[] _chunks;

    [SerializeField] Material _debugMaterial;


    ComputeBuffer _chunkLocalPointsBuffer;
    ComputeBuffer _noisePoints;
    ComputeBuffer _triangleBuffer;
    ComputeBuffer _triCountBuffer;

    public ComputeBuffer ChunkLocalPointsBuffer { get => _chunkLocalPointsBuffer; }
    public ComputeBuffer TriangleBuffer { get => _triangleBuffer; set => _triangleBuffer = value; }
    public ComputeBuffer TriCountBuffer { get => _triCountBuffer; set => _triCountBuffer = value; }
    public ComputeBuffer NoisePoints { get => _noisePoints; set => _noisePoints = value; }
    public MCTerrainSO TerrainSO { get => _terrainSO; }
    public ComputeShader MarchingCubesCS { get => _marchingCubesCS;  }
    public ComputeShader CarvingCS { get => _carvingCS;  }


    private void Start()
    {
        CreateBuffers();

      
        createLocalChunkPoints();

        _generator.SetUpGradientsInformation();


        void someDebugs()
        {
            Vector3Int points = _terrainSO.NumPointsPerAxis;

            Debug.Log("Points per Axis " + points);
            Debug.Log("Points per chunk " + points.x * points.y * points.z);
            Debug.Log("Chunks per Axis " + _terrainChunks);
            Debug.Log("Total Chunks " + _terrainChunks.x * _terrainChunks.y * _terrainChunks.z);
            Debug.Log("Total Points  " + points.x * _terrainChunks.x * points.y * _terrainChunks.y * points.z * _terrainChunks.z);

        }
        someDebugs();
        
        void NewChunks()
        {
            for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

            int nChunks = _terrainChunks.x * _terrainChunks.y * _terrainChunks.z;
            _chunks = new ChunkMesh[nChunks];

            for (int c = 0; c < nChunks; c++)
            {
                GameObject g = new();
                ChunkMesh chunk = g.AddComponent<ChunkMesh>();
                g.transform.parent = transform;
                g.layer = LayerMask.NameToLayer("Terrain");
                chunk.MeshFilter = g.AddComponent<MeshFilter>();
                chunk.MeshCollider = g.AddComponent<MeshCollider>();

                GameObject child = new();
                child.transform.parent = g.transform;
                child.layer = LayerMask.NameToLayer("Chunk");
                BoxCollider box= child.AddComponent<BoxCollider>();
                box.isTrigger = true;
                box.center = Vector3.zero;
                box.size = _terrainSO.BoundsSize;
                g.AddComponent<MeshRenderer>().material = _debugMaterial;
                _chunks[c] = chunk;
            }
        }
       
        NewChunks();
        SetChunks();
        UpdateChunksMesh();
    }

    void createLocalChunkPoints()
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

    void SetChunks()
    {
        for (int x = 0; x < _terrainChunks.x; x++)
            for (int y = 0; y < _terrainChunks.y; y++)
                for (int z = 0; z < _terrainChunks.z; z++)
                {
                    int index = z * _terrainChunks.y * _terrainChunks.x + y * _terrainChunks.x + x;
                    _chunks[index].MeshFilter.transform.position = new Vector3(_terrainSO.BoundsSize.x * x, _terrainSO.BoundsSize.y * y, _terrainSO.BoundsSize.z * z);
                    _chunks[index].MeshFilter.gameObject.name = x.ToString() + y.ToString() + z.ToString();
                    _chunks[index].ChunkCoord = new Vector3Int(x, y, z);
                    _chunks[index].NewChunk(this, _generator);
                }
    }

    private bool updateMesh;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(Input.GetKey(KeyCode.L))_generator.SetUpGradientsInformation();
            createLocalChunkPoints();
            SetChunks();
            UpdateChunksMesh();
        }
        if (Input.GetKeyDown(KeyCode.U)) updateMesh = !updateMesh;
        if(updateMesh || Input.GetKeyDown(KeyCode.Y)) { Debug.Log("updating mesh");  
            UpdateChunksMesh();
        }
    }

    void UpdateChunksMesh()
    {
        foreach (var chunk in _chunks)
        { 
            chunk.DrawChunk();
        }
    }


    #region buffer creation

    private void CreateBuffers()
    {
        if (!Application.isPlaying || (_chunkLocalPointsBuffer == null || _terrainSO.TotalPoints != _chunkLocalPointsBuffer.count))
        {
            if (Application.isPlaying)
            {
                ReleaseBuffers();
            }
            
            _noisePoints = new ComputeBuffer(_terrainSO.NumPointsPerAxis.x * _terrainSO.NumPointsPerAxis.z, sizeof(float) );
            _triangleBuffer = new ComputeBuffer(_terrainSO.TotalVoxels * 5, sizeof(float) * 3 * 3, ComputeBufferType.Append);
            _chunkLocalPointsBuffer = new ComputeBuffer(_terrainSO.TotalPoints, sizeof(float) * 3);
            _triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        }
    }

    void ReleaseBuffers()
    {
        if (_triangleBuffer != null)
        {
            _noisePoints.Release();
            _triangleBuffer.Release();
            _chunkLocalPointsBuffer.Release();
            _triCountBuffer.Release();
        }
    }

    #endregion
}
