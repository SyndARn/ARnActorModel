namespace Actor.Base
{
    public class OperatorPlusBehavior : CompositeBehavior<int>
    {
        private int _accumulator;

        protected override void DoCompositeBehavior(IActor actor, int t)
        {
            inputs.Remove(actor);
            _accumulator += t;
            if (inputs.Count == 0)
            {
                foreach (IActor item in outputs)
                {
                    item.SendMessage(_accumulator);
                }

                outputs.Clear();
            }
        }
    }
}
