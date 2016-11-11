using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Procedural.Darkness;

public class DarknessMovement : MonoBehaviour
{
    private DarknessData _data;
    private Renderer _renderer;
    private ParticleSystem _mainParticles;
    private ParticleSystem _eyeParticles;
    private Vector3 _movementSpeed;
    private Vector3 _pathToGo;
    private bool _moving;


    public void Create(DarknessData data)
    {
        if (_data != null)
        {
            Debug.Log("Create for " + _data.GetName() + " was called a second time, this is not allowed!");
            return;
        }

        _data = data;
        SetInfo();
        SetStart();
    }

    private void SetInfo()
    {
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        _mainParticles = particleSystems[0];
        _eyeParticles = particleSystems[1];

        _mainParticles.randomSeed = (uint)Utility.Random.Next();
        _eyeParticles.randomSeed = (uint)Utility.Random.Next();
    }

    private void SetStart()
    {
        transform.position = _data.Start();
        _moving = false;
    }

    void FixedUpdate()
    {
        if (_moving)
        {
            if (Vector3.Distance(transform.position, _data.End()) >= _data.GetStopDistance())
            {
                _pathToGo = transform.position - _data.End();
                _pathToGo.Normalize();
                _pathToGo.Scale(_movementSpeed);
                transform.position = transform.position - _pathToGo;
            }
        }
    }

    public void SetTotalTravelTime(float timeInSeconds)
    {
        float distanceToNest = Vector3.Distance(_data.Start(), _data.End());
        float travelPerSecond = distanceToNest/timeInSeconds;
        float travelPerUpdate = travelPerSecond/60;
        _movementSpeed = new Vector3(travelPerUpdate, travelPerUpdate, travelPerUpdate);
    }

    public float GetMovementSpeed()
    {
        return _movementSpeed.x;
    }

    public DarknessData GetData()
    {
        return _data;
    }

    public void StartMoving()
    {
        _moving = true;
    }

    public void StopMoving()
    {
        _moving = false;
    }
}
