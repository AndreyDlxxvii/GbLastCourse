using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public class SpinRightNow : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Vector3 _direction;
    [SerializeField] private int _count;
    [SerializeField] private float _radiusSpawn;
    [SerializeField] private float _speed;

    private TransformAccessArray _array;
    void Start()
    {
        _array = new TransformAccessArray(GetTransforms(_count));
    }

    private Transform[] GetTransforms(int numberOfObjects)
    {
        Transform [] transforms = new Transform[numberOfObjects];
        for (int i = 0; i < numberOfObjects; i++)
        {
            transforms[i] = Instantiate(_prefab).transform;
            transforms[i].position = Random.insideUnitSphere * _radiusSpawn;
        }
        return transforms;
    }
    void Update()
    {
        RotateJob rotateJob = new RotateJob
        {
            direction = _direction,
            speed = _speed
        };
        JobHandle jobHandle = rotateJob.Schedule(_array);
        jobHandle.Complete();
    }
    
    public struct RotateJob : IJobParallelForTransform
    {
        public Vector3 direction;
        public float speed;
        public void Execute(int index, TransformAccess transform)
        {
            var vect = transform.rotation.eulerAngles;
            
            transform.rotation = Quaternion.Euler(vect+direction*speed);
        }
    }

    private void OnDestroy()
    {
        _array.Dispose();
    }
}
