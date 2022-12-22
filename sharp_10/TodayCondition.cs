using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lab10New;

[Table("TodayConditions")]
public partial class TodayCondition
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("tickerId")]
    public int TickerId { get; set; }

    [Column("state")]
    [StringLength(10)]
    public string State { get; set; } = null!;

    [ForeignKey("TickerId")]
    [InverseProperty("TodayConditions")]
    public virtual Ticker Ticker { get; set; } = null!;
}