using System;
using System.Windows;
using System.Windows.Documents;

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
            | FrameworkPropertyMetadataOptions.AffectsArrange,
            propertyChangedCallback: BaseSizePropertyChanged
          )
        );

    /// <summary>Setter for BaseProperty</summary>
    public static void SetBase(FrameworkElement element, string value)
      => element.SetValue(BaseProperty, value);
    /// <summary>Getter for BaseProperty</summary>
    public static string GetBase(FrameworkElement element)
      => (element != null) ? (string)element.GetValue(BaseProperty)
                           : DefaultBaseSize.ToString();

    private static void BaseSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = GetFrameworkElementHavingFontSizeProperty(d);
      if (element == null)
        return;

      string baseSizeString = e.NewValue as string;
      double relativeSize = GetRelative(element);
      SetFontSize(element, baseSizeString, relativeSize);
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
            | FrameworkPropertyMetadataOptions.AffectsArrange
            | FrameworkPropertyMetadataOptions.AffectsParentMeasure
            | FrameworkPropertyMetadataOptions.AffectsParentArrange,
            propertyChangedCallback: RelativeSizePropertyChanged
          )
        );

    /// <summary>Setter for RelativeProperty</summary>
    public static void SetRelative(FrameworkElement element, double value)
      => element.SetValue(RelativeProperty, value);
    /// <summary>Getter for RelativeProperty</summary>
    public static double GetRelative(FrameworkElement element)
      => (element != null) ? (double)element.GetValue(RelativeProperty)
                           : double.NaN;

    private static void RelativeSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = GetFrameworkElementHavingFontSizeProperty(d);
      if (element == null)
        return;

      double relativeSize = (double)e.NewValue;
      SetFontSize(element, GetBase(element), relativeSize);
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
            | FrameworkPropertyMetadataOptions.AffectsArrange
            | FrameworkPropertyMetadataOptions.AffectsParentMeasure
            | FrameworkPropertyMetadataOptions.AffectsParentArrange,
            propertyChangedCallback: AbsoluteSizePropertyChanged
          )
        );

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
        return;

      var absoluteSizeString = e.NewValue as string;
      if (string.IsNullOrWhiteSpace(absoluteSizeString))
      {
        ((dynamic)element).FontSize = DefaultBaseSize;
        return;
      }

      double absoluteSize = s2d(e.NewValue as string);
      ((dynamic)element).FontSize = absoluteSize;
    }
    #endregion



    private static FrameworkElement GetFrameworkElementHavingFontSizeProperty(DependencyObject d)
    {
      if (d is FrameworkElement element
          && element.GetType().GetProperty("FontSize") != null)
        return element;

      return null;
    }

    private static void SetFontSize(FrameworkElement element, string baseSizeString, double relativeSize)
    {
      if (double.IsNaN(relativeSize) || relativeSize <= 0.0)
        return;

      double baseSize = s2d(baseSizeString);
      if (double.IsNaN(baseSize) || baseSize <= 0.0)
        return;

      ((dynamic)element).FontSize = baseSize * relativeSize;
    }

    private static readonly FontSizeConverter _fontSizeConverter
      = new FontSizeConverter();
    private static double s2d(string fontSize)
      => (double)_fontSizeConverter.ConvertFromString(fontSize);
  }
}
