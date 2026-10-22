using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list) //in place shuffle
    {
        int count = list.Count;
        for (int i = count - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i
            int randomIndex = Random.Range(0, i + 1);
            
            // Swap
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    public static void Shuffle<T>(this List<T> list, int  seed)
    {
        var randState = Random.state;
        Random.InitState(seed);
        int count = list.Count;
        for (int i = count - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i
            int randomIndex = Random.Range(0, i + 1);
            
            // Swap
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        Random.state = randState; //restore state
    }
}