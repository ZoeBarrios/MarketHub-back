﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.Purchase
{
    public class Purchase
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Required]
        public int UserId { get; set; }


        [ForeignKey("UserId")]

        public User.User User { get; set; } = null!;

        [Required]
        public int SellerId { get; set; }

        [ForeignKey("SellerId")]
        public User.User Seller { get; set; } = null!;

        public bool WasDelivered { get; set; } = false;


        public List<Publication.Publication> Publications { get; set; } = null!;

    

    }
   
}
