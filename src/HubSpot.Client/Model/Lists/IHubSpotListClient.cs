using System.Collections.Generic;
using System.Threading.Tasks;

namespace HubSpot.Model.Lists
{
    public interface IHubSpotListClient
    {
        Task<HubSpotList> CreateAsync(string name, string objectTypeId, string processingType);

        Task<HubSpotList> GetByIdAsync(string listId, bool includeFilters = false);

        Task<IReadOnlyList<HubSpotList>> GetManyByIdAsync(IReadOnlyList<string> listIds, bool includeFilters = false);

        Task<ListSearchResponse> SearchAsync(ListSearchRequest request);

        Task UpdateNameAsync(string listId, string name);

        Task UpdateFiltersAsync(string listId, object filterBranch);

        Task DeleteAsync(string listId);

        Task<ListMembershipResponse> GetMembershipsAsync(string listId, string after = null, int? limit = null);

        Task<ListMembershipResponse> GetMembershipsByJoinOrderAsync(string listId, string after = null, int? limit = null);

        Task AddMembershipsAsync(string listId, IReadOnlyList<string> recordIds);

        Task RemoveMembershipsAsync(string listId, IReadOnlyList<string> recordIds);

        Task<ListIdMapping> GetIdMappingAsync(string legacyListId);

        Task<ListIdMappingBatchResponse> GetIdMappingBatchAsync(IReadOnlyList<string> legacyListIds);
    }
}
