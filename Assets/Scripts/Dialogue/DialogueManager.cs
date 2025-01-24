using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public Dialogue firstDialogue;
    public Dialogue secondDialogue;
    public GameObject objectToActivateAfterFirst;
    public GameObject objectToDeactivateAfterSecond;
    public AudioSource audioSourceForSecondDialogue;

    private bool firstDialogueCompleted = false;
    private bool secondDialogueCompleted = false;

    void Start()
    {
        firstDialogue.gameObject.SetActive(true);
        secondDialogue.gameObject.SetActive(false);
        objectToActivateAfterFirst.SetActive(false);
        objectToDeactivateAfterSecond.SetActive(false);
        audioSourceForSecondDialogue.gameObject.SetActive(false); // Asegurarse de que el AudioSource est� desactivado
    }

    void Update()
    {
        // Si el primer di�logo se ha completado y no se ha procesado antes
        if (!firstDialogueCompleted && !Dialogue.isDialogueActive && firstDialogue.gameObject.activeSelf == false)
        {
            firstDialogueCompleted = true;
            ActivateSecondDialogue();
        }

        // Si el segundo di�logo se ha completado y no se ha procesado antes
        if (!secondDialogueCompleted && !Dialogue.isDialogueActive && secondDialogue.gameObject.activeSelf == false)
        {
            secondDialogueCompleted = true;
            DeactivateAfterSecondDialogue();
        }
    }

    void ActivateSecondDialogue()
    {
        // Activar el GameObject que deber�a aparecer tras el primer di�logo
        objectToActivateAfterFirst.SetActive(true);

        // Activar el segundo di�logo
        secondDialogue.gameObject.SetActive(true);
        secondDialogue.StartDialogue();

        // Activar el AudioSource para el segundo di�logo
        audioSourceForSecondDialogue.gameObject.SetActive(true);
        audioSourceForSecondDialogue.Play();
    }

    void DeactivateAfterSecondDialogue()
    {
        // Desactivar el GameObject despu�s de que el segundo di�logo termine
        objectToDeactivateAfterSecond.SetActive(false);

        // Detener y desactivar el AudioSource para el segundo di�logo
        audioSourceForSecondDialogue.Stop();
        audioSourceForSecondDialogue.gameObject.SetActive(false);
    }
}
