using Avalonia;
using Avalonia.Controls;
using DutyIsland.Models.Notification;

namespace DutyIsland.Controls;

public partial class NotificationSettingsControl : UserControl
{
    public static readonly DirectProperty<NotificationSettingsControl, NotificationSettings> SettingsProperty =
        AvaloniaProperty.RegisterDirect<NotificationSettingsControl, NotificationSettings>(
            nameof(Settings), o => o.Settings, (o, v) => o.Settings = v);

    public NotificationSettings Settings
    {
        get => GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }
    
    public NotificationSettingsControl()
    {
        DataContext = this;
        InitializeComponent();
    }
}