using QApp.Options;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using  Pickaxe.Watcher.IO.Twitter.Model;

namespace Pickaxe.Watcher.Options
{
    class LocationParser : IOptionParser
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
