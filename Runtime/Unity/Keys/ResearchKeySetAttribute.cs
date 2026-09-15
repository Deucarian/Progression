using System;

namespace Deucarian.Progression
{
    /// <summary>Marks an authoritative set of named ResearchKey fields or properties for the Inspector.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ResearchKeySetAttribute : Attribute { }
}
