using AutoMapper;
using ExpenseCategoryApp.Entities;
using ExpenseCategoryApp.ExpenseApprovalThresholds.Dto;

namespace ExpenseCategoryApp.ExpenseApprovalThresholds
{
    public class ExpenseApprovalThresholdMapProfile : Profile
    {
        public ExpenseApprovalThresholdMapProfile()
        {
            CreateMap<ExpenseApprovalThreshold, ExpenseApprovalThresholdDto>();
            CreateMap<CreateExpenseApprovalThresholdDto, ExpenseApprovalThreshold>();
            CreateMap<ExpenseApprovalThresholdDto, ExpenseApprovalThreshold>();
        }
    }
}
