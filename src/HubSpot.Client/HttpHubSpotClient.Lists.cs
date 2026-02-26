using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HubSpot.Model.Lists;
using Kralizek.Extensions.Http;

namespace HubSpot
{
    public partial class HttpHubSpotClient : IHubSpotListClient
    {
        async Task<HubSpotList> IHubSpotListClient.CreateAsync(string name, string objectTypeId, string processingType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(objectTypeId))
            {
                throw new ArgumentNullException(nameof(objectTypeId));
            }

            if (string.IsNullOrEmpty(processingType))
            {
                throw new ArgumentNullException(nameof(processingType));
            }

            var request = new
            {
                name,
                objectTypeId,
                processingType
            };

            var response = await _client.PostAsync<object, HubSpotList>("/crm/v3/lists", request);

            return response;
        }

        async Task<HubSpotList> IHubSpotListClient.GetByIdAsync(string listId, bool includeFilters)
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException(nameof(listId));
            }

            var builder = new HttpQueryStringBuilder();

            if (includeFilters)
            {
                builder.Add("includeFilters", "true");
            }

            var response = await _client.GetAsync<HubSpotList>($"/crm/v3/lists/{listId}", builder.BuildQuery());

            return response;
        }

        async Task<IReadOnlyList<HubSpotList>> IHubSpotListClient.GetManyByIdAsync(IReadOnlyList<string> listIds, bool includeFilters)
        {
            if (listIds == null || listIds.Count == 0)
            {
                return Array.Empty<HubSpotList>();
            }

            var builder = new HttpQueryStringBuilder();

            foreach (var id in listIds)
            {
                builder.Add("listIds", id);
            }

            if (includeFilters)
            {
                builder.Add("includeFilters", "true");
            }

            var response = await _client.GetAsync<ListSearchResponse>("/crm/v3/lists", builder.BuildQuery());

            return response?.Lists ?? Array.Empty<HubSpotList>();
        }

        async Task<ListSearchResponse> IHubSpotListClient.SearchAsync(ListSearchRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await _client.PostAsync<ListSearchRequest, ListSearchResponse>("/crm/v3/lists/search", request);

            return response;
        }

        async Task IHubSpotListClient.UpdateNameAsync(string listId, string name)
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException(nameof(listId));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var builder = new HttpQueryStringBuilder();
            builder.Add("listName", name);

            await _client.PutAsync($"/crm/v3/lists/{listId}/update-list-name", query: builder.BuildQuery());
        }

        async Task IHubSpotListClient.UpdateFiltersAsync(string listId, object filterBranch)
        {
            if (filterBranch == null)
            {
                throw new ArgumentNullException(nameof(filterBranch));
            }

            var request = new
            {
                filterBranch
            };

            await _client.PutAsync<object, object>($"/crm/v3/lists/{listId}/update-list-filters", request);
        }

        async Task IHubSpotListClient.DeleteAsync(string listId)
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException(nameof(listId));
            }

            await _client.DeleteAsync($"/crm/v3/lists/{listId}");
        }

        async Task<ListMembershipResponse> IHubSpotListClient.GetMembershipsAsync(string listId, string after, int? limit)
        {
            var builder = new HttpQueryStringBuilder();

            if (after != null)
            {
                builder.Add("after", after);
            }

            if (limit.HasValue)
            {
                builder.Add("limit", limit.Value);
            }

            var response = await _client.GetAsync<ListMembershipResponse>($"/crm/v3/lists/{listId}/memberships", builder.BuildQuery());

            return response;
        }

        async Task<ListMembershipResponse> IHubSpotListClient.GetMembershipsByJoinOrderAsync(string listId, string after, int? limit)
        {
            var builder = new HttpQueryStringBuilder();

            if (after != null)
            {
                builder.Add("after", after);
            }

            if (limit.HasValue)
            {
                builder.Add("limit", limit.Value);
            }

            var response = await _client.GetAsync<ListMembershipResponse>($"/crm/v3/lists/{listId}/memberships/join-order", builder.BuildQuery());

            return response;
        }

        async Task IHubSpotListClient.AddMembershipsAsync(string listId, IReadOnlyList<string> recordIds)
        {
            if (recordIds == null || recordIds.Count == 0)
            {
                throw new ArgumentException("At least one record ID is required", nameof(recordIds));
            }

            await _client.PutAsync<IReadOnlyList<string>, object>($"/crm/v3/lists/{listId}/memberships/add", recordIds);
        }

        async Task IHubSpotListClient.RemoveMembershipsAsync(string listId, IReadOnlyList<string> recordIds)
        {
            if (recordIds == null || recordIds.Count == 0)
            {
                throw new ArgumentException("At least one record ID is required", nameof(recordIds));
            }

            await _client.PutAsync<IReadOnlyList<string>, object>($"/crm/v3/lists/{listId}/memberships/remove", recordIds);
        }

        async Task<ListIdMapping> IHubSpotListClient.GetIdMappingAsync(string legacyListId)
        {
            var builder = new HttpQueryStringBuilder();
            builder.Add("legacyListId", legacyListId);

            var response = await _client.GetAsync<ListIdMapping>("/crm/v3/lists/idmapping", builder.BuildQuery());

            return response;
        }

        async Task<ListIdMappingBatchResponse> IHubSpotListClient.GetIdMappingBatchAsync(IReadOnlyList<string> legacyListIds)
        {
            if (legacyListIds == null || legacyListIds.Count == 0)
            {
                throw new ArgumentException("At least one legacy list ID is required", nameof(legacyListIds));
            }

            var response = await _client.PostAsync<IReadOnlyList<string>, ListIdMappingBatchResponse>("/crm/v3/lists/idmapping", legacyListIds);

            return response;
        }
    }
}
