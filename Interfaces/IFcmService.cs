using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetPN.Interfaces;

public interface IFcmService
{
    Task<bool> SendNotificationAsync(string token, string title, string body, Dictionary<string, string>? data = null);
    Task<int> SendNotificationToAllAsync(IEnumerable<string> tokens, string title, string body, Dictionary<string, string>? data = null);
}
