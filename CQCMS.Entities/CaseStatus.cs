using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities
{
    public enum CaseStatus : int
    {

        NewCase = 1,
        CaseClosed = 2,
        FirstEmail = 3,
        CaseNotAssigned = 4,
        CaseAssigned = 5,
        NewEmail = 6,
        CaseReopenedByBOLT = 7,
        CaseMultipleMatch = 8,
        AutoReply = 9,
        FileCase = 10
    }
}