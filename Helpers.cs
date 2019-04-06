// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace NodaTime.Intervals.Internal
{
    /// <summary>
    /// Helper static methods for argument/state validation.
    /// </summary>
    internal static class Helpers
    {
        public static int Hash<T1, T2>(T1 t1, T2 t2)
        {
            return (17 * 37 + ((object) t1 != null ? t1.GetHashCode() : 0)) * 37 + ((object) t2 != null ? t2.GetHashCode() : 0);
        }
        public static void CheckState(bool expression, string message)
        {
            if (!expression)
                throw new InvalidOperationException(message);
        }
    }
}
