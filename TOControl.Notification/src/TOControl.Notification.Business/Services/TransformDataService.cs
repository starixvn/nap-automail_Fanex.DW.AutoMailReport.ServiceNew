using TOControl.Notification.Business.Models;
using TOControl.Notification.Business.Repositories;

namespace TOControl.Notification.Business.Services
{
    public interface TransformDataService
    {
        GroupHeadline TransformGroupHeadline(GroupHeadlineRaw groupHeadlineRaw, RawData rawData);

        CustomerMovement TransformMovement(CustomerMovementRaw movementRaw, RawData rawData);

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
            var newHeadlineInfo = repositoryService.QueryCustomerByCustId(groupHeadlineRaw.CustId);

            return new GroupHeadline
            {
                Id = rawData.Id,
                OldHeadline = oldHeadline,
                NewHeadline = newHeadlineInfo.Username,
                Date = rawData.CreatedDate,
                Currency = newHeadlineInfo.Currency
            };
        }

        public CustomerMovement TransformMovement(CustomerMovementRaw movementRaw, RawData rawData)
        {
            var groupHeadline = repositoryService.QuerySingleRow<string>(new GroupHeadlineSelectByGroupIdCrietria { GroupId = movementRaw.GroupId });
            var groupMember = repositoryService.QueryCustomerByCustId(movementRaw.CustId);

            return new CustomerMovement
            {
                Id = rawData.Id,
                GroupHeadline = groupHeadline,
                GroupMember = groupMember.Username,
                Date = rawData.CreatedDate,
                Action = rawData.Action,
                Currency = groupMember.Currency
            };
        }

        public DirectCustomer TransformDirectCustomer(DirectCustomerRaw directCustomerRaw, RawData rawData)
        {
            var userInfo = repositoryService.QueryCustomerByCustId(directCustomerRaw.CustId);
            if (userInfo == null)
            {
                return new DirectCustomer { };
            }

            return new DirectCustomer
            {
                Id = rawData.Id,
                Username = userInfo.Username,
                Level = userInfo.Level,
                Date = rawData.CreatedDate,
                Currency = userInfo.Currency
            };
        }
    }
}