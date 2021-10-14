using System.Collections.Generic;

namespace AzureCalcApp.Data
{
    public static class Extensions
    {
        public static void ValidateQueue(this Queue<string> queue ) 
        {
            while(queue.Count > 10) 
                queue.Dequeue();
        }
    }
}