using System.Linq;
using Auth.FWT.Core.Extensions;
using TeleSharp.TL;

namespace Auth.FWT.Core.Services.Telegram
{
    public class PhotoInfo
    {
        public PhotoInfo(TLPhoto photo)
        {
            var sizes = photo.Sizes.GetListOfValuesOf("Size", "Type", "Location");
            var originalSize = sizes.Where(s => ((string)s["Type"]) == "x").FirstOrDefault() ?? sizes[0];
            var location = originalSize["Location"] as TLFileLocation;

            Size = (int)originalSize["Size"];
            LocalId = location.LocalId;
            Secret = location.Secret;
            VolumeId = location.VolumeId;
        }

        public int Size { get; set; }

        public int LocalId { get; set; }

        public long Secret { get; set; }

        public long VolumeId { get; set; }
    }
}