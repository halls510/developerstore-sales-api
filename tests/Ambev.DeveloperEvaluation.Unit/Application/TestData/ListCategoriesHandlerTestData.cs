using Ambev.DeveloperEvaluation.Application.Products.ListCategories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class ListCategoriesHandlerTestData
{
    public static ListCategoriesCommand GenerateValidCommand()
    {
        return new ListCategoriesCommand { Page = 1, Size = 10, OrderBy = "name" };
    }

    public static List<Category> GenerateCategoryList()
    {
        return new List<Category>
            {
                new Category { Id = 1, Name = "Bebidas" },
                new Category { Id = 2, Name = "Alimentos" },
                new Category { Id = 3, Name = "Higiene" }
            };
    }
}
