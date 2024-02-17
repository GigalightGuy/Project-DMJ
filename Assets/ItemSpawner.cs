using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_ObjectToSpawn;
    [SerializeField] private GameObject m_ObjectToSpawn2;

    private float m_SpawnTimer = 0f;
    private bool m_Spawning = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            m_Spawning = !m_Spawning;
        }

        if (!m_Spawning) return;
        if (m_SpawnTimer > Time.time) return;
        m_SpawnTimer = Time.time + 0.5f;
        Vector3 randomOffset = new(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
        Instantiate(m_ObjectToSpawn, transform.position + randomOffset, Quaternion.identity);

        Vector3 randomOffset2 = new(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
        Instantiate(m_ObjectToSpawn2, transform.position + randomOffset2, Quaternion.identity);
    }
}
