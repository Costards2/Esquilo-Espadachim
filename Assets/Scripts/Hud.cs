using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hud : MonoBehaviour
{
    public GameObject painelControles;
    public GameObject painelPrincipal;

    private void Awake()
    {
        Cursor.visible = true;
    }

    public void Jogar()
    {
        SceneManager.LoadScene("Game");
    }

    public void Controles()
    {
        painelControles.SetActive(true);
        painelPrincipal.SetActive(false);
    }

    public void Voltar()
    {
        painelControles.SetActive(false);
        painelPrincipal.SetActive(true);
    }

    public void Sair()
    {
        Debug.Log("Sair");
        Application.Quit();
    }
}
