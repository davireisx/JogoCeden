using System.Collections.Generic;
using UnityEngine;

public class InimigoManagerGlobal : MonoBehaviour
{
    public static InimigoManagerGlobal Instance;
    public List<GameObject> inimigosAtivos = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegistrarInimigo(GameObject inimigo)
    {
        if (!inimigosAtivos.Contains(inimigo))
            inimigosAtivos.Add(inimigo);
    }

    public void RemoverInimigo(GameObject inimigo)
    {
        if (inimigosAtivos.Contains(inimigo))
            inimigosAtivos.Remove(inimigo);
    }

    public bool TodosInimigosEliminados()
    {
        return inimigosAtivos.Count == 0;
    }
}
