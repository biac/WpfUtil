using System;
using System.Windows;
using System.Windows.Controls;

namespace BluewaterSoft.WpfUtil
{
  /// <summary>
  /// <para xml:lang="ja">XAML において、 相対値でフォントサイズを記述できるようにする。</para>
  /// <para xml:lang="en">Allows you to describe font size by relative value when writing XAML code.</para>
  /// </summary>
  public class FontSize : DependencyObject
  {
    #region Base attached property
    private static readonly double DefaultBaseSize = SystemFonts.MessageFontSize;
    // It defaults to SystemFonts.MessageFontSize which has a default value of 12.
    // https://stackoverflow.com/a/48624542

    /// <summary>
    /// <para xml:lang="ja">Relative サイズの基準となる Font サイズです。 単位記号も付記できます (px, pt, in, cm)。</para>
    /// <para xml:lang="en">Font size on which Relative size is based. Unit symbol (px, pt, in, cm) may be added.</para>
    /// </summary>
    public static readonly DependencyProperty BaseProperty
      = DependencyProperty.RegisterAttached(
          "Base",
          typeof(string),
          typeof(FontSize),
          new FrameworkPropertyMetadata(
            defaultValue: DefaultBaseSize.ToString(),
            FrameworkPropertyMetadataOptions.Inherits
            | FrameworkPropertyMetadataOptions.AffectsMeasure
            | FrameworkPropertyMetadataOptions.AffectsRender,
            propertyChangedCallback: BaseSizePropertyChanged
          ),
          validateValueCallback: o => (o is string s && FontSizeStringToDouble(s) > 0.0)
        );

    /// <summary>Setter for BaseProperty</summary>
    public static void SetBase(FrameworkElement element, string value)
      => element.SetValue(BaseProperty, value);
    /// <summary>Getter for BaseProperty</summary>
    public static string GetBase(FrameworkElement element)
      => element?.GetValue(BaseProperty) as string ?? DefaultBaseSize.ToString();

    private static void BaseSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = GetFrameworkElementHavingFontSizeProperty(d);
      if (element != null)
        SetFontSize(element);
    }
    #endregion


    #region Relative attached property
    /// <summary>
    /// <para xml:lang="ja">Base サイズの倍率で表されるフォントサイズ。</para>
    /// <para xml:lang="en">Font size expressed as a magnification of the Base size.</para>
    /// </summary>
    public static readonly DependencyProperty RelativeProperty
      = DependencyProperty.RegisterAttached(
          "Relative",
          typeof(double),
          typeof(FontSize),
          new FrameworkPropertyMetadata(
            defaultValue: double.NaN,
            FrameworkPropertyMetadataOptions.Inherits
            | FrameworkPropertyMetadataOptions.AffectsMeasure
            | FrameworkPropertyMetadataOptions.AffectsRender,
            propertyChangedCallback: RelativeSizePropertyChanged
          ),
          validateValueCallback: o => (o is double r && (double.IsNaN(r) || r > 0.0))
        );

    /// <summary>Setter for RelativeProperty</summary>
    public static void SetRelative(FrameworkElement element, double value)
      => element.SetValue(RelativeProperty, value);
    /// <summary>Getter for RelativeProperty</summary>
    public static double GetRelative(FrameworkElement element)
      => (double)(element?.GetValue(RelativeProperty) ?? double.NaN);

    private static void RelativeSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = GetFrameworkElementHavingFontSizeProperty(d);
      if (element != null)
        SetFontSize(element);
    }
    #endregion

    #region Absolute attached property
    /// <summary>
    /// <para xml:lang="ja">実際の値で表されるフォントサイズ。本来の FontSize プロパティと同じ意味です。</para>
    /// <para xml:lang="en">Font size expressed as a real value. Same meaning as the traditional FontSize property.</para>
    /// </summary>
    public static readonly DependencyProperty AbsoluteProperty
      = DependencyProperty.RegisterAttached(
          "Absolute",
          typeof(string),
          typeof(FontSize),
          new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsMeasure
            | FrameworkPropertyMetadataOptions.AffectsRender,
            propertyChangedCallback: AbsoluteSizePropertyChanged
          ),
          validateValueCallback: IsValidAbsoluteFontSizeString
        );

    private static bool IsValidAbsoluteFontSizeString(object value)
    {
      if (value == null)
        return true;

      return (value is string s && FontSizeStringToDouble(s) > 0.0);
    }

    /// <summary>Setter for AbsoluteProperty</summary>
    public static void SetAbsolute(FrameworkElement element, string value)
      => element.SetValue(AbsoluteProperty, value);
    /// <summary>Getter for AbsoluteProperty</summary>
    public static string GetAbsolute(FrameworkElement element)
      => element?.GetValue(AbsoluteProperty) as string;

    private static void AbsoluteSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = GetFrameworkElementHavingFontSizeProperty(d);
      if (element == null)
        throw new InvalidOperationException($"{d.DependencyObjectType.Name} has no FontSize property.");

      SetFontSize(element);
    }
    #endregion



    private static FrameworkElement GetFrameworkElementHavingFontSizeProperty(DependencyObject d)
    {
      if (d is FrameworkElement element
          && element.GetType().GetProperty("FontSize") != null)
        return element;

      return null;
    }

    private static void SetFontSize(FrameworkElement element)
    {
      if (GetAbsolute(element) is string absString)
      {
        ((dynamic)element).FontSize = FontSizeStringToDouble(absString);
        return;
      }

      if (GetBase(element) is string baseString)
      {
        double baseSize = FontSizeStringToDouble(baseString);
        double relative = GetRelative(element);
        if (!double.IsNaN(relative))
        {
          ((dynamic)element).FontSize = baseSize * relative;
          return;
        }
      }

      ResetFontSizeProperty(element);
      return;

      static void ResetFontSizeProperty(FrameworkElement element)
      {
        // There are other controls having FontSizeProperty, but they are not FrameworkElements.
        switch (element)
        {
          case TextBlock e:
            e.ClearValue(TextBlock.FontSizeProperty);
            break;
          case Control e:
            e.ClearValue(Control.FontSizeProperty);
            break;
          case AccessText e:
            e.ClearValue(AccessText.FontSizeProperty);
            break;
          case Page e:
            e.ClearValue(Page.FontSizeProperty);
            break;
        }
      }
    }

    private static FontSizeConverter _fontSizeConverter;
    private static double FontSizeStringToDouble(string fontSize)
    {
      if (string.IsNullOrWhiteSpace(fontSize))
        return double.NaN;

      try {
        return (double)
                (_fontSizeConverter ??= new FontSizeConverter())
                .ConvertFromString(fontSize);
      }
      catch {
        return double.NaN;
      }
    }
  }
}
