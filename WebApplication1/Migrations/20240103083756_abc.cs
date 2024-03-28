using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class abc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string SpInsertStudent = @"CREATE or ALTER PROCEDURE dbo.SpInsertStudent
@StudentName nvarchar(50),
@Address nvarchar(max),
@Email nvarchar(max),
@ContactNo nvarchar(max),
@ImagePath nvarchar(max)

as
INSERT INTO [dbo].[Students]
           ([StudentName]
           ,[Address]
           ,[Email]
           ,[ContactNo]
           ,[ImagePath])
     VALUES
          (@StudentName,@Address,@Email,@ContactNo,@ImagePath)
		  return @@identity
GO";

            migrationBuilder.Sql(SpInsertStudent);

            string SpInsertBook = @"Create or ALTER procedure dbo.SpInsertBook
@BookTitle nvarchar(50),
@AuthorName nvarchar(max),
@RentPrice decimal(18,2),
@IsAvailable bit,
@BookBorrowingDate datetime2(7),
@BookReturningDate datetime2(7),
@StudentId int,
@GenreId int

as

INSERT INTO [dbo].[Books]
           ([BookTitle]
           ,[AuthorName]
           ,[RentPrice]
           ,[IsAvailable]
           ,[BookBorrowingDate]
           ,[BookReturningDate]
           ,[StudentId]
           ,[GenreId])
     VALUES
           (@BookTitle,@AuthorName,@RentPrice,@IsAvailable,@BookBorrowingDate,@BookReturningDate,@StudentId,@GenreId)
		   return @@rowcount
GO";
            migrationBuilder.Sql(SpInsertBook);

            string SpUpdateStudent = @"create or ALTER procedure dbo.SpUpdateStudent
@StudentId int,
@StudentName nvarchar(50),
@Address nvarchar(max),
@Email nvarchar(max),
@ContactNo nvarchar(max),
@ImagePath nvarchar(max)
as
UPDATE [dbo].[Students]
   SET [StudentName] =@StudentName 
      ,[Address] = @Address
      ,[Email] = @Email
      ,[ContactNo] = @ContactNo
      ,[ImagePath] = @ImagePath
 WHERE StudentId=@StudentId
 delete from Books where StudentId=@StudentId
 return @@rowcount
GO";
            migrationBuilder.Sql(SpUpdateStudent);

            string SpDeleteStudent = @"Create or ALTER procedure dbo.SpDeleteStudent
@StudentId int
as
DELETE FROM Books where StudentId=@StudentId
DELETE FROM Students Where StudentId=@StudentId
return @@rowcount
GO";
            migrationBuilder.Sql(SpDeleteStudent);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop proc SpInsertStudent");
            migrationBuilder.Sql("drop proc SpInsertBook");
            migrationBuilder.Sql("drop proc SpUpdateStudent");
            migrationBuilder.Sql("drop proc SpDeleteStudent");

        }
    }
}

