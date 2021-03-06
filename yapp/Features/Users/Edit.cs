﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using yapp.Infrastructure;
using yapp.Infrastructure.Security;

namespace yapp.Features.Users
{
    public class Edit
    {
        public class UserData
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Image { get; set; }
        }

        public class Command : IRequest<UserEnvelope>
        {
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            private readonly YappDbContext _context;
            private readonly IPasswordHasher _passwordHasher;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly IMapper _mapper;

            public Handler(YappDbContext context, IPasswordHasher passwordHasher,
                ICurrentUserAccessor currentUserAccessor, IMapper mapper)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _currentUserAccessor = currentUserAccessor;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var currentUsername = _currentUserAccessor.GetCurrentUsername();
                var person = await _context.Persons.Where(x => x.UserName == currentUsername).FirstOrDefaultAsync(cancellationToken);

                person.UserName = message.User.UserName ?? person.UserName;
                person.Email = message.User.Email ?? person.Email;
                person.Image = message.User.Image ?? person.Image;

                if (!string.IsNullOrWhiteSpace(message.User.Password))
                {
                    var salt = Guid.NewGuid().ToByteArray();
                    person.Hash = _passwordHasher.Hash(message.User.Password, salt);
                    person.Salt = salt;
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new UserEnvelope(_mapper.Map<Domain.Person, User>(person));
            }
        }
    }
}