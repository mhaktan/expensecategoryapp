using AutoMapper;
using ExpenseCategoryApp.Entities;
using ExpenseCategoryApp.ExpenseReports.Dto;

namespace ExpenseCategoryApp.ExpenseReports
{
    public class ExpenseReportMapProfile : Profile
    {
        public ExpenseReportMapProfile()
        {
            CreateMap<ExpenseReport, ExpenseReportDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreateExpenseReportDto, ExpenseReport>();
            CreateMap<ExpenseReportDto, ExpenseReport>();
        }
    }
}
