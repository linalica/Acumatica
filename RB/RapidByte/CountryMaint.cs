using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;

// Only two explicitly defined actions are added to the graph, Cancel and Save
namespace RB.RapidByte
{
    public class CountryMaint : PXGraph<CountryMaint>
    {
        public PXSelect<Country> Countries;

        public PXCancel<Country> Cancel;
        public PXSave<Country> Save;

    }
}