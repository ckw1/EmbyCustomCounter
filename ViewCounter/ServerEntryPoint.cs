using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Entities;
using System;
using System.Linq;
using System.Threading;

namespace ViewCounter
{
    public class ServerEntryPoint : IServerEntryPoint
    {
        private readonly ILibraryManager _libraryManager;
        private const string ViewCountTagPrefix = "view-count:";

        public ServerEntryPoint(ILibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }

        public void Run()
        {
            // Subscribe to the playback stop event
            MediaBrowser.Controller.Session.PlaybackStopEventArgs.PlaybackStopped += Player_PlaybackStopped;
        }

        private void Player_PlaybackStopped(object sender, MediaBrowser.Controller.Session.PlaybackStopEventArgs e)
        {
            // Ensure the item played is a base item and played to completion
            if (e.Item is BaseItem item && e.PlayedToCompletion)
            {
                // We only care about Movies for this example
                if (item.GetClientTypeName() == "Movie")
                {
                    UpdateViewCount(item);
                }
            }
        }

        private void UpdateViewCount(BaseItem item)
        {
            int currentCount = 0;
            string viewTag = item.Tags.FirstOrDefault(t => t.StartsWith(ViewCountTagPrefix, StringComparison.OrdinalIgnoreCase));

            // If the tag exists, parse the current count
            if (!string.IsNullOrEmpty(viewTag))
            {
                int.TryParse(viewTag.Substring(ViewCountTagPrefix.Length), out currentCount);
                // Remove the old tag
                item.Tags.Remove(viewTag);
            }

            // Increment the count and add the new tag
            currentCount++;
            item.Tags.Add($"{ViewCountTagPrefix}{currentCount}");

            // Update the item in the library
            // Using CancellationToken.None is acceptable for this background task
            _libraryManager.UpdateItem(item, item.Parent, ItemUpdateType.MetadataEdit, CancellationToken.None);
        }

        public void Dispose()
        {
            // Unsubscribe from the event to prevent memory leaks
            MediaBrowser.Controller.Session.PlaybackStopEventArgs.PlaybackStopped -= Player_PlaybackStopped;
        }
    }
}
