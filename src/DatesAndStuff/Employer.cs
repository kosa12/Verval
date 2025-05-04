using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatesAndStuff
{
    public class Employer {

        string taxId;
        string address;
        string ownername;
        List<int> activityDomains;

        public Employer(
            string taxId,
            string address,
            string ownername,
            List<int> activityDomains)
        {
            this.taxId = taxId;
            this.address = address;
            this.ownername = ownername;
            this.activityDomains = activityDomains;
        }

        internal Employer Clone()
        {
            return new Employer(taxId, address, ownername, new List<int>(activityDomains));
        }
    }
}