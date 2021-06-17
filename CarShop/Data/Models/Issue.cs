namespace CarShop.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class Issue
    {
        [Key]
        [Required]
        [MaxLength(IdMaxLength)]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        public string Description { get; set; }

        public bool IsFixed { get; set; }

        [Required]
        public string CarId { get; set; }

        public Car Car { get; set; }
    }
}
