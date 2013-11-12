using System;

namespace C24.TeamSharper
{
    public sealed class Change
    {
        private readonly string description;
        private readonly Action applyAction;

        public Change(string description, Action applyAction)
        {
            this.description = description;
            this.applyAction = applyAction;
        }

        public string Description
        {
            get { return this.description; }
        }

        public void Apply()
        {
            Action action = this.applyAction;
            if (action != null)
            {
                action();
            }
        }
    }
}
