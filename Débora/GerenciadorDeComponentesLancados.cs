using UnityEngine;
using System.Collections.Generic;

public class GerenciadorDeComponentesLancados : MonoBehaviour
{
    public ComponenteLancado[] componentes;

    private List<ComponenteLancado> chegaram = new List<ComponenteLancado>();

    public void NotificarChegada(ComponenteLancado componente)
    {
        if (!chegaram.Contains(componente))
            chegaram.Add(componente);

        if (chegaram.Count == componentes.Length)
        {
            Debug.Log("? TODOS os componentes chegaram. Iniciando piscada.");
            foreach (var c in componentes)
            {
                c.AtivarPiscada();
            }
        }
    }
}
