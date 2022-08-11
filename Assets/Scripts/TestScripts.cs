using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class TestScripts : MonoBehaviour
{
    private NativeArray<int> _nativeArray;
    
    private NativeArray<Vector3> _nativeArrayPositions;
    private NativeArray<Vector3> _nativeArrayVelocities;
    private NativeArray<Vector3> _nativeArrayFinalPosition;
    
    private void Start()
    {
        _nativeArray = new NativeArray<int>(new []{10,5,15,6,25,8,33}, Allocator.Persistent);
        
        _nativeArrayPositions = new NativeArray<Vector3>(new []{Vector3.back, Vector3.down, Vector3.forward}, Allocator.Persistent);
        _nativeArrayVelocities = new NativeArray<Vector3>(new []{Vector3.up, Vector3.one, Vector3.right}, Allocator.Persistent);
        _nativeArrayFinalPosition = new NativeArray<Vector3>(3, Allocator.Persistent);
        
        TestJobs testJobs = new TestJobs
        {
            MyNativeArray = _nativeArray
        };
        var testJobHandle = testJobs.Schedule();
        testJobHandle.Complete();
        foreach (var ell in _nativeArray)
        {
            Debug.Log(ell);
        }

        TestJobsSecond testJobsSecond = new TestJobsSecond
        {
            NativeArrayPos = _nativeArrayPositions,
            NativeArrayVel = _nativeArrayVelocities,
            NativeArrayFin = _nativeArrayFinalPosition
        };
        var testJobSecondHandle = testJobsSecond.Schedule(_nativeArrayPositions.Length, 0);
        testJobSecondHandle.Complete();
         foreach (var ell in _nativeArrayFinalPosition)
         {
             Debug.Log(ell);
         }
    }

    private void OnDestroy()
    {
        _nativeArray.Dispose();
        _nativeArrayPositions.Dispose();
        _nativeArrayVelocities.Dispose();
        _nativeArrayFinalPosition.Dispose();
    }
}

public struct TestJobs : IJob
{
    public NativeArray<int> MyNativeArray;
    public void Execute()
    {
        for (int i = 0; i < MyNativeArray.Length; i++)
        {
            if (MyNativeArray[i] >= 10)
            {
                MyNativeArray[i] = 0;
            }
        }
    }
}

public struct TestJobsSecond : IJobParallelFor
{
    public NativeArray<Vector3> NativeArrayPos;
    public NativeArray<Vector3> NativeArrayVel;
    public NativeArray<Vector3> NativeArrayFin;
    public void Execute(int index)
    {
        NativeArrayFin[index] = NativeArrayPos[index] + NativeArrayVel[index];
    }
}
