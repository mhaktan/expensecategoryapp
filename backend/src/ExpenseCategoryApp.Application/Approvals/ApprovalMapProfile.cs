using AutoMapper;
using ExpenseCategoryApp.Approvals.Dto;
using ExpenseCategoryApp.Entities;

namespace ExpenseCategoryApp.Approvals
{
    public class ApprovalMapProfile : Profile
    {
        public ApprovalMapProfile()
        {
            CreateMap<ApprovalRecord, ApprovalRecordDto>();
            CreateMap<StatusChangeLog, StatusChangeLogDto>();
        }
    }
}
