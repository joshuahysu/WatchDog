namespace WatchDog
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ServerInformations
    {
        public Guid ID { get; set; }

        [StringLength(12)]
        public string ServerName { get; set; }

        public bool DataDirty { get; set; }

        public DateTime? LastReport { get; set; }
    }
}
