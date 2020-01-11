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

    //private static double _baseSize = DefaultBaseSize;

    //[System.ComponentModel.TypeConverter(typeof(System.Windows.FontSizeConverter))]
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
      //=> _baseSize = s2d(value); 
      => element.SetValue(BaseSizeProperty, value);
    public static string GetBaseSize(FrameworkElement element)
    {
      //if (element == null)
      //{
      //}
      //else if (element.GetType() == typeof(Run))
      //{
      //}

      return (element != null) ? (string)element.GetValue(BaseSizeProperty)
                                : DefaultBaseSize.ToString();
    }

    private static void BaseSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //_baseSize = s2d(e.NewValue as string);
      //c.FontSize = _baseSize * GetRelativeSize(c);

      //if (d is Control c)
      //{
      //  double relativeSize = GetRelativeSize(c);
      //  if (double.IsNaN(relativeSize))
      //    relativeSize = 1.0;

      //  c.FontSize = GetBaseSizeByDouble(d) * relativeSize;
      //}
      if (d is FrameworkElement element)
      {
        if (d.GetType().GetProperty("FontSize") == null)
          return;

        double relativeSize = GetRelativeSize(element);
        if (double.IsNaN(relativeSize))
          return;

        string baseSizeString = e.NewValue as string;
        double baseSize = s2d(baseSizeString);

        //SetFontSizeWhenAble(d, GetBaseSizeByDouble(d) * relativeSize);
        //SetFontSizeWhenAble(d, baseSize * relativeSize);
        ((dynamic)d).FontSize = baseSize * relativeSize;
      }

      //PropertyChanged?.Invoke(d, new PropertyChangedEventArgs(""));
    }

    //private static double GetBaseSizeByDouble(DependencyObject element)
    //{
    //  //return s2d(GetBaseSize(element as FrameworkElement));

    //  string value = GetBaseSize(element as FrameworkElement);
    //  return (double)(new FontSizeConverter()).ConvertFromString(value);
    //}

    //private static double s2d(string value)
    //  => (double)(new FontSizeConverter()).ConvertFromString(value);

    #endregion





    #region RelativeSize
    //[System.ComponentModel.TypeConverter(typeof(System.Windows.FontSizeConverter))]
    public static readonly DependencyProperty RelativeSizeProperty = DependencyProperty.RegisterAttached(
"RelativeSize",
typeof(double),
typeof(FontManager),
  new FrameworkPropertyMetadata(
    double.NaN,
    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
    RelativeSizePropertyChanged)
);


    public static void SetRelativeSize(FrameworkElement element, double value)
      => element.SetValue(RelativeSizeProperty, value);
    public static double GetRelativeSize(FrameworkElement element)
      => (element != null) ? (double)element.GetValue(RelativeSizeProperty)
                           : double.NaN;

    private static void RelativeSizePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      if (o.GetType().GetProperty("FontSize") == null)
        return;

      double relativeSize = (double)e.NewValue;
      if (double.IsNaN(relativeSize) || relativeSize <= 0.0)
        return;

      //if (o.GetType().GetProperty("FontSize") != null)
      //{
      //  //((dynamic)o).FontSize = _baseSize * (double)e.NewValue;
      //  ((dynamic)o).FontSize = GetBaseSizeByDouble(o) * (double)e.NewValue;
      //}
      //SetFontSizeWhenAble(o, GetBaseSizeByDouble(o) * (double)e.NewValue);
      ((dynamic)o).FontSize = GetBaseSizeByDouble(o) * relativeSize;

    }
    #endregion



    private static double GetBaseSizeByDouble(DependencyObject element)
    {
      //return s2d(GetBaseSize(element as FrameworkElement));

      string value = GetBaseSize(element as FrameworkElement);
      //return (double)(new FontSizeConverter()).ConvertFromString(value);
      return s2d(value);
    }


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
