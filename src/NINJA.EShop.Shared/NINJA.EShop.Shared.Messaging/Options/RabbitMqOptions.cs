using System.ComponentModel.DataAnnotations;

namespace NINJA.EShop.Shared.Messaging.Options;

public class RabbitMqOptions
{
    public const string SectionName = "MessageBroker";
    [Required(ErrorMessage = "MessageBroker HostName is required")]
    public string Host { get; set; } = string.Empty;
    [Required(ErrorMessage = "MessageBroker UserName is required")]
    public string UserName { get; set; } = string.Empty;
    [Required(ErrorMessage = "MessageBroker Password is required")]
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    [Range(1,65535,ErrorMessage = "Port must be between 1 and 65535")]
    public ushort Port { get; set; } = 5672;
    [Range(1,65535,ErrorMessage = "StreamPort must be between 1 and 65535")]
    public int StreamPort { get; set; } = 5552;
    public ushort PrefetchCount { get; init; } = 16;
}