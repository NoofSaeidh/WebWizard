
using System;

namespace PX.WebWizard.Acumatica.Wizard
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ArgumentAttribute : Attribute
    {
        public ArgumentAttribute(string name)
        {
            Name = name;
        }

        public ArgumentAttribute(string name, string alias)
        {
            Name = name;
            Alias = alias;
        }

        public string Name { get; }
        public string Alias { get; }
        public ArgumentType ArgumentType { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class NotArgumentAttribute : Attribute
    {
    }
}
