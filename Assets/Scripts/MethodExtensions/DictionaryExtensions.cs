using System.Collections.Generic;

namespace WyneAnimator
{
    public static class DictionaryExtensions
    {
        public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> target, TKey key, TValue value)
        {
            if (target.ContainsKey(key))
            {
                target.Remove(key);
                target.Add(key, value);
            }
            else
            {
                target.Add(key, value);
            }
        }
    }
}
