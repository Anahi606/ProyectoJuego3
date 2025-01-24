using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossOneAbility : MonoBehaviour
{
    [SerializeField] private float da�o;
    [SerializeField] private Vector2 dimensionesCaja;
    [SerializeField] private Transform posicionCaja;
    [SerializeField] private float tiempoDeVida;
    private AudioSource _AudioSource;
    [SerializeField] private AudioClip abilitySound;


    private void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    public void Golpe()
    {

        Collider2D[] objetos = Physics2D.OverlapBoxAll(posicionCaja.position, dimensionesCaja, 0f);
        foreach (Collider2D colisiones in objetos)
        {
            if (colisiones.CompareTag("Player"))
            {
                colisiones.GetComponent<FightPlayer>().TomarDa�o(da�o);
                //FightPlayer.Instance.HitStopTime(0, 5, 0.5f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(posicionCaja.position, dimensionesCaja);
    }

}
