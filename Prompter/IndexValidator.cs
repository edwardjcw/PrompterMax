using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Prompter
{

    public class IndexWrapper : DependencyObject
    {
        public static readonly DependencyProperty MaxIndexProperty = DependencyProperty.Register("MaxIndex", typeof(string), typeof(IndexWrapper), new PropertyMetadata("0"));

        public string MaxIndex
        {
            get { return (string)GetValue(MaxIndexProperty); }
            set { SetValue(MaxIndexProperty, value);}
        }
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

            if (index > int.Parse(Wrapper.MaxIndex))
            {
                return new ValidationResult(false, $"Invalid index: must be {Wrapper.MaxIndex} or less");
            }

            return ValidationResult.ValidResult;
        }
    }
}
