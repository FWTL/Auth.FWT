using System.Data.Entity;
using System.Diagnostics;

namespace Auth.FWT.Tests
{
    public class FakeContext : Data.AppContext
    {
        public FakeContext()
            : base("Server=(localdb)\\v11.0;Integrated Security=true; Initial Catalog=FakeAuth.FTW;Pooling=false")
        {
            Database.Connection.Close();
            Database.Delete();
            Database.SetInitializer(new FakeDBInitializer());
            Database.Log = q => Debug.Write(q);
        }
    }
}
