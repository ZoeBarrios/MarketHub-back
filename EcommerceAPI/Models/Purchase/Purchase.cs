﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.Purchase
{
    public class Purchase
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Required]
        public int UserId { get; set; }


        [ForeignKey("UserId")]
        public User.User User { get; set; } = null!;


        public List<Publication.Publication> Publications = null!;


    }
    public class PurchasePublication
    {
        
        public int PublicationId { get; set; }

        public int PurchaseId { get; set; }
        
    }
}
