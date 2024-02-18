using System.Collections;
using UnityEngine;

namespace Terrain.MarchingCubes
{
    public class ModifyTerrain : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;


        void Start()
        {
            _mainCamera = GetComponent<Camera>();
            StartCoroutine(checkCarving());
        }

        IEnumerator checkCarving()
        {
            while (true)
            {
                if (Input.GetKey(KeyCode.Z)) tryCarving(true);
                if (Input.GetKey(KeyCode.X)) tryCarving(false);
                yield return new WaitForSeconds(0.05f);
            }
        }

        [SerializeField] private LayerMask _TerrainMask;
        [SerializeField] private LayerMask _ChunksMask;
        [SerializeField] private float CarvingSize;

        void tryCarving(bool addRemove)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000, _TerrainMask)) return;

            Collider[] cols = Physics.OverlapSphere(hit.point, CarvingSize, _ChunksMask);

            if (cols.Length < 1) return;

            foreach (Collider hitCol in cols)
            {
                MonoBehaviour[] allScripts = hitCol.transform.parent.gameObject.GetComponentsInChildren<MonoBehaviour>();
                for (int i = 0; i < allScripts.Length; i++)
                {
                    if (allScripts[i] is IChunk)
                    {
                        (allScripts[i] as IChunk).CarveDensities(hit.point, CarvingSize, addRemove, false);
                    }
                }

            }
        }
    }
}