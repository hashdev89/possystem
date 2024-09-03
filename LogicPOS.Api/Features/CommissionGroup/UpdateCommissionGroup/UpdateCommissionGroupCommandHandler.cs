﻿using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.CommissionGroups.UpdateCommissionGroup
{
    public class UpdateCommissionGroupCommandHandler : RequestHandler<UpdateCommissionGroupCommand, ErrorOr<Unit>>
    {
        public UpdateCommissionGroupCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateCommissionGroupCommand command,
                                                           CancellationToken cancellationToken = default)
        {
            try
            {
                var httpResponse = await _httpClient.PutAsJsonAsync($"users/commission-groups/{command.Id}",
                                                                     command,
                                                                     cancellationToken);

                return await HandleUpdateEntityHttpResponseAsync(httpResponse);

            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}