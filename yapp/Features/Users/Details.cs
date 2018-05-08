using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using yapp.Infrastructure;
using yapp.Infrastructure.Errors;

namespace yapp.Features.Users
{
    public class Details
    {
        public class Query : IRequest<UserEnvelope>
        {
            public string UserName { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.UserName).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, UserEnvelope>
        {
            private readonly YappDbContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(YappDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var person = await _context.Persons
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserName == message.UserName, cancellationToken);
                if (person == null)
                {
                    throw new RestException(HttpStatusCode.NotFound);
                }

                return new UserEnvelope(_mapper.Map<Domain.Person, User>(person));
            }
        }
    }
}