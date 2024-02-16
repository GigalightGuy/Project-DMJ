using UnityEngine;

namespace Terrain.MarchingCubes
{
    public class ChunkMesh : MonoBehaviour, IChunk
    {

        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private Vector3Int _chunkCoord;

        public MeshFilter MeshFilter { get => _meshFilter; set => _meshFilter = value; }
        public Vector3Int ChunkCoord { get => _chunkCoord; set => _chunkCoord = value; }
        public MeshCollider MeshCollider { get => _meshCollider; set => _meshCollider = value; }

        TerrainMC _ctx;

        ComputeBuffer _densitiesBuffer;

   

        public void NewChunk(TerrainMC ctx, DensityGenerator generator)
        {
            _ctx = ctx;
            _densitiesBuffer = new ComputeBuffer(_ctx.TerrainSO.TotalPoints, sizeof(float));
            generator.Generate(_densitiesBuffer, _ctx.ChunkLocalPointsBuffer, _ctx.NoisePoints, _chunkCoord);
        }


        public void DrawChunk()
        {
            _ctx.TriangleBuffer.SetCounterValue(0);
            _ctx.MarchingCubesCS.SetBuffer(0, "points", _ctx.ChunkLocalPointsBuffer);
            _ctx.MarchingCubesCS.SetBuffer(0, "densities", _densitiesBuffer);
            _ctx.MarchingCubesCS.SetBuffer(0, "triangles", _ctx.TriangleBuffer);
            _ctx.MarchingCubesCS.SetInt("xAxis", _ctx.TerrainSO.NumPointsPerAxis.x);
            _ctx.MarchingCubesCS.SetInt("yAxis", _ctx.TerrainSO.NumPointsPerAxis.y);
            _ctx.MarchingCubesCS.SetInt("zAxis", _ctx.TerrainSO.NumPointsPerAxis.z);
            _ctx.MarchingCubesCS.SetFloat("isoLevel", _ctx.TerrainSO.IsoLevel);


            Vector3Int numThreadsPerAxis = _ctx.NumThreadsPerAxis;
            _ctx.MarchingCubesCS.Dispatch(0, numThreadsPerAxis.x, numThreadsPerAxis.y, numThreadsPerAxis.z);


            // Get number of triangles in the triangle buffer
            ComputeBuffer.CopyCount(_ctx.TriangleBuffer, _ctx.TriCountBuffer, 0);
            int[] triCountArray = { 0 };
            _ctx.TriCountBuffer.GetData(triCountArray);
            int numTris = triCountArray[0];

            // Get triangle data from shader
            Triangle[] tris = new Triangle[numTris];
            _ctx.TriangleBuffer.GetData(tris, 0, 0, numTris);


            var vertices = new Vector3[numTris * 3];
            var meshTriangles = new int[numTris * 3];

            for (int i = 0; i < numTris; i++)
                for (int j = 0; j < 3; j++)
                {
                    meshTriangles[i * 3 + j] = i * 3 + j;
                    vertices[i * 3 + j] = tris[i][j];
                }

            _meshFilter.mesh.Clear();
            _meshFilter.mesh.vertices = vertices;
            _meshFilter.mesh.triangles = meshTriangles;
            _meshFilter.mesh.RecalculateNormals();
            _meshCollider.sharedMesh = _meshFilter.sharedMesh;
        }




        public void UpdateDensities(Vector3 pos, float carvingRange, bool addRomove, bool smooth)
        {
            Vector3 boundsSize = _ctx.TerrainSO.BoundsSize;
            Vector3 carvingPoint = pos - new Vector3(boundsSize.x * _chunkCoord.x, boundsSize.y * _chunkCoord.y, boundsSize.z * _chunkCoord.z);

            int value = (addRomove) ? 1 : -1;

            _ctx.Generator.Carve(_densitiesBuffer, carvingPoint, carvingRange, value, smooth);

            DrawChunk();
        }


        struct Triangle
        {
#pragma warning disable 649 // disable unassigned variable warning
            public Vector3 a;
            public Vector3 b;
            public Vector3 c;

            public readonly Vector3 this[int i]
            {
                get
                {
                    return i switch
                    {
                        0 => a,
                        1 => b,
                        _ => c,
                    };
                }
            }
        }
    }
}