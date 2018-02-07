using System.Data.Entity;

namespace Auth.FWT.Tests
{
    public class FakeDBInitializer : DropCreateDatabaseAlways<FakeContext>
    {
        public FakeDBInitializer() : base()
        {
        }

        public override void InitializeDatabase(FakeContext context)
        {
            base.InitializeDatabase(context);
        }

        protected override void Seed(FakeContext context)
        {
            base.Seed(context);
        }
    }
}
