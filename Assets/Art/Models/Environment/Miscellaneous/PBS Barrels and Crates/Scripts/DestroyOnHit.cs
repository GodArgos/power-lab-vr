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

    // Update is called once per frame
    //void OnCollisionEnter(Collision col)
    //{
    //       if (col.gameObject.CompareTag("Weapon") && CheckVelocity(col.gameObject.GetComponent<EnableWeaponDestruction>().velocity, 3.5f))
    //	{
    //            instantiate the exploding barrel
    //           GameObject go = (GameObject)Instantiate(
    //               explodedPrefab,
    //               gameObject.transform.position,
    //               gameObject.transform.rotation
    //           );

    //            get the explosion component on the new object
    //           ExplodeBarrel explodeComp = go.GetComponent<ExplodeBarrel>();

    //            set desired properties
    //           explodeComp.explosionForce = explosionForce;
    //           explodeComp.explosionRadius = explosionRadius;
    //           explodeComp.upForceMin = upForceMin;
    //           explodeComp.upForceMax = upForceMax;
    //           explodeComp.autoDestroy = autoDestroy;
    //           explodeComp.lifeTime = lifeTime;

    //            make the barrel explode
    //           explodeComp.Explode();

    //            Sincroniza el objeto en la red
    //           NetworkServer.Spawn(go);

    //            destroy the nice barrel
    //           NetworkServer.Destroy(gameObject);
    //       }
    //}

    //   private bool CheckVelocity(Vector3 velocity, float minVelocity)
    //   {
    //       return velocity.x >= minVelocity ? true : (velocity.y >= minVelocity ? true : (velocity.z >= minVelocity ? true : false));
    //   }

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

        // Destruye el objeto actual en la red
        NetworkServer.Destroy(gameObject);
    }

    private bool CheckVelocity(Vector3 velocity, float minVelocity)
    {
        return velocity.x >= minVelocity || velocity.y >= minVelocity || velocity.z >= minVelocity;
    }
}
