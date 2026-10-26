using AutoMapper;
using ExpenseCategoryApp.Entities;
using ExpenseCategoryApp.Budgets.Dto;

namespace ExpenseCategoryApp.Budgets
{
    public class BudgetMapProfile : Profile
    {
        public BudgetMapProfile()
        {
            CreateMap<Budget, BudgetDto>();
            CreateMap<CreateBudgetDto, Budget>();
            CreateMap<BudgetDto, Budget>();
        }
    }
}
