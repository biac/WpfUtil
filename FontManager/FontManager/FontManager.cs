using System;
using System.Windows;
using System.Windows.Documents;

namespace BluewaterSoft.WpfUtil
{
  public class FontManager : DependencyObject
  {
    #region BaseSize
    private static readonly double DefaultBaseSize = SystemFonts.MessageFontSize;
    // It defaults to SystemFonts.MessageFontSize which has a default value of 12.
    // https://stackoverflow.com/a/48624542

    /// <summary>
    /// BaseSize 添付プロパティ:
    /// RelativeSize の基準となる FontSize を設定します。
    /// 単位付記も可能です(px, pt, in, cm)。
    /// </summary>
    public static readonly DependencyProperty BaseSizeProperty
      = DependencyProperty.RegisterAttached(
          "BaseSize",
          typeof(string),
          typeof(FontManager),
          new FrameworkPropertyMetadata(
            defaultValue: DefaultBaseSize.ToString(),
            FrameworkPropertyMetadataOptions.Inherits
            | FrameworkPropertyMetadataOptions.AffectsMeasure
            | FrameworkPropertyMetadataOptions.AffectsArrange,
            propertyChangedCallback: BaseSizePropertyChanged
          )
        );

    public static void SetBaseSize(FrameworkElement element, string value)
      => element.SetValue(BaseSizeProperty, value);
    public static string GetBaseSize(FrameworkElement element)
      => (element != null) ? (string)element.GetValue(BaseSizeProperty)
                           : DefaultBaseSize.ToString();

    private static void BaseSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = GetFrameworkElementHavingFontSizeProperty(d);
      if (element == null)
        return;

      string baseSizeString = e.NewValue as string;
      double relativeSize = GetRelativeSize(element);
      SetFontSize(element, baseSizeString, relativeSize);
    }
    #endregion


    #region RelativeSize
    /// <summary>
    /// RelativeSize 添付プロパティ:
    /// フォントサイズを RelativeSize を基準とした比率で設定します。
    /// </summary>
    public static readonly DependencyProperty RelativeSizeProperty
      = DependencyProperty.RegisterAttached(
          "RelativeSize",
          typeof(double),
          typeof(FontManager),
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

    public static void SetRelativeSize(FrameworkElement element, double value)
      => element.SetValue(RelativeSizeProperty, value);
    public static double GetRelativeSize(FrameworkElement element)
      => (element != null) ? (double)element.GetValue(RelativeSizeProperty)
                           : double.NaN;

    private static void RelativeSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = GetFrameworkElementHavingFontSizeProperty(d);
      if (element == null)
        return;

      double relativeSize = (double)e.NewValue;
      SetFontSize(element, GetBaseSize(element), relativeSize);
    }
    #endregion

    #region AbsoluteSize
    /// <summary>
    /// AbsoluteSize 添付プロパティ:
    /// フォントサイズを絶対値で設定します。
    /// 本来の FontSize プロパティと同じ意味です。
    /// </summary>
    public static readonly DependencyProperty AbsoluteSizeProperty
      = DependencyProperty.RegisterAttached(
          "AbsoluteSize",
          typeof(string),
          typeof(FontManager),
          new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsMeasure
            | FrameworkPropertyMetadataOptions.AffectsArrange
            | FrameworkPropertyMetadataOptions.AffectsParentMeasure
            | FrameworkPropertyMetadataOptions.AffectsParentArrange,
            propertyChangedCallback: AbsoluteSizePropertyChanged
          )
        );

    public static void SetAbsoluteSize(FrameworkElement element, string value)
      => element.SetValue(AbsoluteSizeProperty, value);
    public static string GetAbsoluteSize(FrameworkElement element)
      => element?.GetValue(AbsoluteSizeProperty) as string;

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
