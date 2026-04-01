using System;

// TODO: BioBrain.Annotations was likely from JetBrains.Annotations (ReSharper).
// This stub provides the NotifyPropertyChangedInvocator attribute used in BaseWebViewModel.
// Can be replaced with actual JetBrains.Annotations NuGet package if needed.

namespace BioBrain.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
    {
        public string ParameterName { get; }

        public NotifyPropertyChangedInvocatorAttribute() { }
        public NotifyPropertyChangedInvocatorAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }
    }
}
