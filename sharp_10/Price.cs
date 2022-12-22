
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lab10New;

public partial class Price
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("tickerId")]
    public int TickerId { get; set; }

    [Column("price")]
    public float Price1 { get; set; }

    [Column("date", TypeName = "date")]
    public DateTime Date { get; set; }

    [ForeignKey("TickerId")]
    [InverseProperty("Prices")]
    public virtual Ticker Ticker { get; set; } = null!;
}