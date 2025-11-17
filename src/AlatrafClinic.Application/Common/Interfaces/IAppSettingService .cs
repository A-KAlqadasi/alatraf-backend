using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlatrafClinic.Application.Common.Interfaces;

public interface IAppSettingService
{
    Task<string?> GetValueAsync(string key);
    Task<T?> GetValueAsync<T>(string key);
    Task UpdateValueAsync(string key, string value);
}