using ParsingTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.theUniverse.commands
{
    public abstract class Command
    {
        public class CommandName : IComparable<CommandName>, IEquatable<CommandName>
        {
            public string Value { get; }
            public CommandName(string value)
            {
                this.Value = value.ToLower();
            }
            public int Length => Value.Length;
            public static implicit operator CommandName(string s) => new CommandName(s);
            public static implicit operator string(CommandName cn) => cn.Value;
            public int CompareTo(CommandName other) => Value?.CompareTo(other?.Value) ?? other?.Value?.CompareTo(Value) ?? 0;
            public bool Equals(CommandName other) => Value?.Equals(other?.Value) ?? other?.Value?.Equals(Value) ?? true;
            public override bool Equals(object obj) => obj switch {
                    CommandName cn => Equals(cn),
                    string s => Equals(new CommandName(s)),
                    _ => false
                };
            
            public override int GetHashCode() => Value.GetHashCode();
            public static bool operator ==(CommandName a, CommandName b) => a.Equals(b);
            public static bool operator !=(CommandName a, CommandName b) => !a.Equals(b);
            public override string ToString() => Value;
        }
        public abstract CommandName Name { get; }
        public abstract string Definition { get; }
        public virtual string Manual { get => Definition; }
        public string Execute(string args, Universe universe)
        {
            return exec(args, universe);
        }
        protected abstract string exec(string args, Universe universe);
        protected Tokenizer argSplitter { get; } = new Tokenizer(new string[0], StringProps.Barriers, new string[0], new string[0], new string[0], new string[0], StringProps.Barriers, new string[0]) { BreakOnWhiteSpace = true, IncludeEmpty = false };

        public static readonly string defName = "def";
    }
}
