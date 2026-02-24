using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using HubSpot;
using HubSpot.Contacts.Filters;
using HubSpot.Model;
using HubSpot.Model.Contacts;
using HubSpot.Model.Lists;
using Moq;
using NUnit.Framework;
using ModelContact = HubSpot.Model.Contacts.Contact;

namespace Tests.Contacts.Filters {
    [TestFixture]
    public class ListContactFilterTests
    {
        [Test, CustomAutoData]
        public async Task Contacts_are_fetched_from_memberships(
            [Frozen] Mock<IHubSpotListClient> mockListClient,
            [Frozen] Mock<IHubSpotContactClient> mockContactClient,
            IHubSpotClient client,
            ListContactFilter sut,
            IReadOnlyList<Property> properties,
            IFixture fixture,
            long contactId1, long contactId2)
        {
            fixture.Customize<ModelContact>(cu => cu.OmitAutoProperties().With(p => p.Id));

            var memberRecords = new[] { contactId1, contactId2 }
                .Select(id => fixture.Build<ListMemberRecord>().With(p => p.RecordId, id.ToString()).Create())
                .ToArray();

            var memberships = fixture.Build<ListMembershipResponse>()
                                     .With(p => p.Results, (IReadOnlyList<ListMemberRecord>)memberRecords)
                                     .Without(p => p.Paging)
                                     .Create();

            mockListClient.Setup(p => p.GetMembershipsAsync(It.IsAny<string>(), null, It.IsAny<int>()))
                          .ReturnsAsync(memberships);

            var contacts = new[] { contactId1, contactId2 }
                .Select(id => fixture.Build<ModelContact>().With(p => p.Id, id).Create())
                .ToDictionary(c => c.Id);

            mockContactClient.Setup(p => p.GetManyByIdAsync(It.IsAny<IReadOnlyList<long>>(), properties, PropertyMode.ValueOnly, FormSubmissionMode.None, false, false))
                             .ReturnsAsync(contacts);

            var response = await sut.GetContacts(client, properties);

            Assert.That(response, Has.Count.EqualTo(2));
            mockListClient.Verify(p => p.GetMembershipsAsync(It.IsAny<string>(), null, It.IsAny<int>()), Times.Once);
            mockContactClient.Verify(p => p.GetManyByIdAsync(It.Is<IReadOnlyList<long>>(ids => ids.Contains(contactId1) && ids.Contains(contactId2)), properties, PropertyMode.ValueOnly, FormSubmissionMode.None, false, false), Times.Once);
        }

        [Test, CustomAutoData]
        public async Task Empty_membership_list_returns_no_contacts(
            [Frozen] Mock<IHubSpotListClient> mockListClient,
            [Frozen] Mock<IHubSpotContactClient> mockContactClient,
            IHubSpotClient client,
            ListContactFilter sut,
            IReadOnlyList<Property> properties,
            IFixture fixture)
        {
            var memberships = fixture.Build<ListMembershipResponse>()
                                     .With(p => p.Results, Enumerable.Empty<ListMemberRecord>().ToArray() as IReadOnlyList<ListMemberRecord>)
                                     .Without(p => p.Paging)
                                     .Create();

            mockListClient.Setup(p => p.GetMembershipsAsync(It.IsAny<string>(), null, It.IsAny<int>()))
                          .ReturnsAsync(memberships);

            var response = await sut.GetContacts(client, properties);

            Assert.That(response, Is.Empty);
            mockContactClient.Verify(
                p => p.GetManyByIdAsync(It.IsAny<IReadOnlyList<long>>(), It.IsAny<IReadOnlyList<IProperty>>(), It.IsAny<PropertyMode>(), It.IsAny<FormSubmissionMode>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Test, CustomAutoData]
        public async Task Multiple_membership_pages_are_fetched(
            [Frozen] Mock<IHubSpotListClient> mockListClient,
            [Frozen] Mock<IHubSpotContactClient> mockContactClient,
            IHubSpotClient client,
            ListContactFilter sut,
            IReadOnlyList<Property> properties,
            IFixture fixture,
            long contactId1, long contactId2,
            string afterCursor)
        {
            fixture.Customize<ModelContact>(cu => cu.OmitAutoProperties().With(p => p.Id));

            var firstPage = fixture.Build<ListMembershipResponse>()
                                   .With(p => p.Results, (IReadOnlyList<ListMemberRecord>)new[] { fixture.Build<ListMemberRecord>().With(p => p.RecordId, contactId1.ToString()).Create() })
                                   .With(p => p.Paging, fixture.Build<ListPaging>().With(p => p.Next, fixture.Build<ListPagingNext>().With(p => p.After, afterCursor).Create()).Create())
                                   .Create();

            var secondPage = fixture.Build<ListMembershipResponse>()
                                    .With(p => p.Results, (IReadOnlyList<ListMemberRecord>)new[] { fixture.Build<ListMemberRecord>().With(p => p.RecordId, contactId2.ToString()).Create() })
                                    .Without(p => p.Paging)
                                    .Create();

            mockListClient.Setup(p => p.GetMembershipsAsync(It.IsAny<string>(), null, It.IsAny<int>()))
                          .ReturnsAsync(firstPage);
            mockListClient.Setup(p => p.GetMembershipsAsync(It.IsAny<string>(), afterCursor, It.IsAny<int>()))
                          .ReturnsAsync(secondPage);

            var contacts = new[] { contactId1, contactId2 }
                .Select(id => fixture.Build<ModelContact>().With(p => p.Id, id).Create())
                .ToDictionary(c => c.Id);

            mockContactClient.Setup(p => p.GetManyByIdAsync(It.IsAny<IReadOnlyList<long>>(), properties, PropertyMode.ValueOnly, FormSubmissionMode.None, false, false))
                             .ReturnsAsync(contacts);

            var response = await sut.GetContacts(client, properties);

            Assert.That(response, Has.Count.EqualTo(2));
            mockListClient.Verify(p => p.GetMembershipsAsync(It.IsAny<string>(), null, It.IsAny<int>()), Times.Once);
            mockListClient.Verify(p => p.GetMembershipsAsync(It.IsAny<string>(), afterCursor, It.IsAny<int>()), Times.Once);
        }
    }
}
