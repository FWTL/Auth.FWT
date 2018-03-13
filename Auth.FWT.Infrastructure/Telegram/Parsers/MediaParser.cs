using System;
using System.Collections.Generic;
using TeleSharp.TL;
using TeleSharp.TL.Photos;

namespace Auth.FWT.Infrastructure.Telegram.Parsers
{
    public static class MediaParser
    {
        private static readonly Dictionary<string, Func<TLAbsMessageMedia, MediaInfo>> Switch = new Dictionary<string, Func<TLAbsMessageMedia, MediaInfo>>()
        {
            { typeof(TLMessageMediaEmpty).FullName, x => { return null; } },
            { typeof(TLMessageMediaPhoto).FullName, x => { return Parse(x as TLMessageMediaPhoto); } },
            { typeof(TLMessageMediaGeo).FullName, x => { return Parse(x as TLMessageMediaGeo); } },
            { typeof(TLMessageMediaContact).FullName, x => { return Parse(x as TLMessageMediaContact); } },
            { typeof(TLMessageMediaUnsupported).FullName, x => { return Parse(x as TLMessageMediaUnsupported); } },
            { typeof(TLMessageMediaDocument).FullName, x => { return Parse(x as TLMessageMediaDocument); } },
            { typeof(TLMessageMediaWebPage).FullName, x => { return Parse(x as TLMessageMediaWebPage); } },
            { typeof(TLMessageMediaVenue).FullName, x => { return Parse(x as TLMessageMediaVenue); } },
            { typeof(TLMessageMediaGame).FullName, x => { return Parse(x as TLMessageMediaGame); } },
            { typeof(TLMessageMediaInvoice).FullName, x => { return Parse(x as TLMessageMediaInvoice); } },
        };

        public static MediaInfo Parse(TLAbsMessageMedia media)
        {
            return Switch[media.GetType().FullName](media);
        }

        private static MediaInfo Parse(TLMessageMediaContact media)
        {
            return null;
        }

        private static MediaInfo Parse(TLMessageMediaDocument media)
        {
            var document = media.Document as TLDocument;
            return new MediaInfo(document);
        }

        private static MediaInfo Parse(TLMessageMediaGame media)
        {
            return null;
        }

        private static MediaInfo Parse(TLMessageMediaGeo media)
        {
            return null;
        }

        private static MediaInfo Parse(TLMessageMediaInvoice media)
        {
            return null;
        }

        private static MediaInfo Parse(TLMessageMediaPhoto media)
        {
            var photo = media.Photo as TeleSharp.TL.TLPhoto;
            return new MediaInfo(photo);
        }

        private static MediaInfo Parse(TLMessageMediaUnsupported media)
        {
            return null;
        }

        private static MediaInfo Parse(TLMessageMediaVenue media)
        {
            return null;
        }

        private static MediaInfo Parse(TLMessageMediaWebPage media)
        {
            return null;
        }
    }
}