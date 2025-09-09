using System.Collections;
using UnityEngine;

public class AudioManagerMuseu : MonoBehaviour
{
    [Header("�udios")]
    public AudioSource audio1; // �udio que toca no in�cio
    public AudioSource audio2; // �udio que toca quando o objeto � ativado

    [Header("GameObject Trigger")]
    public GameObject triggerObject;

    [Header("Configura��es")]
    public float delayAfterDeactivation = 2f; // Delay antes de voltar o audio1

    private float audio1Position = 0f;
    private bool audio1WasPlaying = false;

    private void Start()
    {
        // Come�a tocando o �udio 1
        if (audio1 != null)
        {
            audio1.Play();
        }

        // Checa a ativa��o do GameObject
        StartCoroutine(CheckTrigger());
    }

    private IEnumerator CheckTrigger()
    {
        bool lastState = triggerObject.activeSelf;

        while (true)
        {
            bool currentState = triggerObject.activeSelf;

            // Se mudou o estado
            if (currentState != lastState)
            {
                if (currentState) // Ativado
                {
                    // Para o audio1 e salva posi��o
                    if (audio1.isPlaying)
                    {
                        audio1Position = audio1.time;
                        audio1.Stop();
                    }

                    // Toca audio2
                    if (audio2 != null)
                        audio2.Play();
                }
                else // Desativado
                {
                    // Para audio2
                    if (audio2.isPlaying)
                        audio2.Stop();

                    // Espera 2 segundos e volta audio1 da posi��o salva
                    StartCoroutine(ResumeAudio1WithDelay());
                }

                lastState = currentState;
            }

            yield return null;
        }
    }

    private IEnumerator ResumeAudio1WithDelay()
    {
        yield return new WaitForSeconds(delayAfterDeactivation);

        if (audio1 != null)
        {
            audio1.time = audio1Position;
            audio1.Play();

            // Loop manual caso o �udio termine
            StartCoroutine(LoopAudio1());
        }
    }

    private IEnumerator LoopAudio1()
    {
        while (audio1.isPlaying == false)
        {
            audio1.Play();
            yield return new WaitForSeconds(audio1.clip.length - audio1.time);
        }
    }
}
