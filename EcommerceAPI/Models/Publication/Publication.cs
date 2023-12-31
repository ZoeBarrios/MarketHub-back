﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace EcommerceAPI.Models.Publication
{
    public class Publication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PublicationId { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(512)]
        public string Description { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public int? Stock { get; set; } = 1;

        public DateTime CreatedAt = DateTime.Now;

        public bool IsPaused { get; set; } = false;

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User.User User { get; set; } = null!;


        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category.Category Category { get; set; } = null!;

        


    }

    public class PurchasePublication
    {

        public int PublicationId { get; set; }

        public int PurchaseId { get; set; }


    }


}
