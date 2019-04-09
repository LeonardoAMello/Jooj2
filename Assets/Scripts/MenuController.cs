using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    public GameObject menu, historia, comoJogar, configs, intro, transicao, creditos, fundoMenuEstatico, fundoMenuAnimado;
    public AudioMixer mixer;
    public Animator transitionAnimator;

    private void Start()
    {
        PlayerPrefs.SetFloat("Sensibility X", 75);
        PlayerPrefs.SetFloat("Sensibility Y", 20);

        if (QualitySettings.GetQualityLevel() >= 2)
        {
            fundoMenuAnimado.SetActive(true);
            fundoMenuEstatico.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PlayerPrefs.HasKey("gameStarted"))
        {
            Destroy(intro);
            Invoke("DesactivateTransition", 1f);
        }
        PlayerPrefs.DeleteAll();
        transicao.GetComponent<Animator>().SetTrigger("Finish");
        Destroy(intro, 4f);
        Invoke("DesactivateTransition", 3f);
    }

    public void BtnJogar()
    {
        transicao.SetActive(true);
        transicao.GetComponent<Animator>().SetTrigger("Start");
        transitionAnimator.SetTrigger("MakeTransition");
        Invoke("StartLoad", 1f);
    }

    private void StartLoad()
    {
        StartCoroutine(LoadAsynchronously(1));
    }

    public void BtnHistoria()
    {
        menu.SetActive(false);
        historia.SetActive(true);
    }

    public void BtnVoltaHistoria()
    {
        historia.SetActive(false);
        menu.SetActive(true);
    }
    
    public void BtnComoJogar()
    {
        menu.SetActive(false);
        comoJogar.SetActive(true);
    }

    public void BtnVoltaComoJogar()
    {
        comoJogar.SetActive(false);
        menu.SetActive(true);
    }

    public void BtnConfig()
    {
        menu.SetActive(false);
        configs.SetActive(true);
    }

    public void BtnVoltaConfig()
    {
        configs.SetActive(false);
        menu.SetActive(true);
    }

    public void BtnCreditos()
    {
        menu.SetActive(false);
        creditos.SetActive(true);
    }

    public void BtnVoltaCreditos()
    {
        menu.SetActive(true);
        creditos.SetActive(false);
    }

    public void BtnSair()
    {
        Application.Quit();
    }

    public void ChangeVolumeMusic(float volume)
    {
        mixer.SetFloat("MusicVolume", volume);
    }

    public void ChangeVolumeEffects(float volume)
    {
        mixer.SetFloat("EffectsVolume", volume);
    }

    public void SensibilityX(float sensibility)
    {
        PlayerPrefs.SetFloat("Sensibility X", sensibility);
    }

    public void SensibilityY(float sensibility)
    {
        PlayerPrefs.SetFloat("Sensibility Y", sensibility);
    }

    private void DesactivateTransition()
    {
        transicao.SetActive(false);
    }

    public IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }

    }
}
