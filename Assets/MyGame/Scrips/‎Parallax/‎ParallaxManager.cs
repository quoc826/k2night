using UnityEngine;
using System;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _speedMotion; // toc do di chuyen cua layer

    private Vector3 _lastPosion; // luu vi tri cuoi cung cua player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // _player = GameObject.FindGameObjectWithTag("Player").transform;
        _lastPosion = _player.position;
    }

    private void LateUpdate()
    {
        // tinh delta positon khi player di chuyen
        Vector3 deltaMove = _player.position - _lastPosion;

        // Di chuyển nền ngược chiều với hướng di chuyển của player (có dấu trừ trước deltaMove.x)
        this.transform.position += new Vector3(-deltaMove.x * _speedMotion, 0f, 0f);

        _lastPosion = _player.position;
    }
}