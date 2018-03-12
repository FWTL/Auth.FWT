using Auth.FWT.Core.Extensions;
using System.Linq;
using TeleSharp.TL;

namespace Auth.FWT.API.Controllers.Job
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

    public class MediaInfo
    {
        public MediaInfo(TLDocument document)
        {
            AccessHash = document.AccessHash;
            Version = document.Version;
            Size = document.Size;
            MimeType = document.MimeType;
            Id = document.Id;
        }

        public long AccessHash { get; set; }

        public int Version { get; set; }

        public int Size { get; set; }

        public string MimeType { get; set; }

        public long Id { get; set; }

        public string Message { get; set; }
    }
}