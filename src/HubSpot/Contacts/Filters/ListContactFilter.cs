using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HubSpot.Model;
using HubSpot.Model.Contacts;

namespace HubSpot.Contacts.Filters
{
    public class ListContactFilter : IContactFilter
    {
        private readonly string _listId;
        private const int MembershipPageSize = 250;
        private const int ContactBatchSize = 100;

        public ListContactFilter(string listId)
        {
            _listId = listId;
        }

        public async Task<IReadOnlyList<Model.Contacts.Contact>> GetContacts(IHubSpotClient client, IReadOnlyList<IProperty> propertiesToQuery)
        {
            var recordIds = await GetAllMemberRecordIds(client).ConfigureAwait(false);

            if (recordIds.Count == 0)
            {
                return new Model.Contacts.Contact[0];
            }

            var contacts = new List<Model.Contacts.Contact>();

            for (int i = 0; i < recordIds.Count; i += ContactBatchSize)
            {
                var batch = recordIds.Skip(i).Take(ContactBatchSize).ToList();
                var batchResult = await client.Contacts.GetManyByIdAsync(batch, propertiesToQuery, PropertyMode.ValueOnly, FormSubmissionMode.None).ConfigureAwait(false);
                contacts.AddRange(batchResult.Values);
            }

            return contacts;
        }

        private async Task<IReadOnlyList<long>> GetAllMemberRecordIds(IHubSpotClient client)
        {
            var allIds = new List<long>();
            string after = null;

            do
            {
                var response = await client.Lists.GetMembershipsAsync(_listId, after, MembershipPageSize).ConfigureAwait(false);

                if (response?.Results != null)
                {
                    foreach (var membership in response.Results)
                    {
                        if (long.TryParse(membership.RecordId, out var id))
                        {
                            allIds.Add(id);
                        }
                    }
                }

                after = response?.Paging?.Next?.After;

            } while (after != null);

            return allIds;
        }
    }
}
