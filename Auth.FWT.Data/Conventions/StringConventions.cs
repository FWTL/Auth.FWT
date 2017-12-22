using System.Data.Entity.ModelConfiguration.Conventions;

namespace Auth.FWT.Data.Conventions
{
    public class StringConventions : Convention
    {
        public StringConventions()
        {
            this.Properties<string>().Configure(x => x.HasMaxLength(80));
            this.Properties<string>().Where(x => x.Name == "Text").Configure(x => x.HasMaxLength(255));
            this.Properties<string>().Where(x => (x.Name.StartsWith("HTML") || x.Name.EndsWith("HTML")) || (x.Name.StartsWith("Description") || x.Name.EndsWith("Description")) || (x.Name.StartsWith("Tooltip") || x.Name.EndsWith("Tooltip")) || (x.Name.StartsWith("Note") || x.Name.EndsWith("Note"))).Configure(x => x.IsMaxLength());

            var email = this.Properties<string>().Where(x => (x.Name.StartsWith("Email") || x.Name.EndsWith("Email")));
            email.Configure(x => x.HasMaxLength(50));
            email.Configure(x => x.HasColumnType("varchar"));

            var phone = this.Properties<string>().Where(x => (x.Name.StartsWith("Phone") || x.Name.EndsWith("Phone")));
            phone.Configure(x => x.HasMaxLength(20));
            phone.Configure(x => x.HasColumnType("varchar"));

            var zipCode = this.Properties<string>().Where(x => x.Name == "PostalCode");
            zipCode.Configure(x => x.HasMaxLength(10));
            zipCode.Configure(x => x.HasColumnType("varchar"));

            var name = this.Properties<string>().Where(x => (x.Name.StartsWith("Name") || x.Name.EndsWith("Name")));
            name.Configure(x => x.IsRequired());

            var symbol = this.Properties<string>().Where(x => (x.Name.StartsWith("Symbol") || x.Name.EndsWith("Symbol")));
            symbol.Configure(x => x.HasMaxLength(4));
            symbol.Configure(x => x.IsRequired());
        }
    }
}
