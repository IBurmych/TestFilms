using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestFilms.Migrations
{
    /// <inheritdoc />
    public partial class addingGetCategoriesNesting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCategoriesNesting]') AND type in (N'P', N'PC'))
                DROP PROCEDURE [dbo].[GetCategoriesNesting]
                GO
                CREATE PROCEDURE [dbo].[GetCategoriesNesting] @Id INT
                AS
                BEGIN
	                WITH CTE as 
		                (
			                SELECT n = 0, Parent_category_id FROM dbo.Categories WHERE Id = @Id
			                UNION all
			                SELECT n + 1, c.Parent_category_id FROM CTE 
				                inner join dbo.Categories c ON cte.Parent_category_id = c.Id
		                )
	                SELECT MAX(n) AS Nesting
	                FROM CTE
                END
            ");
            migrationBuilder.Sql(@"
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsCategoriesLoop]') AND type in (N'P', N'PC'))
                DROP PROCEDURE [dbo].[IsCategoriesLoop]
                GO
                CREATE PROCEDURE [dbo].[IsCategoriesLoop] @id int, @parentId int
                AS
                BEGIN
                    WITH CTE as 
                        (
                            SELECT n = CASE WHEN @id = @parentId THEN 1 ELSE 0 END, Parent_category_id, Id FROM dbo.Categories WHERE Id = @parentId
                            UNION all
                            SELECT n = CASE WHEN @id = c.Id THEN 1 ELSE 0 END, c.Parent_category_id, c.Id FROM CTE 
                                inner join dbo.Categories c ON cte.Parent_category_id = c.Id AND @id != cte.Id
                        )
                    SELECT max(n)
                    FROM CTE
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCategoriesNesting]') AND type in (N'P', N'PC'))
                DROP PROCEDURE [dbo].[GetCategoriesNesting]
                GO
            ");
            migrationBuilder.Sql(@"
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsCategoriesLoop]') AND type in (N'P', N'PC'))
                DROP PROCEDURE [dbo].[IsCategoriesLoop]
                GO
            ");
        }
    }
}
