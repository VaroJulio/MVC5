using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MVC5.Models
{
    public class VaroGalleryContext : DbContext
    {
        public VaroGalleryContext() : base("VaroGalleryEntities")
        {
            //Database.SetInitializer(new CreateDatabaseIfNotExists< VaroGalleryContext >());
        }

        public DbSet<Parametro> Parametros { get; set; }
    }
}