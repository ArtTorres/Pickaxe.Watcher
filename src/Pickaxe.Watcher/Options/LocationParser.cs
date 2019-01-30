using MagnetArgs;
using Pickaxe.Watcher.IO.Twitter.Model;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Pickaxe.Watcher.Options
{
    class LocationParser : IParser
    {
        public object Parse(string value)
        {
            return this.ParseLocations(value);
        }

        private Location[] ParseLocations(string json)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(Location[]));

                return (Location[])serializer.ReadObject(stream);
            }
        }
    }
}
