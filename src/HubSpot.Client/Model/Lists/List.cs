using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HubSpot.Model.Lists
{
    public class HubSpotList
    {
        [JsonProperty("listId")]
        public string ListId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("objectTypeId")]
        public string ObjectTypeId { get; set; }

        [JsonProperty("processingType")]
        public string ProcessingType { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("listVersion")]
        public int ListVersion { get; set; }

        [JsonProperty("filterBranch")]
        public object FilterBranch { get; set; }

        [JsonProperty("additionalProperties")]
        public IReadOnlyDictionary<string, string> AdditionalProperties { get; set; }
    }

    public class ListSearchRequest
    {
        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; } = 100;

        [JsonProperty("processingTypes")]
        public IReadOnlyList<string> ProcessingTypes { get; set; }

        [JsonProperty("additionalProperties")]
        public IReadOnlyList<string> AdditionalProperties { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }
    }

    public class ListSearchResponse
    {
        [JsonProperty("lists")]
        public IReadOnlyList<HubSpotList> Lists { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class ListMemberRecord
    {
        [JsonProperty("recordId")]
        public string RecordId { get; set; }

        [JsonProperty("membershipTimestamp")]
        public DateTimeOffset MembershipTimestamp { get; set; }
    }

    public class ListMembershipResponse
    {
        [JsonProperty("results")]
        public IReadOnlyList<ListMemberRecord> Results { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("paging")]
        public ListPaging Paging { get; set; }
    }

    public class ListPaging
    {
        [JsonProperty("next")]
        public ListPagingNext Next { get; set; }
    }

    public class ListPagingNext
    {
        [JsonProperty("after")]
        public string After { get; set; }
    }

    public class ListIdMapping
    {
        [JsonProperty("listId")]
        public string ListId { get; set; }

        [JsonProperty("legacyListId")]
        public string LegacyListId { get; set; }
    }

    public class ListIdMappingBatchResponse
    {
        [JsonProperty("legacyListIdsToIdsMapping")]
        public IReadOnlyList<ListIdMapping> Mappings { get; set; }

        [JsonProperty("missingLegacyListIds")]
        public IReadOnlyList<string> MissingLegacyListIds { get; set; }
    }
}
