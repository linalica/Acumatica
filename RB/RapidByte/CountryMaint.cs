using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;


namespace RB.RapidByte
{
    public class CountryMaint : PXGraph<CountryMaint>
    {
        public PXSelect<Country> Countries;

        public PXCancel<Country> Cancel;
        public PXSave<Country> Save;

    }
}