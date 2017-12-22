using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Auth.FWT.Data.Conventions
{
    public class IndexesConventions : Convention
    {
        public IndexesConventions()
        {
            this.Properties<bool>().Where(x => x.Name == "IsDeleted").Configure(x => x.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Deleted"))));
        }
    }
}
