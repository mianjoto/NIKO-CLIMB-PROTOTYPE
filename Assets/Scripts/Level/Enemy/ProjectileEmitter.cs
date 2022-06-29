using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEmitter : MonoBehaviour
{
    public GameObject projectilePrefab;

    public void Shoot() {
        if (this.gameObject == null)
            return;
        GameObject instance = Instantiate(projectilePrefab, transform.position, transform.rotation);
        instance.transform.SetParent(gameObject.transform);
    }

}
