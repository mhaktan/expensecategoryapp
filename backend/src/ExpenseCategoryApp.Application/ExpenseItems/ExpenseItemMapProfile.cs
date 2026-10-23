using AutoMapper;
using ExpenseCategoryApp.Entities;
using ExpenseCategoryApp.ExpenseItems.Dto;

namespace ExpenseCategoryApp.ExpenseItems
{
    public class ExpenseItemMapProfile : Profile
    {
        public ExpenseItemMapProfile()
        {
            CreateMap<ExpenseItem, ExpenseItemDto>();
            CreateMap<CreateExpenseItemDto, ExpenseItem>();
            CreateMap<ExpenseItemDto, ExpenseItem>();
        }
    }
}
