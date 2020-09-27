using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    class Environment
    {
        public Dictionary<string, ILabeled>                NamedItems { get; } = new Dictionary<string, ILabeled>();
        public Dictionary<(string Name, int ArgCount), Function> Functions { get; } = new Dictionary<(string Name, int ArgCount), Function>();

        public bool AddFunction(Function f)
        {
            return Functions.TryAdd(f.Id, f);
        }
        public bool AddFunctions(IEnumerable<Function> functions)
        {
            bool fullSuccess = true;
            foreach(Function f in functions)
            {
                if (!AddFunction(f)) { fullSuccess = false; }
            }
            return fullSuccess;
        }

        public bool AddLabeled(IEnumerable<ILabeled> labeled)
        {
            bool success = true;
            foreach(ILabeled l in labeled)
            {
                if (!NamedItems.TryAdd(l.Name, l)) { success = false; }
            }
            return success;
        }
        public bool AddLabeled(ILabeled labeled)
        {
            return NamedItems.TryAdd(labeled.Name, labeled);
        }

    }
}
