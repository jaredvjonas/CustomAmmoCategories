using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Threading
{
    public static class ThreadFlagExtensions
    {
        private static ThreadLocal<HashSet<string>> _flags = new ThreadLocal<HashSet<string>>(() => new HashSet<string>());

        public static void SetFlag(this Thread thread, string flag)
        {
            var set = _flags.Value;
            lock (set)
            {
                set.Add(flag);
            }
        }

        public static void ClearFlag(this Thread thread, string flag)
        {
            var set = _flags.Value;
            lock (set)
            {
                set.Remove(flag);
            }
        }

        public static bool isFlagSet(this Thread thread, string flag)
        {
            var set = _flags.Value;
            lock (set)
            {
                return set.Contains(flag);
            }
        }
    }
}
