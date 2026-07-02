using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManager.Models
{
    public class QueueSettings
    {
        public int Id { get; set; }

        public SchedulingAlgorithm SelectedAlgorithm { get; set; }
            = SchedulingAlgorithm.FIFO;
    }
}