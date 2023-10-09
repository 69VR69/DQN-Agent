using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Color _color;

    private void Start()
    {
        _renderer.material.color = _color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Level Completed!");
            //GameManager.Instance.LevelCompleted();
        }
    }
}