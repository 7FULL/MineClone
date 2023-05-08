using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualTexture : MonoBehaviour
{
    public Texture2D noiseTexture;
    
    public NoiseSettings noiseSettings;
    
    public SpriteRenderer spriteRenderer;

    private int widht;

    private int height;

    // Start is called before the first frame update
    void Start()
    {
        aplicarTextura();
    }

    public void aplicarTextura()
    {
        widht = noiseTexture.width;
        
        height = noiseTexture.height;
        
        Color[] colors = new Color[widht * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < widht; x++)
            {
                float noiseValue = MyNoise.OctavePerlin(x, y, noiseSettings);
                
                float remappedValue = MyNoise.RemapValue01(noiseValue, 0, 1);
                
                colors[y * widht + x] = new Color(remappedValue, remappedValue, remappedValue, 1);
            }
        }
        
        noiseTexture.SetPixels(colors);
        
        noiseTexture.Apply();
        
        // Crea un nuevo sprite utilizando la textura
        Sprite sprite = Sprite.Create(noiseTexture, new Rect(0, 0, widht, height), Vector2.one * 0.5f);

        // Asigna el sprite al componente Sprite Renderer
        spriteRenderer.sprite = sprite;
    }

}
