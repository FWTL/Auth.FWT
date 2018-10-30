using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FWT.Infrastructure.Grid
{
    public class PaginatedResults<TData>
    {
        public PaginatedResults()
        {
        }

        public PaginatedResults(long total, PaginationParams paginationParams, SortParams sortParams, List<TData> data)
        {
            BuildUrl(total, paginationParams, sortParams, data);
            CalculateProperties(total, paginationParams);
            Data = data;
        }

        private void BuildUrl(long total, PaginationParams paginationParams, SortParams sortParams, List<TData> data)
        {
            Uri baseUri = new Uri(paginationParams.Host);
            Uri uri = new Uri(baseUri, paginationParams.Path);

            var lastPage = (int)Math.Ceiling(total / (decimal)paginationParams.PerPage);

            if (paginationParams.Page + 1 <= lastPage)
            {
                var queryNext = new Dictionary<string, string>
                {
                    { nameof(SortParams.ColumnNo), $"{ sortParams.ColumnNo}" },
                    { nameof(SortParams.Direction), $"{ sortParams.ColumnNo}" },
                };

                NextPageUrl = QueryHelpers.AddQueryString(uri.ToString(), queryNext);
            }

            if (paginationParams.Page - 1 >= 1)
            {
                var queryPrevious = new Dictionary<string, string>
                {
                    { nameof(SortParams.ColumnNo), $"{ sortParams.ColumnNo}" },
                    { nameof(SortParams.Direction), $"{ sortParams.ColumnNo}" },
                };

                PreviousPageUrl = QueryHelpers.AddQueryString(uri.ToString(), queryPrevious);
            }
        }

        private void CalculateProperties(long total, PaginationParams paginationParams)
        {
            PerPage = (int)paginationParams.PerPage;
            CurrentPage = paginationParams.Page;
            LastPage = (int)Math.Ceiling(total / (decimal)paginationParams.PerPage);
            From = paginationParams.Offset + 1;
            To = CalcualteTo(total, paginationParams);
            Total = total;
        }

        private long CalcualteTo(long total, PaginationParams paginationParams)
        {
            if (CurrentPage == LastPage)
            {
                return total;
            }

            return paginationParams.Offset + (int)paginationParams.PerPage + 1;
        }

        public long Total { get; set; }

        [JsonProperty(PropertyName = "per_page")]
        public int PerPage { get; set; }

        [JsonProperty(PropertyName = "current_page")]
        public long CurrentPage { get; set; }

        [JsonProperty(PropertyName = "last_page")]
        public int LastPage { get; set; }

        [JsonProperty(PropertyName = "next_page_url")]
        public string NextPageUrl { get; set; }

        [JsonProperty(PropertyName = "prev_page_url")]
        public string PreviousPageUrl { get; set; }

        public long From { get; set; }

        public long To { get; set; }

        public List<TData> Data { get; set; } = new List<TData>();
    }
}