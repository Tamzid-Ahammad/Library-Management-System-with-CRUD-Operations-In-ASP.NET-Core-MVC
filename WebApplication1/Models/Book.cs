using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

        [Required]
        [StringLength(50)]
        public string BookTitle { get; set; }

        public string AuthorName { get; set; }
        public decimal RentPrice { get; set; }

        public bool IsAvailable { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BookBorrowingDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookReturningDate { get; set; } = DateTime.Now.AddDays(7);
        public int BookReturningTimeInDays => (BookReturningDate - BookBorrowingDate).Days;
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}
