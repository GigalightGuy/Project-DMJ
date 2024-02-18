using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.MarchingCubes
{
    public class ModifyTerrainProjectile : MonoBehaviour
    {
        public static float CarvingSize = 3f;
        [SerializeField] private LayerMask _ChunksMask;
        [SerializeField] private bool carvingMode;

        private void Start()
        {
            CarvingSize = 3f;

            Invoke(nameof(kill), 10);
        }

        public void kill()
        {
            Destroy(this.gameObject);
        }

        void TryCarving(bool addRemove)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, CarvingSize, _ChunksMask);

            if (cols.Length < 1) return;

            foreach (Collider hitCol in cols)
            {
                MonoBehaviour[] allScripts = hitCol.transform.parent.gameObject.GetComponentsInChildren<MonoBehaviour>();
                for (int i = 0; i < allScripts.Length; i++)
                {
                    if (allScripts[i] is IChunk)
                    {
                        (allScripts[i] as IChunk).CarveDensities(transform.position, CarvingSize, addRemove, false);
                    }
                }

            }

            Destroy(this.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            TryCarving(carvingMode);
        }
    }
}