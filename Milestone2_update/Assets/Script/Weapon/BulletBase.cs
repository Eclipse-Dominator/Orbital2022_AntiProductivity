using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public float lifeTime;
    public float bulletSpeed;
    static ChunkManager terrainModifier;
    Rigidbody bullet;

    private void Awake()
    {
        if (terrainModifier == null)
        {
            terrainModifier = GameObject.FindObjectOfType<ChunkManager>();
        }
    }

    void Start()
    {
        bullet = GetComponent<Rigidbody>(); 
        bullet.velocity = transform.forward * bulletSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTime <= 0)
        {
            Destroy(gameObject);    
        }
        lifeTime -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit!");
        terrainModifier.updateChunk(transform.position, 5);
        Destroy(gameObject);
    }
}
