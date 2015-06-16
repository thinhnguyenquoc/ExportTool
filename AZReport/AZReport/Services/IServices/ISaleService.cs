﻿using AZReport.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AZReport.Services.IServices
{
    public interface ISaleService : IEntityService<Sale>
    {
        Sale CheckAndUpdate(Sale sale);
    }
}