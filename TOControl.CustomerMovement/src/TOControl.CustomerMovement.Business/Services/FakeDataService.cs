using System;
using System.Collections.Generic;
using System.Linq;
using TOControl.CustomerMovement.Business.Enums;
using TOControl.CustomerMovement.Business.Models;

namespace TOControl.CustomerMovement.Business.Services
{
    public class FakeDataService : DataService
    {
        public List<int> Ids
        {
            get
            {
                return new List<int> { 1, 2, 3 };
            }
        }

        public Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>> Query(NotifiedByFunction notifiedByFunction)
        {
            var groupHeadline = CreateGroupHeadline();
            var customerMovememnt = CreateCustomerMovement();
            var directCustomer = CreateDirectCustomer();

            return new Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>>(groupHeadline, customerMovememnt, directCustomer);
        }

        private IEnumerable<GroupHeadline> CreateGroupHeadline()
        {
            return new List<GroupHeadline>
            {
                new GroupHeadline { OldHeadline = "Supertest1", NewHeadline = "Supertest1A", Date = new DateTime(2017, 01, 01, 01, 01, 01) },
                new GroupHeadline { OldHeadline = "Supertest2", NewHeadline = "Supertest2A", Date = new DateTime(2017, 01, 12, 12, 12, 12) }
            };
        }

        private IEnumerable<Movement> CreateCustomerMovement()
        {
            var movements = new List<Movement>
            {
                new Movement { GroupHeadline = "Supertest3", GroupMember = "Supertest4", Action = "Add", Date = new DateTime(2017, 01, 01, 01, 01, 01) },
                new Movement { GroupHeadline = "Supertest3", GroupMember = "Supertest5", Action = "Remove", Date = new DateTime(2017, 01, 12, 12, 12, 12) },
                new Movement { GroupHeadline = "Supertest6", GroupMember = "Master1", Action = "Add", Date = new DateTime(2017, 06, 11, 11, 11, 11) }
            };

            movements.ForEach(movement =>
            {
                movement.Count = movements.Count(s => s.GroupHeadline == movement.GroupHeadline);
            });

            return movements;
        }

        private IEnumerable<DirectCustomer> CreateDirectCustomer()
        {
            return new List<DirectCustomer>
            {
                new DirectCustomer { Username = "Supertest1", Level = "Super", Date = new DateTime(2017, 01, 01, 01, 01, 01) },
                new DirectCustomer { Username = "Master1", Level = "Master", Date = new DateTime(2017, 01, 12, 12, 12, 12) },
                new DirectCustomer { Username = "Agent01", Level = "Agent", Date = new DateTime(2017, 06, 11, 11, 11, 11) }
            };
        }
    }
}