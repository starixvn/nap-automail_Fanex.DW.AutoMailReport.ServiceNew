using TOControl.CustomerMovement.Business.Models;
using TOControl.CustomerMovement.Business.Repositories;

namespace TOControl.CustomerMovement.Business.Services
{
    public interface TransformDataService
    {
        GroupHeadline TransformGroupHeadline(GroupHeadlineRaw groupHeadlineRaw, RawData rawData);

        Movement TransformMovement(MovementRaw movementRaw, RawData rawData);

        DirectCustomer TransformDirectCustomer(DirectCustomerRaw directCustomerRaw, RawData rawData);
    }

    public class TransformDataServiceImp : TransformDataService
    {
        private readonly TOControlRepositoryService repositoryService;

        public TransformDataServiceImp(TOControlRepositoryService repositoryService)
        {
            this.repositoryService = repositoryService;
        }

        public GroupHeadline TransformGroupHeadline(GroupHeadlineRaw groupHeadlineRaw, RawData rawData)
        {
            var oldHeadline = repositoryService.QueryCustomerByCustId(groupHeadlineRaw.OldCustId).Username;
            var newHeadline = repositoryService.QueryCustomerByCustId(groupHeadlineRaw.CustId).Username;

            return new GroupHeadline
            {
                Id = rawData.Id,
                OldHeadline = oldHeadline,
                NewHeadline = newHeadline,
                Date = rawData.CreatedDate
            };
        }

        public Movement TransformMovement(MovementRaw movementRaw, RawData rawData)
        {
            var groupHeadline = repositoryService.QuerySingleRow<string>(new GroupHeadlineSelectByGroupIdCrietria { GroupId = movementRaw.GroupId });
            var groupMember = repositoryService.QueryCustomerByCustId(movementRaw.CustId).Username;

            return new Movement
            {
                Id = rawData.Id,
                GroupHeadline = groupHeadline,
                GroupMember = groupMember,
                Date = rawData.CreatedDate,
                Action = rawData.Action
            };
        }

        public DirectCustomer TransformDirectCustomer(DirectCustomerRaw directCustomerRaw, RawData rawData)
        {
            var userInfo = repositoryService.QueryCustomerByCustId(directCustomerRaw.CustId);
            var username = userInfo.Username;
            var level = userInfo.Level;

            return new DirectCustomer
            {
                Id = rawData.Id,
                Username = username,
                Level = level,
                Date = rawData.CreatedDate
            };
        }
    }
}