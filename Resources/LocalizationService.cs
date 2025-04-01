using _3.QKA_DACK.Resources;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace _3.QKA_DACK.Resources
{
    public class LocalizationService
    {
        private readonly IStringLocalizer _localizer;
        public LocalizationService(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName!);
            _localizer = factory.Create("SharedResource", assemblyName.Name);
        }
        public string GetLocalizedHtmlString(string key)
        {
            return _localizer[key];
        }
    }
}