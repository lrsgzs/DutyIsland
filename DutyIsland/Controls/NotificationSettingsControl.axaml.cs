using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using DutyIsland.Interface.Models.Notification;

namespace DutyIsland.Controls;

public partial class NotificationSettingsControl : UserControl
{
    public static readonly StyledProperty<NotificationSettings> SettingsProperty =
        AvaloniaProperty.Register<NotificationSettingsControl, NotificationSettings>(
            nameof(Settings), defaultBindingMode: BindingMode.TwoWay);
    
    public NotificationSettings Settings
    {
        get => GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }
    
    public NotificationSettingsControl()
    {
        InitializeComponent();
    }
}