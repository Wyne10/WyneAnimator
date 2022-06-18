using System.Collections.Generic;

namespace WyneAnimator
{
    public static class ListExtensions
    {
        public static void AddArray<T>(this List<T> target, T[] array)
        {
            foreach(T t in array)
            {
                target.Add(t);
            }
        }
    }
}


