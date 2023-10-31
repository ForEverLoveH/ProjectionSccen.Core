using System.ComponentModel;

namespace ProjectionSccen.Core.ViewModels;
/// <summary>
/// 
/// </summary>
public class ViewModelBase : INotifyPropertyChanged
{   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyName"></param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    /// <summary>
    /// 
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
}