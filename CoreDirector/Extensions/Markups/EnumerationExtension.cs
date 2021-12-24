using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;
using CoreDirector.Models.Commons;

namespace CoreDirector.Extensions.Markups
{
    public class EnumerationExtension : MarkupExtension
    {
        #region Fields
        private readonly Type _enumType = null!;
        #endregion

        #region Properties
        public Type EnumType
        {
            get => _enumType;
            private init
            {
                if (_enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                _enumType = value;
            }
        }
        #endregion

        #region Constructor
        public EnumerationExtension(Type enumType)
        {
            EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
        }
        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);

            return enumValues.Cast<object>().Select(enumValue => new EnumerationMember
            {
                Value = enumValue,
                Description = GetDescription(enumValue)
            }).ToArray();
        }

        private string GetDescription(object enumValue)
        {
            var attribute = EnumType.GetField(enumValue.ToString() ?? string.Empty)?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();

            return attribute is DescriptionAttribute descriptionAttribute
                ? descriptionAttribute.Description
                : enumValue.ToString()
                  ?? string.Empty;
        }
    }
}
