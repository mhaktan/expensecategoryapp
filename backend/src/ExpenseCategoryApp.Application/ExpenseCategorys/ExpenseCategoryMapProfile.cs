using AutoMapper;
using ExpenseCategoryApp.Entities;
using ExpenseCategoryApp.ExpenseCategorys.Dto;

namespace ExpenseCategoryApp.ExpenseCategorys
{
    public class ExpenseCategoryMapProfile : Profile
    {
        public ExpenseCategoryMapProfile()
        {
            CreateMap<ExpenseCategory, ExpenseCategoryDto>();
            CreateMap<CreateExpenseCategoryDto, ExpenseCategory>();
            CreateMap<ExpenseCategoryDto, ExpenseCategory>();
        }
    }
}
