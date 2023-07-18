using System;
using System.Linq;
using Xamarin.Forms;

namespace elogbookmobile.Effects
{
    public class TintEffect : RoutingEffect
    {
        public static readonly BindableProperty TintColorProperty = BindableProperty.CreateAttached("TintColor", typeof(Color), typeof(TintEffect), default(Color), propertyChanged: OnTintColorChanged);

        public static Color GetTintColor(BindableObject bindable)
        {
            return (Color)bindable.GetValue(TintColorProperty);
        }

        public static void SetTintColor(BindableObject bindable, Color value)
        {
            bindable.SetValue(TintColorProperty, value);
        }

        public static void OnTintColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is View view))
            {
                return;
            }

            if (!view.Effects.Any(e => e is TintEffect))
            {
                view.Effects.Add(new TintEffect());
            }
        }

        public TintEffect() : base($"elogbookmobile.{nameof(TintEffect)}") { }
    }
}
