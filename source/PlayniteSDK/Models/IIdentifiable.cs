﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playnite.SDK.Models
{
    public interface IIdentifiable
    {
        /// <summary>
        /// Gets unique object id.
        /// </summary>
        Guid Id { get; }
    }
}
