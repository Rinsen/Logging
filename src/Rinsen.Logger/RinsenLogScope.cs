using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rinsen.Logger
{
    public class RinsenLogScope
    {
        private readonly string _name;
        private readonly object _state;

        internal RinsenLogScope(string name, object state)
        {
            _name = name;
            _state = state;
        }

        public RinsenLogScope Parent { get; private set; }

        private static AsyncLocal<RinsenLogScope> _value = new AsyncLocal<RinsenLogScope>();
        public static RinsenLogScope Current
        {
            set
            {
                _value.Value = value;
            }
            get
            {
                return _value.Value;
            }
        }

        public static IDisposable Push(string name, object state)
        {
            var temp = Current;
            Current = new RinsenLogScope(name, state);
            Current.Parent = temp;

            return new DisposableScope();
        }

        public IEnumerable<KeyValuePair<string,object>> GetScopeKeyValuePairs()
        {
            if (_state is IEnumerable<KeyValuePair<string, object>> stateProperties)
            {
                return stateProperties;

            }

            return Enumerable.Empty<KeyValuePair<string, object>>();
        }

        public override string ToString()
        {
            return _state?.ToString();
        }

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }
    }
}
