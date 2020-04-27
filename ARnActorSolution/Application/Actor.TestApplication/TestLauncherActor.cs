using Actor.Base;

namespace Actor.TestApplication
{
    public class TestLauncherActor : ActionActor
    {
        private readonly Future<bool> future = new Future<bool>();
        public TestLauncherActor()
            : base()
        {
        }

        public void Finish()
        {
            future.SendMessage(true);
        }

        public bool Wait()
        {
            return future.Result();
        }
    }
}
