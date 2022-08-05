using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

enum TaskID
{
    Task1,
    Task2,
    TaskCanceled
}
public class Unit : MonoBehaviour
{
    private int _health = 80;

    private void Start()
    {
        //ReceiveHealing();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancelToken = cancellationTokenSource.Token;
        //cancellationTokenSource.Cancel();
        Task task1 = TestAsync(5000, cancelToken);
        Task task2 = TestAsync2(60, cancelToken);
        WhatTaskFasterAsync(cancellationTokenSource, task1, task2);
    }
    
    public void ReceiveHealing()
    {
        if (HealingCoroutine() != null)
        {
            StopCoroutine(HealingCoroutine());
        }
        StartCoroutine(HealingCoroutine());
    }

   IEnumerator HealingCoroutine()
   {
       int i = 0;
       while (i < 6)
       {
           if (_health < 100)
           {
               _health += 5;
               Debug.Log(_health);
           }
           else
               yield break;
           i++;
           yield return new WaitForSeconds(0.5f);
       }
   }

   async Task<TaskID> TestAsync(int time, CancellationToken cancelToken)
   {
       if (cancelToken.IsCancellationRequested)
       {
           Debug.Log($"Cancel token!{TaskID.Task1}");
           return TaskID.TaskCanceled;
       }
       await Task.Delay(time, cancelToken);
       Debug.Log($"End work!{TaskID.Task1}");
       return TaskID.Task1;
   }
   async Task<TaskID> TestAsync2(int frame, CancellationToken cancelToken)
   {
       for (int i = 0; i <= frame; i++)
       {
           if (cancelToken.IsCancellationRequested)
           {
               Debug.Log($"Cancel token!{TaskID.Task2}");
               return TaskID.TaskCanceled;
           }
           //Debug.Log(i);
           await Task.Yield();
       }
       Debug.Log($"End work {TaskID.Task2}");
       return TaskID.Task2;
   }

   private async Task WhatTaskFasterAsync(CancellationTokenSource ct, Task task1, Task task2)
   {
       Task<TaskID> test = (Task<TaskID>) await Task.WhenAny(task1, task2);
       var taskID = await test;
       ct.Cancel();
       switch (taskID)
       {
           case TaskID.Task1:
               Debug.Log("true");
               break;
           case TaskID.Task2:
               Debug.Log("false");
               break;
           case TaskID.TaskCanceled:
               Debug.Log("false");
               break;
       }
       Debug.Log($"Task1 status{task1.Status}");
       Debug.Log($"Task2 status{task2.Status}"); 
       Debug.Log($"First task end: {taskID}");
   }
}