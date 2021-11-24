using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Prompter
{

    public class IndexWrapper : DependencyObject
    {
        public static readonly DependencyProperty MaxIndexProperty = DependencyProperty.Register("MaxIndex", typeof(int), typeof(IndexWrapper), new PropertyMetadata(int.MaxValue));

        public int MaxIndex
        {
            get { return (int)GetValue(MaxIndexProperty); }
            set { SetValue(MaxIndexProperty, value);}
        }
    }

    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }

    public class IndexValidator : ValidationRule
    {

        public IndexWrapper Wrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var success = int.TryParse(value as string, out var index);
            if (!success)
            {
                return new ValidationResult(false, "Invalid index: must be number");
            }

            if (index <= 0)
            {
                return new ValidationResult(false, "Invalid index: must be 1 or greater");
            }

            if (index > Wrapper.MaxIndex)
            {
                return new ValidationResult(false, $"Invalid index: must be {Wrapper.MaxIndex} or less");
            }

            return ValidationResult.ValidResult;
        }
    }
}
