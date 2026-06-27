using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Описание")]
    [TextArea]
    [SerializeField] private string description = "Это описание объекта"; // Измените в Inspector для каждого объекта

    [Header("Пазлы")]
    [SerializeField] private int puzzleId = 0; // 1,2,3 для пазл-объектов; 0 для остальных

    [Header("Параметры осмотра")]
    [SerializeField, Tooltip("Стартовая дистанция камеры от центра объекта")]
    private float orbitDistance = 1f;   // приватное поле, видно в инспекторе

    [SerializeField, Tooltip("Минимально допустимая дистанция (чтобы камера не влетела в объект)")]
    private float minZoom = 0.8f;       // приватное поле, видно в инспекторе

    [SerializeField, Tooltip("Максимальная дистанция")]
    private float maxZoom = 3f;         // приватное поле, видно в инспекторе
    public AudioSource AmbientSource;
    public AudioClip AudioClip;

    [HideInInspector]
    public AudioSource AudioSource;

    private void Awake()
    {
        if (AudioSource == null)
        AudioSource = GetComponent<AudioSource>();
    }

    

    // Публичные только для чтения (без возможности прямого установки извне)
    public string Description => description;
    public int PuzzleId => puzzleId;

    public float OrbitDistance => orbitDistance;
    public float MinZoom => minZoom;
    public float MaxZoom => maxZoom;
}
