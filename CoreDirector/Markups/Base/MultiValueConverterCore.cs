using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace CoreDirector.Markups
{
    public abstract class MultiValueConverterCore : MarkupSupport, IMultiValueConverter
    {
        public bool AllowTypeMissmatch { get; set; }

        MethodInfo _convert;
        MethodInfo _convertBack;
        Type[] _argumentTypes;
        Type _returnType;

        object[] _convertParam;
        object[] _convertBackParam;

        bool _isBasic;

        public MultiValueConverterCore()
        {
            Type type = GetType();

            // weak convert, back method
            _convert = GetType().GetMethod("Convert");
            _convertBack = GetType().GetMethod("ConvertBack");

            if (_convert == null || _convertBack == null)
                throw new MissingMethodException();

            // generic type check
            Type[] genericTypes = type.BaseType.GenericTypeArguments;

            if (genericTypes.Length == 0 || genericTypes.Length == 2)
                throw new Exception();

            _isBasic = genericTypes.Length == 1;

            // generic type
            _argumentTypes = genericTypes.Take(genericTypes.Length - 1).ToArray();
            _returnType = genericTypes.Last();

            // param 
            if (!_isBasic)
                _convertParam = new object[_argumentTypes.Length + 2];
            else
                _convertParam = new object[3];

            _convertBackParam = new object[3];
        }

        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!AllowTypeMissmatch)
                ValidateConvert(values, targetType);

            if (_isBasic)
            {
                _convertParam[0] = values;
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                    _convertParam[i] = values[i];
            }

            _convertParam[_convertParam.Length - 2] = parameter;
            _convertParam[_convertParam.Length - 1] = culture;

            object result = _convert.Invoke(this, _convertParam);

            return AllowTypeMissmatch ? result : Convert.ChangeType(result, _returnType);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (!AllowTypeMissmatch)
                ValidateConvertBack(targetTypes);

            _convertBackParam[0] = AllowTypeMissmatch ? value : Convert.ChangeType(value, _returnType);
            _convertBackParam[1] = parameter;
            _convertBackParam[2] = culture;

            return (object[])_convertBack.Invoke(this, _convertBackParam);
        }

        void ValidateConvert(object[] values, Type targetType)
        {
            if (!targetType.IsAssignableFrom(_returnType))
                throw new Exception("Convert type missmatch");

            if (_isBasic)
                return;

            if (values.Length < _argumentTypes.Length)
                throw new Exception();

            for (int i = 0; i < _argumentTypes.Length; i++)
                if (!_argumentTypes[i].IsAssignableFrom(values[i].GetType()))
                    throw new Exception("Convert type missmatch");
        }

        void ValidateConvertBack(Type[] targetTypes)
        {
            if (_argumentTypes.Length < targetTypes.Length)
                throw new Exception();

            for (int i = 0; i < _argumentTypes.Length; i++)
                if (!_argumentTypes[i].IsAssignableFrom(targetTypes[i]))
                    throw new Exception("ConvertBack type missmatch");
        }
    }
}
