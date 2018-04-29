namespace Actor.Base
{
    public class OperatorPlusBehavior : CompositeBehavior<int>
    {
        private int accumulator;

        protected override void DoCompositeBehavior(IActor actor, int t)
        {
            inputs.Remove(actor);
            accumulator += t;
            if (inputs.Count == 0)
            {
                foreach (var item in outputs)
                {
                    item.SendMessage(accumulator);
                }
                outputs.Clear();
            }
        }
    }
}
