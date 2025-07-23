using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using ViewCounter.Configuration;

namespace ViewCounter
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override Guid Id => new Guid("a8b9592c-633b-47e2-88a4-98c5d2c2509c");

        public override string Name => "View Counter Plugin";

        public override string Description => "Adds a view counter to movies and displays it on a custom page.";

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public static Plugin Instance { get; private set; }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "ViewCounterPage",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html",
                }
            };
        }
    }
}
