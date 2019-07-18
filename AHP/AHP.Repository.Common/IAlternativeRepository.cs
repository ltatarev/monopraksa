﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHP.Model.Common;

namespace AHP.Repository.Common
{
	public interface IAlternativeRepository
	{
        #region Methods

        List<IAlternative> Get();

        #endregion
    }
}
