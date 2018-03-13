using TeleSharp.TL;

namespace Auth.FWT.Core.Services.Telegram
{
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

        public long Id { get; set; }

        public string Message { get; set; }

        public string MimeType { get; set; }

        public int Size { get; set; }

        public int Version { get; set; }
    }
}
