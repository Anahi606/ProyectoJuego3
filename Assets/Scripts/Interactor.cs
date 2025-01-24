using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    public GameObject imageObject;
    public GameObject dialogueObject;

    private bool isNearObject = false;
    public bool hasInteracted = false;

    void Start()
    {
        imageObject.SetActive(false);
        dialogueObject.SetActive(false);
    }

    void Update()
    {
        //El player est� cerca del objeto
        if (isNearObject && !hasInteracted && Input.GetKeyDown(KeyCode.F))
        {
            dialogueObject.SetActive(true);
            imageObject.SetActive(false);
            hasInteracted = true;
        }
    }

    //El player entra en el �rea de colisi�n con el objeto interactuable
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            isNearObject = true;
            imageObject.SetActive(true);
        }
    }

    //El player sale del �rea de colisi�n
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearObject = false;
            imageObject.SetActive(false);
        }
    }
}
