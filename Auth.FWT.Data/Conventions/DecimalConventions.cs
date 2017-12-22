using System.Data.Entity.ModelConfiguration.Conventions;

namespace Auth.FWT.Data.Conventions
{
    public class DecimalConventions : Convention
    {
        public DecimalConventions()
        {
            this.Properties<decimal>().Where(x => x.Name == "Latitude").Configure(x => x.HasPrecision(9, 6));
            this.Properties<decimal>().Where(x => x.Name == "Longitude").Configure(x => x.HasPrecision(9, 6));
        }
    }
}
