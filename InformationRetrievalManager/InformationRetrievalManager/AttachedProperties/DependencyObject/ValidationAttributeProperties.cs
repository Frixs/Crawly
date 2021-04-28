using InformationRetrievalManager.Core;
using System;
using System.Windows;

namespace InformationRetrievalManager
{
    /// <summary>
    /// <see cref="string"/>: Dependency property to attach validation attribute rules instance into <see cref="ValidationRule"/>
    /// This dependency property works for <see cref="DataStringValidationRule"/>
    /// </summary>
    public class ValidationStringAttributeProperty : DependencyObject
    {
        public IValidableAttribute<string> Value
        {
            get { return (IValidableAttribute<string>)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(IValidableAttribute<string>),
            typeof(ValidationStringAttributeProperty),
            new PropertyMetadata(default(IValidableAttribute<string>))
            );
    }

    /// <summary>
    /// <see cref="int"/>: Dependency property to attach validation attribute rules instance into <see cref="ValidationRule"/>
    /// This dependency property works for <see cref="DataIntegerValidationRule"/>
    /// </summary>
    public class ValidationIntegerAttributeProperty : DependencyObject
    {
        public IValidableAttribute<int> Value
        {
            get { return (IValidableAttribute<int>)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(IValidableAttribute<int>),
            typeof(ValidationIntegerAttributeProperty),
            new PropertyMetadata(default(IValidableAttribute<int>))
            );
    }

    /// <summary>
    /// <see cref="double"/>: Dependency property to attach validation attribute rules instance into <see cref="ValidationRule"/>
    /// This dependency property works for <see cref="DataDoubleValidationRule"/>
    /// </summary>
    public class ValidationDoubleAttributeProperty : DependencyObject
    {
        public IValidableAttribute<double> Value
        {
            get { return (IValidableAttribute<double>)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(IValidableAttribute<double>),
            typeof(ValidationDoubleAttributeProperty),
            new PropertyMetadata(default(IValidableAttribute<double>))
            );
    }

    /// <summary>
    /// <see cref="TimeSpan"/>: Dependency property to attach validation attribute rules instance into <see cref="ValidationRule"/>
    /// This dependency property works for <see cref="DataTimespanValidationRule"/>
    /// </summary>
    public class ValidationTimespanAttributeProperty : DependencyObject
    {
        public IValidableAttribute<TimeSpan> Value
        {
            get { return (IValidableAttribute<TimeSpan>)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(IValidableAttribute<TimeSpan>),
            typeof(ValidationTimespanAttributeProperty),
            new PropertyMetadata(default(IValidableAttribute<TimeSpan>))
            );
    }

    #region Helper Classes

    /// <summary>
    /// Wrapper to evade error: Cannot find governing FrameworkElement for target element
    /// </summary>
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

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }

    #endregion
}
