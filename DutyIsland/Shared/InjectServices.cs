using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ClassIsland.Core.Abstractions.Services.Logging;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DutyIsland.Shared;

public static class InjectServices
{
    // 获取 AppLoggerProvider 逻辑，暂时不用
    public static ILoggerProvider GetAppLoggerProvider(IServiceCollection services)
    {
        var serviceDescriptorType = typeof(ServiceDescriptor);
        var implementationInstanceField = 
            serviceDescriptorType
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .First(field => field.Name == "_implementationInstance");
        
        IAppLogService? appLogService = null;
        ILoggerProvider? appLoggerProvider = null;
        
        foreach (var service in services)
        {
            if (service.ServiceType.ToString().Contains("AppLogService"))
            {
                if (service.ImplementationInstance != null)
                {
                    appLogService = (IAppLogService)service.ImplementationInstance;
                    continue;
                }
                
                appLogService = (IAppLogService)Activator.CreateInstance(service.ServiceType)!;
                implementationInstanceField.SetValue(service, appLogService);
            }
            else if (service.ServiceType == typeof(ILoggerProvider) && 
                     (service.ImplementationType?.ToString().Contains("AppLoggerProvider") ?? false))
            {
                if (service.ImplementationInstance != null)
                {
                    appLoggerProvider = (ILoggerProvider)service.ImplementationInstance;
                    continue;
                }
                
                appLoggerProvider = (ILoggerProvider)Activator.CreateInstance(service.ImplementationType, appLogService!)!;
                implementationInstanceField.SetValue(service, appLoggerProvider);
            }
        }
    
        return appLoggerProvider!;
    }

    public static bool TryGetAddSettingsPageGroupMethod([MaybeNullWhen(false)] out MethodInfo method)
    {
        var settingsWindowRegistryExtensionsType = typeof(SettingsWindowRegistryExtensions);
        method = settingsWindowRegistryExtensionsType
            .GetMethods()
            .FirstOrDefault(method => (method.ToString()?.Contains("AddSettingsPageGroup") ?? false) && method.GetParameters().Length == 4);
        return method != null;
    }

    public static FieldInfo GetSettingsPageInfoNameField()
    {
        var settingsPageInfoType = typeof(SettingsPageInfo);
        var field = settingsPageInfoType
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(method => method.ToString()?.Contains("Name") ?? false);
        return field!;
    }

    public static PropertyInfo GetSettingsPageInfoGroupIdProperty()
    {
        var settingsPageInfoType = typeof(SettingsPageInfo);
        var property = settingsPageInfoType
            .GetProperties()
            .FirstOrDefault(method => method.ToString()?.Contains("GroupId") ?? false);
        return property!;
    }
}