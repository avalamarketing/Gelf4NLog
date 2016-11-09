using System.IO;
using System.Reflection;
using Xunit;

namespace Gelf4NLog.UnitTest.Resources
{
    internal class ResourceHelper
    {
        internal static TextReader GetResource(string filename)
        {
            Assert.NotNull(filename);
            var thisAssembly = Assembly.GetExecutingAssembly();
            var resourceFullName = typeof (ResourceHelper).Namespace + "." + filename;
            var manifestResourceStream = thisAssembly.GetManifestResourceStream(resourceFullName);
            Assert.NotNull(manifestResourceStream);

            return new StreamReader(manifestResourceStream);
        }
    }
}
