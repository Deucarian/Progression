using System;

namespace Deucarian.Progression
{
    /// <summary>Marks an authoritative set of named CurrencyKey fields or properties for the Inspector.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class CurrencyKeySetAttribute : Attribute { }
}
