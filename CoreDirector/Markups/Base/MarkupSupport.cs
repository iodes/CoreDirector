using System;
using System.Windows.Markup;

namespace CoreDirector.Markups
{
    public class MarkupSupport : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
