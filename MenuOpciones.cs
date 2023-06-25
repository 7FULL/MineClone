using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuOpciones : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;
    
    float sliderValuev;
    
    private string[] qualitys;
    
    int quality;

    void Start()
    {
        quality = PlayerPrefs.GetInt("numeroDeCalidad", 2);
            
        qualityDropdown.value = quality;
        Debug.Log(quality);
        if (Screen.fullScreen)
        {
            fullscreenToggle.isOn = true;
        }
        else
        {
            fullscreenToggle.isOn = false;
        }
        
        RevisarCalidades();
        volumeSlider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        AudioListener.volume = volumeSlider.value;
        setQuality();
    }

    public void ChangeSlider(float valor)
    {
        sliderValuev = valor;
        PlayerPrefs.SetFloat("volumenAudio", sliderValuev);
        AudioListener.volume = volumeSlider.value;
    }
    
    public void setQuality()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        PlayerPrefs.SetInt("numeroDeCalidad", qualityDropdown.value);
        quality = qualityDropdown.value;
        Debug.Log(quality);
    }
    
    public void toggleFullScreen(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }
    
    public void RevisarCalidades()
    {
        qualitys = QualitySettings.names;

        qualityDropdown.ClearOptions();
        List<string> opciones = new List<string>();

        for (int i = 0; i < qualitys.Length; i++)
        {
            string opcion = qualitys[i];
            opciones.Add(opcion);
        }
        qualityDropdown.AddOptions(opciones);
        qualityDropdown.value = quality;
        qualityDropdown.RefreshShownValue();
        
    }

    public void recargarEscena()
    {
        SceneManager.LoadScene(0);
    }

    public void salir()
    {
        Application.Quit();
    }
}