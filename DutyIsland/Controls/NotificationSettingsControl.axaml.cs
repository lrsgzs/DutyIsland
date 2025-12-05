using Avalonia;
using Avalonia.Controls;
using DutyIsland.Models.Notification;

namespace DutyIsland.Controls;

public partial class NotificationSettingsControl : UserControl
{
    public static readonly DirectProperty<NotificationSettingsControl, NotificationSettings> SettingsProperty =
        AvaloniaProperty.RegisterDirect<NotificationSettingsControl, NotificationSettings>(
            nameof(Settings), o => o.Settings, (o, v) => o.Settings = v);

    private NotificationSettings _settings = new();
    
    public NotificationSettings Settings
    {
        get => _settings;
        set => SetAndRaise(SettingsProperty, ref _settings, value);
    }
    
    public NotificationSettingsControl()
    {
        DataContext = this;
        InitializeComponent();
    }
}