using System;

namespace ScopedUnitOfWork.Framework
{
    public static class UniqueIdGenerator
    {
        public static string Generate()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}