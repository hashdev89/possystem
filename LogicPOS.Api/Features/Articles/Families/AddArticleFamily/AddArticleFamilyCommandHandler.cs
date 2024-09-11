﻿using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Families.AddArticleFamily
{
    public class AddArticleFamilyCommandHandler :
        RequestHandler<AddArticleFamilyCommand, ErrorOr<Guid>>
    {
        public AddArticleFamilyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddArticleFamilyCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("articles/families", command, cancellationToken);
        }
    }
}