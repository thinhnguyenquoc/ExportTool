﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AZReport.Services.IServices
{
    public interface IProgramService : IEntityService<Program>
    {
        Program CheckAndUpdate(Program program);
    }
}