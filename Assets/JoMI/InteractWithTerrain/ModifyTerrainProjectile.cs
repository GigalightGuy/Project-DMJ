using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyTerrainProjectile : MonoBehaviour
{

    [SerializeField] private LayerMask _ChunksMask;
    [SerializeField] private float CarvingSize;
    [SerializeField] private bool carvingMode;

    private void Start()
    {
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
                    (allScripts[i] as IChunk).UpdateDensities(transform.position, CarvingSize, addRemove);
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