
using UnityEngine;

namespace Terrain.MarchingCubes
{
    public interface IChunk
    {
        public void CarveDensities(Vector3 pos, float radius, bool addRomove, bool smooth);
    }
}