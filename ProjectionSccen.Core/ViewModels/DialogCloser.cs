using System.Windows;

namespace ProjectionSccen.Core.ViewModels;
/// <summary>
/// 
/// </summary>
public static class DialogCloser
{
    /// <summary>
    ///     
    /// </summary>
    public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(DialogCloser), new PropertyMetadata(DialogResultChanged));
    /// <summary>
    /// 
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = d as Window;
        if (window != null)
        {
            window.Close();
             
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public static void SetDialogResult(Window target, bool? value)
    {
        target.SetValue(DialogResultProperty, value);
    }
}