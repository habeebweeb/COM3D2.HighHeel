using System.Collections.Generic;
using MaidStatus;

namespace COM3D2.HighHeel
{
    public static class Extensions
    {
        public static void Deconstruct(
            this Status status, out string guid, out string firstName, out string lastName
        )
        {
            guid = status.guid;
            firstName = status.firstName;
            lastName = status.lastName;
        }

        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value
        )
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}
