using UnityEngine;
using System.Collections;
using Mirror;

public class DestroyOnHit : NetworkBehaviour 
{
	public GameObject explodedPrefab;

	public float explosionForce = 2.0f;
	public float explosionRadius = 5.0f;
	public float upForceMin = 0.0f;
	public float upForceMax = 0.5f;

	public bool autoDestroy = true;
	public float lifeTime = 5.0f;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Weapon") && CheckVelocity(col.gameObject.GetComponent<EnableWeaponDestruction>().velocity, 3.5f))
        {
            // Si es el servidor, destruye directamente
            if (isServer)
            {
                DestroyAndSpawn();
            }
            else
            {
                // Si es un cliente, envía una comanda al servidor
                CmdDestroyAndSpawn();
            }
        }
    }

    // Comanda que se ejecuta en el servidor cuando es llamada por un cliente
    [Command(requiresAuthority = false)]
    void CmdDestroyAndSpawn()
    {
        DestroyAndSpawn();
    }

    // Función que realiza la destrucción y el spawn del objeto roto
    void DestroyAndSpawn()
    {
        // Instancia el prefab roto
        GameObject go = Instantiate(
            explodedPrefab,
            transform.position,
            transform.rotation
        );

        // Configura los componentes necesarios
        ExplodeBarrel explodeComp = go.GetComponent<ExplodeBarrel>();

        explodeComp.explosionForce = explosionForce;
        explodeComp.explosionRadius = explosionRadius;
        explodeComp.upForceMin = upForceMin;
        explodeComp.upForceMax = upForceMax;
        explodeComp.autoDestroy = autoDestroy;
        explodeComp.lifeTime = lifeTime;

        // Haz que el barril explote
        explodeComp.Explode();

        // Sincroniza el objeto en la red
        NetworkServer.Spawn(go);

        go.GetComponent<SoundPlayer>().CmdPlaySoundForAll("crate_break_" + (int)Random.Range(1, 5));

        // Destruye el objeto actual en la red
        NetworkServer.Destroy(gameObject);
    }

    private bool CheckVelocity(Vector3 velocity, float minVelocity)
    {
        return velocity.x >= minVelocity || velocity.y >= minVelocity || velocity.z >= minVelocity;
    }
}
