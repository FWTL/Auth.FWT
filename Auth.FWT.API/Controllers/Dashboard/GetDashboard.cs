using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Extensions;
using Auth.FWT.CQRS;
using Auth.FWT.Infrastructure.Handlers;
using StackExchange.Redis;

namespace Auth.FWT.API.Controllers.Dashboard
{
    public class GetDashboard
    {
        public class Query : IQuery
        {
            public int UserId { get; set; }
            public int Offset { get; set; }
            public int Limit { get; set; }
        }

        public class Result
        {
            public Result()
            {
            }

            public Result(TelegramJob telegramJob)
            {
                Id = telegramJob.Id;
                CreateDate = telegramJob.CreatedDateUTC.ToLocalTime();
                LastStatusUpdateDate = telegramJob.LastStatusUpdateDateUTC.ToLocalTime();
                Status = telegramJob.Status.GetDescription();
            }

            public long Id { get; set; }

            public DateTime CreateDate { get; set; }
            public int Fetched { get; set; }
            public DateTime LastStatusUpdateDate { get; set; }
            public string Status { get; set; }
            public int Total { get; set; }
        }

        public class Cache : RedisJsonHandler<Query, List<Result>>
        {
            public Cache(IDatabase cache) : base(cache)
            {
                KeyFn = query => { return $"GetDashboard{query.UserId}.{query.Offset}.{query.Limit}"; };
            }

            public override TimeSpan? Ttl(Query query)
            {
                if (query.Offset == 0 && query.Limit == 10)
                {
                    return TimeSpan.FromDays(1);
                }

                return TimeSpan.FromMinutes(5);
            }
        }

        public class Handler : IQueryHandler<Query, List<Result>>
        {
            private IDatabase _cache;
            private ConnectionMultiplexer _conn;
            private IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork, IDatabase cache, ConnectionMultiplexer conn)
            {
                _unitOfWork = unitOfWork;
                _cache = cache;
                _conn = conn;
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public async Task<List<Result>> Handle(Query query)
            {
                var resultQuery = _unitOfWork.TelegramJobRepository.Query()
                    .Where(tj => tj.UserId == query.UserId)
                    .OrderByDescending(tj => tj.Id)
                    .Paginate(query.Offset, query.Limit);

                var results = (await resultQuery.ToListAsync()).Select(tj => new Result(tj)).ToList();

                foreach (var item in resultQuery.Where(r => r.Status == Core.Enums.Enum.TelegramJobStatus.Fetching))
                {
                    var result = results.FirstOrDefault(r => r.Id == item.Id);
                    result.Fetched = (await _cache.StringGetAsync($"Fetching{item.JobId}")).ToN<int>() ?? 0;
                    result.Total = (await _cache.StringGetAsync($"FetchingTotal{item.JobId}")).ToN<int>() ?? 0;
                }

                return results;
            }
        }
    }
}