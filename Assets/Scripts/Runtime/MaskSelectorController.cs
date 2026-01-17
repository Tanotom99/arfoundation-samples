using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI; // Necesario para interactuar con el Slider
using System.Collections.Generic;

// Este script se adjunta al objeto ARFaceManager.
public class MaskSelectorController : MonoBehaviour
{
    [Tooltip("Todos los materiales (máscaras) disponibles.")]
    public Material[] faceMaskMaterials;
    
    [Tooltip("Referencia al Slider UI en tu Canvas.")]
    public Slider maskSlider;

    // El material activo actualmente seleccionado por el Slider.
    private Material currentSelectedMaterial;

    private ARFaceManager arFaceManager;

    void Start()
    {
        arFaceManager = GetComponent<ARFaceManager>();
        
        if (arFaceManager == null || faceMaskMaterials == null || faceMaskMaterials.Length == 0)
        {
            Debug.LogError("Configuración inicial incompleta. Revisa ARFaceManager y el array de Materiales.");
            enabled = false;
            return;
        }

        // 1. Configurar el Slider: Mínimo es 0, Máximo es el número de materiales - 1
        maskSlider.minValue = 0;
        maskSlider.maxValue = faceMaskMaterials.Length - 1;
        maskSlider.wholeNumbers = true; // Asegura que solo se elijan índices enteros
        
        // 2. Asignar el primer material por defecto
        currentSelectedMaterial = faceMaskMaterials[0];
        
        // 3. Suscribirse a los eventos: cuando una nueva cara aparece, aplicamos el material actual.
        arFaceManager.facesChanged += OnFacesChanged;
    }

    // Se llama automáticamente cada vez que el valor del Slider cambia.
    public void SelectMask(float sliderValue)
    {
        // 1. Convertir el valor flotante del slider a un índice entero
        int newIndex = Mathf.RoundToInt(sliderValue);

        // 2. Asignar el material seleccionado
        currentSelectedMaterial = faceMaskMaterials[newIndex];
        
        Debug.Log("Material seleccionado: " + newIndex + " (" + currentSelectedMaterial.name + ")");

        // 3. Aplicar el nuevo material a todas las caras ya detectadas
        foreach (var face in arFaceManager.trackables)
        {
            ApplyMaskToFace(face, currentSelectedMaterial);
        }
    }

    // Método que se activa cuando Unity detecta o actualiza una cara
    void OnFacesChanged(ARFacesChangedEventArgs eventArgs)
    {
        // Aplicar la máscara actual a todas las caras nuevas
        foreach (var face in eventArgs.added)
        {
            ApplyMaskToFace(face, currentSelectedMaterial);
        }
    }

    // Función auxiliar para aplicar el material al MeshRenderer
    void ApplyMaskToFace(ARFace face, Material materialToApply)
    {
        var meshRenderer = face.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = materialToApply;
        }
    }
}
