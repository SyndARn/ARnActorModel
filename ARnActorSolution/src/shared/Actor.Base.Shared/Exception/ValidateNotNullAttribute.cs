using System;

namespace Actor.Base
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}
