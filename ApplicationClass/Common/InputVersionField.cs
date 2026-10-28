using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer;

public class InputVersionField
{

    [HiddenInput(DisplayValue = false)]
    public byte[]? ConcurrencyToken { get; init; }
}
