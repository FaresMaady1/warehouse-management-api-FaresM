using System;
using System.Collections.Generic;

namespace WebApi.DbFirst.Models;

public partial class ProductImage
{
    public Guid ProductId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
