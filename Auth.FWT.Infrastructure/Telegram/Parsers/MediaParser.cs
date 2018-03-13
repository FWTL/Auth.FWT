using System;
using System.Collections.Generic;
using System.Linq;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Services.Telegram;
using TeleSharp.TL;

namespace Auth.FWT.Infrastructure.Telegram.Parsers
{
    public static class MediaParser
    {
        private static readonly Dictionary<string, Func<TLAbsMessageMedia, ITelegramMedia>> Switch = new Dictionary<string, Func<TLAbsMessageMedia, ITelegramMedia>>()
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

        public static ITelegramMedia Parse(TLAbsMessageMedia media)
        {
            return Switch[media.GetType().FullName](media);
        }

        private static ITelegramMedia Parse(TLMessageMediaContact media)
        {
            return null;
        }

        private static ITelegramMedia Parse(TLMessageMediaDocument media)
        {
            var document = media.Document as TLDocument;

            var strickerAttributeConsturctorId = new TLDocumentAttributeSticker().Constructor;
            var tlStickerSet = document.Attributes.GetListOfValuesOf("Stickerset").Select(item => item["Stickerset"]).FirstOrDefault(item => item.IsNotNull());
            if (tlStickerSet != null)
            {
                var stickerSet = tlStickerSet as TLInputStickerSetID;
                return new MediaInfo(document, stickerSet);
            }

            return new MediaInfo(document);
        }

        private static ITelegramMedia Parse(TLMessageMediaGame media)
        {
            return null;
        }

        private static ITelegramMedia Parse(TLMessageMediaGeo media)
        {
            return null;
        }

        private static ITelegramMedia Parse(TLMessageMediaInvoice media)
        {
            return null;
        }

        private static ITelegramMedia Parse(TLMessageMediaPhoto media)
        {
            var photo = media.Photo as TLPhoto;
            return new PhotoInfo(photo);
        }

        private static ITelegramMedia Parse(TLMessageMediaUnsupported media)
        {
            return null;
        }

        private static ITelegramMedia Parse(TLMessageMediaVenue media)
        {
            return null;
        }

        private static ITelegramMedia Parse(TLMessageMediaWebPage media)
        {
            return null;
        }
    }
}