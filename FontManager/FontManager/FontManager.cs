using System;
using System.Windows;
using System.Windows.Documents;

namespace BluewaterSoft.WpfUtil
{
  public class FontManager : DependencyObject
  {
    // *********************************
    // あと、AbsFontSize 添付プロパティを付ければ、実用になるかな?
    // ほんとは、既存の Font プロパティの設定を判別できれば、OK なんだけど…
    // *********************************


    //  public static DependencyProperty FontSizeProperty 
    //    =
    //Button.FontSizeProperty.AddOwner(typeof(FontManager),
    //    new FrameworkPropertyMetadata(FontSizePropertyChanged)
    //  );

    //  private static void FontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //  {

    //  }




    #region BaseSize
    private static readonly double DefaultBaseSize = SystemFonts.MessageFontSize;
    // It defaults to SystemFonts.MessageFontSize which has a default value of 12.
    // https://stackoverflow.com/a/48624542

    public static readonly DependencyProperty BaseSizeProperty = DependencyProperty.RegisterAttached(
"BaseSize",
typeof(string),
typeof(FontManager),
  new FrameworkPropertyMetadata(
    DefaultBaseSize.ToString(),
    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure |
     FrameworkPropertyMetadataOptions.AffectsArrange,
    BaseSizePropertyChanged)
);

    public static void SetBaseSize(FrameworkElement element, string value)
      => element.SetValue(BaseSizeProperty, value);
    public static string GetBaseSize(FrameworkElement element)
    {
      return (element != null) ? (string)element.GetValue(BaseSizeProperty)
                                : DefaultBaseSize.ToString();
    }

    private static void BaseSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //if (d is FrameworkElement element)
      //{
      //  if (element.GetType().GetProperty("FontSize") == null)
      //    return;
      var element = GetElementHavingFontSizeProperty(d);
      if (element == null)
        return;

        double relativeSize = GetRelativeSize(element);
      //if (double.IsNaN(relativeSize) || relativeSize <= 0.0)
      //  return;

      string baseSizeString = e.NewValue as string;
      //double baseSize = s2d(e.NewValue as string);
      //((dynamic)element).FontSize = baseSize * relativeSize;
      SetFontSize(element, baseSizeString, relativeSize);
      //}
    }
    #endregion





    #region RelativeSize
    public static readonly DependencyProperty RelativeSizeProperty = DependencyProperty.RegisterAttached(
"RelativeSize",
typeof(double),
typeof(FontManager),
  new FrameworkPropertyMetadata(
    double.NaN,
    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange |
     FrameworkPropertyMetadataOptions.AffectsParentMeasure |
     FrameworkPropertyMetadataOptions.AffectsParentArrange,
    RelativeSizePropertyChanged)
);


    public static void SetRelativeSize(FrameworkElement element, double value)
      => element.SetValue(RelativeSizeProperty, value);
    public static double GetRelativeSize(FrameworkElement element)
      => (element != null) ? (double)element.GetValue(RelativeSizeProperty)
                           : double.NaN;

    private static void RelativeSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //if (d is FrameworkElement element)
      //{
      //  if (element.GetType().GetProperty("FontSize") == null)
      //    return;
      var element = GetElementHavingFontSizeProperty(d);
      if (element == null)
        return;

      double relativeSize = (double)e.NewValue;
      //if (double.IsNaN(relativeSize) || relativeSize <= 0.0)
      //  return;

      //double baseSize = s2d(GetBaseSize(element));
      //((dynamic)element).FontSize = baseSize * relativeSize;
      SetFontSize(element, GetBaseSize(element), relativeSize);

      //}
    }
    #endregion

    private static FrameworkElement GetElementHavingFontSizeProperty(DependencyObject d)
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


    //private static double GetBaseSizeByDouble(DependencyObject element)
    //{
    //  //string baseSize = GetBaseSize(element as FrameworkElement);
    //  return s2d(GetBaseSize(element as FrameworkElement));
    //}

    private static readonly FontSizeConverter _fontSizeConverter
      = new FontSizeConverter();
    private static double s2d(string fontSize)
    {
      return (double)_fontSizeConverter.ConvertFromString(fontSize);
    }

    //private static void SetFontSizeWhenAble(DependencyObject o, double fontSize)
    //{
    //  if (o.GetType().GetProperty("FontSize") != null)
    //    ((dynamic)o).FontSize = fontSize;
    //}
  }
}
