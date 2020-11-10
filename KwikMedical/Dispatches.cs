using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KwikMedical
{
    class Dispatches
    {
        public string nhsID { get; set; }
        public string caseNumber { get; set; }
        public string patientAddress { get; set; }
        public string ambulance { get; set; }
        public string hosptial { get; set; }
        public string condition { get; set; }
        
        //sets the case to active so ambulance drivers know this still has to be treated
        public bool active { get; set; }

        //details for hospital to recieve from ambulance
        public string medicName { get; set; }
        public string timeOfArrival { get; set; }
        public string timeOfFinish { get; set; }
        public string helpGiven { get; set; }
        public bool onWayToHospital { get; set; }
    }
}
