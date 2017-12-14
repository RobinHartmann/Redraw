using NDesk.Options;
using System;

namespace Redraw
{
    public class OptionProcessor
    {
        private string optionName;
        private string optionValue;

        private bool processed = false;

        private bool IsSet
        {
            get
            {
                return !string.IsNullOrEmpty(optionValue);
            }
        }

        public OptionProcessor(string optionValue, string optionName)
        {
            this.optionName = optionName;
            this.optionValue = optionValue;
        }

        public OptionProcessor AssertIsSet()
        {
            AssertNotProcessed();

            if (!IsSet)
            {
                throw new OptionException("Option " + optionName + " is mandatory", optionName);
            }

            return this;
        }

        public OptionProcessor AddDefault(string defaultValue)
        {
            AssertNotProcessed();

            if (!IsSet)
            {
                optionValue = defaultValue;
            }

            return this;
        }

        public void Process(Action<string> optionAction)
        {
            AssertNotProcessed();

            optionAction(optionValue);
            processed = true;
        }

        public void ProcessAsInt(Action<int> optionAction)
        {
            AssertNotProcessed();

            int optionIntValue;

            if (!IsSet)
            {
                optionIntValue = -1;
            }
            else if (!int.TryParse(optionValue, out optionIntValue))
            {
                throw new OptionException("Option " + optionName + " must be a valid integer", optionName);
            }

            optionAction(optionIntValue);
            processed = true;
        }

        public void ProcessAsEnum<T>(Action<T> optionAction)
            where T : struct
        {
            AssertNotProcessed();

            T optionEnumValue;

            if (!IsSet)
            {
                optionEnumValue = default(T);
            }
            else if (!Enum.TryParse<T>(optionValue, out optionEnumValue))
            {
                throw new OptionException("Option " + optionName + " must be part of the enumeration '" + typeof(T).ToString() + "'", optionName);
            }

            optionAction(optionEnumValue);
            processed = true;
        }

        private void AssertNotProcessed()
        {
            if (processed)
            {
                throw new InvalidOperationException("This OptionProcessor has already processed the associated option and cannot be used anymore");
            }
        }
    }
}
