using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lab10New;

public partial class Ticker
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ticker")]
    [StringLength(10)]
    public string Ticker1 { get; set; } = null!;

    [InverseProperty("Ticker")]
    public virtual ICollection<Price> Prices { get; } = new List<Price>();

    [InverseProperty("Ticker")]
    public virtual ICollection<TodayCondition> TodayConditions { get; } = new List<TodayCondition>();
}