using System;
using System.Collections.Generic;
using TeleSharp.TL;

namespace Auth.FWT.Infrastructure.Telegram.Parsers
{
    public static class MediaParser
    {
        private static readonly Dictionary<string, Action<TLAbsMessageMedia>> Switch = new Dictionary<string, Action<TLAbsMessageMedia>>()
        {
            { typeof(TLMessageMediaEmpty).FullName, x => { } },
            { typeof(TLMessageMediaPhoto).FullName, x => { Parse(x as TLMessageMediaPhoto); } },
            { typeof(TLMessageMediaGeo).FullName, x => { Parse(x as TLMessageMediaGeo); } },
            { typeof(TLMessageMediaContact).FullName, x => { Parse(x as TLMessageMediaContact); } },
            { typeof(TLMessageMediaUnsupported).FullName, x => { Parse(x as TLMessageMediaUnsupported); } },
            { typeof(TLMessageMediaDocument).FullName, x => { Parse(x as TLMessageMediaDocument); } },
            { typeof(TLMessageMediaWebPage).FullName, x => { Parse(x as TLMessageMediaWebPage); } },
            { typeof(TLMessageMediaVenue).FullName, x => { Parse(x as TLMessageMediaVenue); } },
            { typeof(TLMessageMediaGame).FullName, x => { Parse(x as TLMessageMediaGame); } },
            { typeof(TLMessageMediaInvoice).FullName, x => { Parse(x as TLMessageMediaInvoice); } },
        };

        public static void Parse(TLAbsMessageMedia media)
        {
            Switch[media.GetType().FullName](media);
        }

        private static void Parse(TLMessageMediaContact media)
        {
        }

        private static void Parse(TLMessageMediaDocument media)
        {
        }

        private static void Parse(TLMessageMediaGame tLMessageMediaGame)
        {
        }

        private static void Parse(TLMessageMediaGeo media)
        {
        }

        private static void Parse(TLMessageMediaInvoice tLMessageMediaInvoice)
        {
        }

        private static void Parse(TLMessageMediaPhoto media)
        {
        }

        private static void Parse(TLMessageMediaUnsupported media)
        {
        }

        private static void Parse(TLMessageMediaVenue tLMessageMediaVenue)
        {
        }

        private static void Parse(TLMessageMediaWebPage media)
        {
        }
    }
}