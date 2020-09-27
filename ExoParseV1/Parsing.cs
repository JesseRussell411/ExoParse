//#define multithreadingTokenize


using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ExoParsev1
{
    public class Parser
    {

        private class Item
        {
            // This is what you do when you're tired of using tuples
            public override string ToString()
            {
                if (!TextOnly && (text ?? "") == "")
                {
                    if (IsOp) { return op.ToString(); }
                    else if (IsBm) { return bm.ToString(); }
                    else if (IsAm) { return am.ToString(); }
                    else if (IsVal) { return val.ToString(); }
                    else if (IsParens) { return $"({subItems?.Select(si => si.ToString()).Aggregate((c, si) => $"{c}, {si}") ?? ""})"; }
                    else { return ""; }
                }

                return text ?? "";
            }
            public Item(string text, Operator op, Modifier bm, Modifier am, Valuable val)
            {
                this.text = text;
                this.op = op;
                this.bm = bm;
                this.am = am;
                this.val = val;
            }
            public Item()
                : this(null, null, null, null, null) { }

            public Item(string text, Operator op) : this(text, op, null, null, null) { }
            public Item(string text, Modifier bm, Modifier am) : this(text, null, bm, am, null) { }
            public Item(string text, Valuable val) : this(text, null, null, null, val) { }
            public Item(Item[] subItems) : this(subItems.ToList()) { }
            public Item(List<Item> subItems) : this() { this.subItems = subItems; }

            public List<Item> SubItems
            {
                get { return subItems != null ? subItems : subItems = new List<Item>(); }
                set { subItems = value; }
            }

            private List<Item> subItems = null;

            public Operator op = null;
            public Modifier bm = null;
            public Modifier am = null;
            public Valuable val = null;
            public string text = null;

            public bool IsOp { get { return op != null && !IsParens; } }
            public bool IsBm { get { return bm != null && !IsParens; } }
            public bool IsAm { get { return am != null && !IsParens; } }
            public bool IsVal { get { return val != null && !IsParens; } }
            public bool IsSymbol { get { return IsOp || IsBm || IsAm; } }
            public bool IsParens { get { return subItems?.Any() ?? false; } }
            public bool IsElement { get { return IsVal || IsParens; } }
            public bool Empty { get { return text == null && !SubItems.Any(); } }
            public bool TextOnly { get { return !(IsSymbol || IsParens || IsElement) && text != null; } }

            public void SetOpOnly()
            {
                bm = null;
                am = null;
                val = null;
                SubItems = null;
            }
            public void SetBmOnly()
            {
                op = null;
                am = null;
                val = null;
                SubItems = null;
            }
            public void SetAmOnly()
            {
                op = null;
                bm = null;
                val = null;
                SubItems = null;
            }
            public void SetElementOnly()
            {
                op = null;
                bm = null;
                am = null;
            }
            public void SetIsParentOnly()
            {
                op = null;
                am = null;
                val = null;
            }
        }
        public Parser()
        {

        }
        public Operator DefaultOperator { get; set; } = new Multiplication();

        #region operators
        public Operator[] Operators
        {
            get
            {
                return operators.Values.ToArray();
            }
            set
            {
                Operators_Clear();
                Operators_Add(value);
            }
        }
        private Dictionary<string, Operator> operators = new Dictionary<string, Operator>();
        public string[] Operators_Symbols
        {
            get
            {
                return operators.Keys.ToArray();
            }
        }
        public Operator Operators_Get(string symbol)
        {
            return operators[symbol];
        }
        public bool Operators_TryGet(string symbol, out Operator found)
        {
            return operators.TryGetValue(symbol, out found);
        }
        public void Operators_Add(Operator item)
        {
            operators.Add(item.Symbol, item);
            PriorityIndex_Add(item);
        }
        public void Operators_Add(Operator[] items)
        {
            foreach (Operator item in items)
            {
                operators.Add(item.Symbol, item);
            }
            PriorityIndex_Add(items);
        }
        public bool Operators_Remove(string symbol)
        {
            Operator op;
            if (operators.Remove(symbol, out op))
            {
                PriorityIndex_Remove(op);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Operators_Clear()
        {
            operators.Clear();
            operatorPriorityIndex.Clear();
        }
        #endregion

        #region Modifiers
        public string[] Modifiers_Symbols { get { return modifiers.Keys.Select(k => k.symbol).ToArray(); } }
        public Modifier[] Modifiers
        {
            get
            {
                return modifiers.Values.ToArray();
            }
            set
            {
                Modifiers_Clear();
                Modifiers_Add(value);
            }
        }
        private Dictionary<(string symbol, Orientation orientation), Modifier> modifiers = new Dictionary<(string symbol, Orientation orientation), Modifier>();

        public void Modifiers_Add(Modifier mod)
        {
            modifiers.Add((mod.Symbol, mod.Orientation), mod);
            PriorityIndex_Add(mod);
        }
        public void Modifiers_Add(Modifier[] mods)
        {
            foreach (Modifier mod in mods)
            {
                Modifiers_Add(mod);
            }
        }
        public bool Modifiers_Remove((string symbol, Orientation orientation) id)
        {
            Modifier mod;
            if (modifiers.Remove(id, out mod))
            {
                PriorityIndex_Remove(mod);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Modifiers_Clear()
        {
            modifiers.Clear();
            modifierPriorityIndex.Clear();
        }
        #endregion

        #region priority index
        //--------------
        public void BuildPrioriyIndex()
        {
            operatorPriorityIndex.Clear();
            modifierPriorityIndex.Clear();
            PriorityIndex_Add(operators.Values.ToArray());
            PriorityIndex_Add(modifiers.Values.ToArray());
        }

        #region operator
        private Dictionary<int, Dictionary<string, Operator>> operatorPriorityIndex { get; } = new Dictionary<int, Dictionary<string, Operator>>();
        public void PriorityIndex_Add(Operator op)
        {
            if (operatorPriorityIndex.ContainsKey(op.Priority))
            {
                operatorPriorityIndex[op.Priority].Add(op.Symbol, op);
            }
            else
            {
                operatorPriorityIndex.Add(op.Priority, new Dictionary<string, Operator>() { { op.Symbol, op } });
            }
        }
        public void PriorityIndex_Add(Operator[] ops)
        {
            foreach (Operator op in ops)
            {
                PriorityIndex_Add(op);
            }
        }
        public void PriorityIndex_Remove(Operator op)
        {
            operatorPriorityIndex[op.Priority].Remove(op.Symbol);
        }
        public void PriorityIndex_Remove(Operator[] ops)
        {
            foreach (Operator op in ops)
            {
                PriorityIndex_Remove(op);
            }
        }
        #endregion

        #region modifier
        private Dictionary<int, Dictionary<(string symbol, Orientation orientation), Modifier>> modifierPriorityIndex { get; } = new Dictionary<int, Dictionary<(string symbol, Orientation orientation), Modifier>>();
        public void PriorityIndex_Add(Modifier mod)
        {
            if (modifierPriorityIndex.ContainsKey(mod.Priority))
            {
                modifierPriorityIndex[mod.Priority].Add(mod.Id, mod);
            }
            else
            {
                modifierPriorityIndex.Add(mod.Priority, new Dictionary<(string symbol, Orientation orientation), Modifier>() { { mod.Id, mod } });
            }
        }
        public void PriorityIndex_Add(Modifier[] mods)
        {
            foreach (Modifier mod in mods)
            {
                PriorityIndex_Add(mod);
            }
        }
        public void PriorityIndex_Remove(Modifier mod)
        {
            modifierPriorityIndex[mod.Priority].Remove(mod.Id);
        }
        public void PriorityIndex_Remove(Modifier[] mods)
        {
            foreach (Modifier mod in mods)
            {
                PriorityIndex_Remove(mod);
            }
        }
        #endregion

        //--------------
        #endregion
        public string OpenParenthesis { get; set; } = "(";
        public string CloseParenthesis { get; set; } = ")";
        public Dictionary<string, Variable> VariableContainer { get; set; }
        public Dictionary<(string name, int argCount), Function> FunctionContainer { get; set; }
        private string[] symbols
        {
            get
            {
                return operators.Keys.Union(modifiers.Keys.Select(m => m.symbol)).ToArray();
            }
        }

        #region Itemize
        private List<Item> Itemize(string statement, Valuable starterElement = null, Dictionary<string, Variable> localVariableContainer = null)
        {
            // This method normaly expects tokens but we can take care of that.

            // Return nothing if the statement is null or empty.
            if ((statement ?? "") == "") { return new List<Item>(); }

            //// Remove whitespace.
            //bool firstIsWhiteSpace = statement.Length > 0 ? char.IsWhiteSpace(statement.First()) : false;
            bool firstIsWhiteSpace;

            if (firstIsWhiteSpace = char.IsWhiteSpace(statement.First()) || char.IsWhiteSpace(statement.Last()))
            {
                statement = statement.Trim();    // Don't worry, string is immutable and stored by value not reference, this won't mess anything up externaly. If I used String instead of string it would.
            }

            // If the statement is empty, return nothing.
            if (statement.Length == 0) { return new List<Item>(); }

            //// Take care of parenthesis.
            //if (statement.IsWrapped(OpenParenthesis, CloseParenthesis))
            //{
            //    return new Item(Itemize(statement.UnWrap(OpenParenthesis, CloseParenthesis))).Ta();
            //}

            // To start itemizing, I'll tokenize the statement.
            string[] tokens = statement.Tokenize(symbols, OpenParenthesis.Ta(), CloseParenthesis.Ta(), new string[0], new string[0], CloseParenthesis.Ta(), true, true);

            // Now we can really get to business.
            return Itemize(tokens, firstIsWhiteSpace ? null : starterElement, localVariableContainer);
        }
        private List<Item> Itemize(string[] tokens, Valuable starterElement = null, Dictionary<string, Variable> localVariableContainer = null)
        {
            // If there are no tokens then return nothing.
            if (tokens.Length == 0) { return new Item[0].ToList(); }

            // Itemize the tokens
            List<Item> items = new List<Item>();
            foreach (string token in tokens)
            {
                // Take care of Parens.
                if (token.IsWrapped(OpenParenthesis, CloseParenthesis))
                {
                    items.Add(new Item(Itemize(token.UnWrap(OpenParenthesis, CloseParenthesis), null, localVariableContainer)));
                }
                else
                {
                    Operator op = operators.GetValueOrDefault(token);
                    Modifier bm = modifiers.GetValueOrDefault((token, Orientation.Before));
                    Modifier am = modifiers.GetValueOrDefault((token, Orientation.After));
                    Valuable val = ((object)op ?? (object)bm ?? (object)am) != null ? null : ParseValuableElement(token, localVariableContainer);
                    items.Add(new Item(token, op, bm, am, val));
                }
            }

            // Adding the starter element if necesary
            if (items.Any() && items[0].IsOp && starterElement != null)
            {
                items.Insert(0, new Item(starterElement.ToString(), starterElement));
            }

            Item item;
            Item prev;
            Item next;
            // Taking care of confusions
            for (int i = 0; i < items.Count; i++)
            {
                item = items[i];
                next = i == items.Count - 1 ? null : items[i + 1];
                prev = i == 0 ? null : items[i - 1];

                // Dealing with operator/modifier confusion.
                // Some modifiers and operators have the same symbol. Prime example is "-". It can be MINUS or NEGATIVE.

                if (item.IsBm)
                {
                    if (prev?.IsSymbol ?? true)
                    {
                        item.SetBmOnly();
                    }
                }

                if (item.IsAm)
                {
                    if (next?.IsSymbol ?? true)
                    {
                        item.SetAmOnly();
                    }
                }

                if (item.IsOp) { item.SetOpOnly(); }

                if (item.IsBm && item.IsAm)
                {
                    item.SetBmOnly();
                }

                if (DefaultOperator != null && item.IsElement && (prev?.IsElement ?? false))
                {
                    items.Insert(i, new Item(DefaultOperator.ToString(), DefaultOperator));
                    i++;
                }
            }
            return items;
        }
        /// <summary>
        /// Inserts default oeprator into item list in-place.
        /// Only to be run after GroupModifiers and before GroupOperators
        /// </summary>
        /// <param name="items"></param>
        private void InsertDefaultOp(List<Item> items)
        {
            if (DefaultOperator == null) { return; }
            // insert default operator where necesary.
            Item item;
            Item next;
            Item prev;
            for (int i = 0; i < items.Count; i++)
            {
                item = items[i];
                next = i == items.Count - 1 ? null : items[i + 1];
                prev = i == 0 ? null : items[i - 1];

                if (item?.IsParens ?? false)
                {
                    InsertDefaultOp(item.SubItems);
                }

                if (item.IsElement && (prev?.IsElement ?? false))
                {
                    items.Insert(i, new Item(DefaultOperator.Symbol, DefaultOperator));
                    i++;
                }
            }
        }
        private List<Item> GroupModifiers(List<Item> items, int? modifierMinimumPriority = null)
        {
            bool hasBMods = false;
            bool hasAMods = false;
            if (modifierMinimumPriority == null)
            {
                foreach (Item item in items)
                {
                    if (item.IsBm)
                    {
                        hasBMods = true;
                        modifierMinimumPriority = modifierMinimumPriority == null ? item.bm.Priority : (int?)Math.Min(item.bm.Priority, (int)modifierMinimumPriority);
                    }
                    if (item.IsAm)
                    {
                        hasAMods = true;
                    }
                }
            }


            if (hasBMods || hasAMods)
            {
                List<Item> itemList = new List<Item>();
                if (hasBMods)
                {
                    List<Item> cache = new List<Item>();

                    Item mod = null;

                    Item item = null;
                    Item next = null;
                    Item prev = null;
                    bool dontadd;
                    for (int i = 0; i < items.Count; i++)
                    {
                        dontadd = false;
                        item = items[i];
                        next = i == items.Count - 1 ? null : items[i + 1];
                        prev = i == 0 ? null : items[i - 1];


                        if (item.IsBm && mod == null)
                        {
                            if (item.bm.Priority == modifierMinimumPriority)
                            {
                                if (mod != null)
                                {
                                    List<Item> sl = mod.Tl();
                                    sl.Add(new Item(GroupModifiers(cache)));
                                    itemList.Add(new Item(sl));
                                    //itemList.Add(new Item(GroupModifiers(cache.ToArray())));
                                    //itemList.Add(new Item(mod.Toa().Concat(GroupModifiers(cache.ToArray())).ToArray()));
                                }
                                else
                                {
                                    itemList.AddRange(cache);
                                }
                                cache.Clear();
                                mod = item;
                                dontadd = true;
                            }
                        }
                        else if (item.IsOp && mod != null)
                        {
                            if (item.op.Priority < modifierMinimumPriority/* || (next.IsBm && next.bm.Priority <= modifierMinimumPriority)*/)
                            {
                                if (mod != null)
                                {
                                    //itemList.Add(mod);
                                    //itemList.Add(new Item(GroupModifiers(cache.ToArray())));
                                    List<Item> sl = mod.Tl();
                                    sl.Add(new Item(GroupModifiers(cache)));
                                    itemList.Add(new Item(sl));

                                    //itemList.Add(new Item(mod.Ta().Concat(new Item(GroupModifiers(cache.ToList())).Tl()).ToList()));
                                }
                                else
                                {
                                    itemList.AddRange(GroupModifiers(cache));
                                }
                                cache.Clear();
                                mod = null;
                                itemList.Add(item);
                                dontadd = true;
                            }
                        }
                        if (!dontadd)
                        {
                            cache.Add(item);
                        }
                    }

                    if (mod != null)
                    {
                        //itemList.Add(mod);
                        //itemList.Add(new Item(GroupModifiers(cache.ToArray())));
                        List<Item> sl = mod.Tl();
                        sl.Add(new Item(GroupModifiers(cache)));
                        itemList.Add(new Item(sl));

                        //itemList.Add(new Item(mod.Tl().Concat(new Item(GroupModifiers(cache.ToList())).Tl()).ToList()));
                    }
                    else
                    {
                        itemList.AddRange(GroupModifiers(cache));
                    }
                }
                else
                {
                    itemList.AddRange(items);
                }

                if (hasAMods)
                {
                    //group after-oriented modifiers

                    Item prev = null;
                    Item item = null;

                    int itemList_Count = itemList.Count;
                    for (int i = 0; i < itemList_Count; i++)
                    {
                        item = itemList[i];
                        prev = i == 0 ? null : itemList[i - 1];
                        if (item.IsAm)
                        {
                            if (i != 0 && !(prev?.IsSymbol ?? false))
                            {
                                itemList[i - 1] = new Item(new Item[] { prev, item });
                                itemList.RemoveAt(i);
                                itemList_Count = itemList.Count;
                                i--;
                            }
                            else
                            {
                                itemList[i] = new Item(new Item[] { new Item(), item });
                            }
                        }
                    }
                }

                return itemList;
            }
            else
            {
                return items.ToList(); ;
            }
        }
        private List<Item> GroupOperators(List<Item> items, int? operatorMinimumPriority = null)
        {
            if (operatorMinimumPriority == null)
            {
                foreach (Item item in items)
                {
                    if (item.op != null)
                    {
                        operatorMinimumPriority = operatorMinimumPriority == null ? item.op.Priority : (int?)Math.Min(item.op.Priority, (int)operatorMinimumPriority);
                    }
                }
            }

            if (operatorMinimumPriority == null)
            {
                foreach (Item item in items)
                {
                    if (item.IsParens)
                    {
                        item.SubItems = GroupOperators(item.SubItems);
                    }
                }
                return items;
            }

            int? lowestPriorityInCache = null;
            List<Item> groupedItems = new List<Item>();
            List<Item> cache = new List<Item>();
            Item op = null;
            void breakstuff()
            {
                if (op != null)
                {
                    groupedItems.Add(op);
                    op = null;
                }

                if (cache.Count == 1 && cache[0].IsParens)
                {
                    cache[0].SubItems = GroupOperators(cache[0].SubItems);
                }

                if (cache.Count > 0)
                {

                    groupedItems.Add(
                        cache.Count == 1 ? cache[0] : new Item(GroupOperators(cache.ToList(), lowestPriorityInCache)));
                }
            }
            foreach (Item item in items)
            {
                if (item.op != null && item.op.Priority == operatorMinimumPriority)
                {
                    if (item.op.Priority == operatorMinimumPriority)
                    {
                        breakstuff();
                        op = item;
                        cache.Clear();
                    }
                    else
                    {
                        lowestPriorityInCache = item.op.Priority < lowestPriorityInCache ? item.op.Priority : lowestPriorityInCache;
                        cache.Add(item);
                    }
                }
                else
                {
                    cache.Add(item);
                }
            }
            breakstuff();
            return groupedItems;
        }
        private Valuable ConvertItemsToExpression(List<Item> items)
        {
            if (items.Count == 0) { return null; }
            if (items.Count == 1)
            {
                if (items[0].IsParens) { return ConvertItemsToExpression(items[0].SubItems); }
                else if (items[0].IsVal) { return items[0].val; }
                else if (items[0].IsOp)
                {
                    Expression ex = new Expression();
                    ex.AddItem((Valuable)null);
                    ex.AddItem(items[0].op, null);
                    return ex;
                }
                else if (items[0].IsBm) { return new ValuableModifier(items[0].bm, null); }
                else if (items[0].IsAm) { return new ValuableModifier(items[0].am, null); }
                else
                {
                    return null;
                }
            }

            Expression exp = new Expression();
            Item item = null;
            Item prev = null;
            Item next = null;
            int li = items.Count - 1;
            for (int i = 0; i < items.Count; i++)
            {
                item = items[i];
                next = i == items.Count - 1 ? null : items[i + 1];
                prev = i == 0 ? null : items[i - 1];
                //
                if (item.IsOp)
                {
                    // Insert void element if no element exists in the space where there should be one. ex: 8**8 -> 8 * void * 8, 8* -> 8*void
                    if (next == null || !next.IsElement)
                    {
                        exp.AddItem(item.op, null);
                    }
                }
                else if (item.IsBm)
                {
                    Valuable val = null;
                    if (next != null)
                    {
                        val = next.IsParens ? ConvertItemsToExpression(next.SubItems) : next.val;
                    }

                    return new ValuableModifier(item.bm, val);
                }
                else if (item.IsAm)
                {
                    Valuable val = null;
                    if (prev != null)
                    {
                        val = prev.IsParens ? ConvertItemsToExpression(prev.SubItems) : prev.val;
                    }
                    return new ValuableModifier(item.am, val);
                }
                else if (item.IsVal)
                {
                    if (prev?.IsOp ?? false)
                    {
                        if (exp.Any())
                        {
                            exp.AddItem(prev.op, item.val);
                        }
                        else
                        {
                            // Insert void element if no element exists in the space where there should be one. ex: *8 -> void * 8
                            exp.AddItem(null, null);
                            exp.AddItem(prev.op, item.val);
                        }
                    }
                    else if (prev == null)
                    {
                        exp.AddItem(item.val);
                    }
                }
                else if (item.IsParens)
                {
                    if (prev?.IsOp ?? false)
                    {
                        exp.AddItem(prev.op, ConvertItemsToExpression(item.SubItems));
                    }
                    else if (prev == null)
                    {
                        exp.AddItem(ConvertItemsToExpression(item.SubItems));
                    }
                }

                //
            }

            return exp;
        }
        #endregion
        private Valuable ParseValuableElement(string elementString, Dictionary<string, Variable> localVariableContainer = null)
        {
            // Give up if the string is null.
            if (elementString == null) { return null; }

            // Ok first things first, get rid of whitespace
            elementString = elementString.Trim(); // Don't worry java users, string is a struct so it's store by value not reference, this won't break anything.

            // Give up if the string is empty.
            if (elementString.Length == 0) { return null; }

            // Now for the simplest possible element. Does the string represent a number?
            double num;
            (string name, string[] args) partParsed;
            Function func;
            if (double.TryParse(elementString, out num))
            {
                return new constant(num);
            }
            else if (elementString.IsLable()) // Is it a variable? If it's a valid label then it is.
            {
                Dictionary<string, Variable> varCan = localVariableContainer ?? VariableContainer;

                Variable var;
                if (varCan.TryGetValue(elementString, out var))
                {
                    return var;
                }
                else
                {
                    varCan.Add(elementString, var = new Variable(elementString));
                    return var;
                }
            }
            else if (elementString.IsFunction(OpenParenthesis, CloseParenthesis, out partParsed)
                && FunctionContainer.TryGetValue((partParsed.name, partParsed.args.Length), out func)) // Is it a function
            {
                return new FunctionHolder(func, partParsed.args.Select(a => ParseExpression(a)).ToArray());
            }
            else
            {
                throw new MessageException($"Unkown Element: {elementString}");
            }
        }
        public Valuable ParseExpression(string statement, Valuable starterElement = null, Dictionary<string, Variable> localVariableContainer = null)
        {
            List<Item> items = Itemize(statement, starterElement, localVariableContainer);

            items = GroupModifiers(items);

            // insert default operator where necesary.
            InsertDefaultOp(items);

            items = GroupOperators(items);

            Valuable exp = ConvertItemsToExpression(items);

            return exp;
        }
    }
    public class UnequalParenArrays : Exception
    {
        public UnequalParenArrays() : base() { }

        public UnequalParenArrays(string message) : base(message) { }
    }
    public static class ParseUtils
    {
        public static string[] Tokenize(this string text, string[] separators, bool trimWhitespace = true, bool breakOnWhiteSpace = false)
        { return text.Tokenize(separators, new string[0], new string[0], trimWhitespace, breakOnWhiteSpace); }
        public static string[] Tokenize(this string text, string[] separators, string[] openParens, string[] closeParens, bool trimWhitespace = true, bool breakOnWhiteSpace = false)
        { return text.Tokenize(separators, openParens, closeParens, new string[0], trimWhitespace, breakOnWhiteSpace); }
        public static string[] Tokenize(this string text, string[] separators, string[] openParens, string[] closeParens, string[] quotes, bool trimwhitespace = true, bool breakOnWhiteSpace = false)
        { return text.Tokenize(separators, openParens, closeParens, quotes, new string[0], new string[0], trimwhitespace, breakOnWhiteSpace); }
        public static string[] Tokenize(this string text, string[] separators, string[] openParens, string[] closeParens, string[] quotes, string[] preBreaks, string[] postBreaks, bool trimwhitespace = true, bool breakOnWhiteSpace = false)
        { return text.Tokenize(separators, openParens, closeParens, quotes, preBreaks, postBreaks, new string[0], trimwhitespace, breakOnWhiteSpace); }
        public static string[] Tokenize(this string text, string[] separators, string[] openParens, string[] closeParens, string[] quotes, string[] preBreaks, string[] postBreaks, string[] delims, bool trimWhitespace = true, bool breakOnWhiteSpace = false)
        {
            if (openParens.Length != closeParens.Length)
            {
                throw new UnequalParenArrays("The arrays: openParens and closeParens must be the same length so that every openParen has a corresponding closeParen.");
            }

            int[] depth = new int[openParens.Length];
            bool[] inQuotes = new bool[quotes.Length];

            int text_lastIndex = text.Length - 1;
            SymbolFinder symbolFinder = new SymbolFinder(separators.Union(openParens).Union(closeParens).Union(quotes).Union(preBreaks).Union(postBreaks).ToArray());
            StringBuilder token = new StringBuilder();
            List<string> tokens = new List<string>();
            char c;
            char? next;
            string symb;
            bool freeToBreak;
            for (int i = 0; i < text.Length; i++)
            {
                freeToBreak = true;

                c = text[i];
                next = i == text_lastIndex ? null : (char?)text[i + 1];

                symb = symbolFinder.Find(c, next);
                token.Append(c);

                // Handle parethesis.
                for (int j = 0; j < openParens.Length; j++)
                {
                    if (symb == openParens[j]) { depth[j]++; }
                    if (symb == closeParens[j]) { depth[j]--; }
                    if (depth[j] != 0) { freeToBreak = false; }
                }

                // Handle quotes.
                for (int j = 0; j < quotes.Length; j++)
                {
                    if (symb == quotes[j]) { inQuotes[j] = !inQuotes[j]; }
                    if (inQuotes[j]) { freeToBreak = false; }
                }

                if (freeToBreak)
                {
                    if (separators.Contains(symb))
                    {
                        // Remove the separator from the text to be added to the array.
                        token.Remove(token.Length - symb.Length, symb.Length);

                        // Trim text if the switch is on.
                        if (trimWhitespace)
                        {
                            string s = token.ToString();
                            token.Clear();
                            token.Append(s.Trim());
                        }

                        // Add the text to the array unless the text is empty.
                        if (token.Length > 0) { tokens.Add(token.ToString()); }

                        // Add the separator.
                        tokens.Add(symb);

                        // Clear the stringBuilder for the next cycle.
                        token.Clear();
                    }
                    else if (preBreaks.Contains(symb))
                    {
                        // Remove the separator from the text to be added to the array.
                        token.Remove(token.Length - symb.Length, symb.Length);

                        // Trim text if the switch is on.
                        if (trimWhitespace)
                        {
                            string s = token.ToString();
                            token.Clear();
                            token.Append(s.Trim());
                        }

                        // Add the text to the array unless the text is empty.
                        if (token.Length > 0) { tokens.Add(token.ToString()); }

                        // Clear the stringBuilder for the next cycle.
                        token.Clear();

                        // Add the preBreak seperator to the next cycle.
                        token.Append(symb);
                    }
                    else if (postBreaks.Contains(symb))
                    {

                        // Trim text if the switch is on.
                        if (trimWhitespace)
                        {
                            string s = token.ToString();
                            token.Clear();
                            token.Append(s.Trim());
                        }

                        // Add the text to the array unless the text is empty.
                        if (token.Length > 0) { tokens.Add(token.ToString()); }

                        // Clear the stringBuilder for the next cycle.
                        token.Clear();
                    }
                    else if (delims.Contains(symb))
                    {
                        // Remove the separator from the text to be added to the array.
                        token.Remove(token.Length - symb.Length, symb.Length);

                        // Trim text if the switch is on.
                        if (trimWhitespace)
                        {
                            string s = token.ToString();
                            token.Clear();
                            token.Append(s.Trim());
                        }

                        // Add the text to the array unless the text is empty.
                        if (token.Length > 0) { tokens.Add(token.ToString()); }

                        // Clear the stringBuilder for the next cycle.
                        token.Clear();
                    }
                    else if (breakOnWhiteSpace && char.IsWhiteSpace(c))
                    {
                        // Remove the single whitespace character from the stringBuilder.
                        token.Remove(token.Length - 1, 1);

                        // Trim text if the switch is on.
                        if (trimWhitespace)
                        {
                            string s = token.ToString();
                            token.Clear();
                            token.Append(s.Trim());
                        }

                        // Add the text to the array unless the text is empty.
                        if (token.Length > 0) { tokens.Add(token.ToString()); }

                        // Clear the stringBuilder for the next cycle.
                        token.Clear();

                        // Add the single whitespace character to the next cycle.
                        token.Append(c);
                    }
                }
            }
            // Trim text if the switch is on.
            if (trimWhitespace)
            {
                string s = token.ToString();
                token.Clear();
                token.Append(s.Trim());
            }
            // Clearing out the rest of the text.
            if (token.Length > 0) { tokens.Add(token.ToString()); }

            // We're done.
            return tokens.ToArray();
        }
        public static bool TryParseFunctionId(string text, out (string name, int argCount)? parsedFunctionId)
        {

            //structure of functionId: "{name}[{number of args}]" <-- *remeber brackets
            StringBuilder nameBuilder = new StringBuilder();
            StringBuilder argsBuilder = new StringBuilder();
            int openIndex = 0;
            int closeIndex = 0;
            bool isFunctionId = false;
            string name = "";
            int argCount = 0;

            bool fillNameNotArgs = true;
            //foreach (char thing in text)
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '[')
                {
                    if (fillNameNotArgs)
                    {
                        openIndex = i;
                        fillNameNotArgs = false;
                    }
                }

                if (fillNameNotArgs)
                {
                    nameBuilder.Append(text[i]);
                }
                else
                {
                    argsBuilder.Append(text[i]);
                }
            }

            name = nameBuilder.ToString();
            string args = argsBuilder.ToString();

            if (name.IsLable() & args.IsWrapped("[", "]"))
            {
                // This is the known structure
                // so far: "{name}[{ ??? }]"
                //                ^   ^   ^
                //                |   |   |
                //                |   |   the last character is definitely the closing bracket, so closeIndex is equal to the last index in text
                //                |   What we don't know is whether the stuff inside the square brackets is a number
                //                The open index is equal to this character here, the first square bracket

                closeIndex = text.Length - 1;

                if (int.TryParse(text[(openIndex + 1)..closeIndex], out argCount))
                {
                    isFunctionId = argCount >= 0;
                }
                else { isFunctionId = false; }
            }
            else { isFunctionId = false; }

            if (isFunctionId)
            {
                parsedFunctionId = (name: name, argCount: argCount);
                return true;
            }
            else
            {
                parsedFunctionId = null;
                return false;
            }
        }
        public static bool IsFunction(this string text, string openParen, string closeParen, out (string name, string[] args) partParsed)
        {
            //structure of function: "{a-b,A-B,_, 0-9, .}({ARGS deliminated by commas})" <-- remeber parathesis
            SymbolFinder tnzr = new SymbolFinder(new string[] { openParen, closeParen });
            StringBuilder nameBuilder = new StringBuilder();
            StringBuilder argsBuilder = new StringBuilder();

            bool fillNameNotArgs = true;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                char? next = i == text.Length - 1 ? null : (char?)text[i + 1];
                string smb = tnzr.Find(c, next);

                if (smb == openParen)
                {
                    fillNameNotArgs = false;
                }

                if (fillNameNotArgs)
                {
                    nameBuilder.Append(c);
                }
                else
                {
                    argsBuilder.Append(c);
                }
            }

            string name = nameBuilder.ToString();
            string args = argsBuilder.ToString();

            if (name.IsLable() & args.IsWrapped(openParen, closeParen))
            {
                partParsed = (
                    name: name,
                    args: args.UnWrap(openParen, closeParen).SmartSplit(openParen, closeParen, ",").Select(a => a.Trim()).ToArray()
                    );
                return true;
            }
            else
            {
                partParsed = (null, null);
                return false;
            }
        }
        public static bool IsVariable(this string text)
        {
            return text.IsLable();
        }
        public static bool IsLable(this string text)
        {
            //structure of varaible: "{a-b,A-B,_, 0-9,.}"

            // lables can have: uppercase/lowercase letters, underscore(_), period(.), and numbers but they have to contain at least one non-number character.
            // They can also have spaces but they cannot start or end with a space or any other whitespace characters

            //also it must have at least one character: text.Length >= 1
            #region this bit does stuff to, even though it's before declarations
            if (!(text.Length >= 1))
            {
                return false; //--(FAIL)--
            }
            else if (text.Trim().Length != text.Length)
            {
                //test failed --(FAIL)--
                return false;

                //the text starts or ends with some sort of whitespace which is not allowd
            }
            #endregion

            string a_b = "abcdefghijklmnopqrstuvwxyz";
            string A_B = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string _0_9 = "0123456789";
            string _ = "_";
            string space = " ";
            string dot = ".";

            bool contains_non_NumberOrSpace = false;

            {
                //bool first = true;
                foreach (char c in text)
                {
                    bool is_number = _0_9.Contains(c);
                    bool is_space = space.Contains(c);
                    if ((a_b.Contains(c))
                        || (A_B.Contains(c))
                        || (is_number)
                        //| (_0_9.Contains(thing) & !first)
                        || (_.Contains(c))
                        //|| (is_space)
                        || (dot.Contains(c)))

                    {
                        //test passed, so far --(PASS?)--
                    }
                    else
                    {
                        //test failed --(FAIL)--
                        return false;
                    }

                    if (!(is_number | is_space)) { contains_non_NumberOrSpace = true; }

                    //first = false;
                }
            }
            //test passed so far --(PASS?)--

            if (!contains_non_NumberOrSpace)
            {
                //test failed --(FAIL)--
                return false;
            }
            else
            {
                //test passed --(PASS)--
                return true;
            }
        }
        public static string UnWrap(this string text, string openBracket, string closeBracket)
        {
            if (text.IsWrapped(openBracket, closeBracket))
            {
                return text.Substring(openBracket.Length, text.Length - (openBracket.Length + closeBracket.Length));
            }
            else
            {
                return text;
            }
        }
        public static string[] SmartSplit(this string text, string openBracket, string closeBracket, string delim)
        {
            SymbolFinder tnzr = new SymbolFinder(new string[] { openBracket, closeBracket, delim });
            StringBuilder sb = new StringBuilder(); // sb stands for StringBuilder
            List<string> ab = new List<string>(); // ab is supose to stand for array-builder
            int depth = 0;
            char c;
            char? next;
            string smb;
            for (int i = 0; i < text.Length; i++)
            {
                c = text[i];
                next = i == text.Length - 1 ? null : (char?)text[i + 1];
                smb = tnzr.Find(c, next);

                if (smb == openBracket) { depth++; }
                if (smb == closeBracket) { depth--; }

                sb.Append(c);

                if (depth == 0)
                {
                    if (smb == delim)
                    {
                        sb.Remove(sb.Length - smb.Length, smb.Length);
                        ab.Add(sb.ToString()); // PARTIAL DUPLICATE A
                        sb.Clear();
                    }
                }
            }

            if (sb.Length != 0) { ab.Add(sb.ToString()); } //PARTIAL DUPLICATE A

            return ab.ToArray();
        }
        public static bool IsWrapped(this string text, string openBracket, string closeBracket)
        {
            return text.IsWrapped(new string[] { openBracket }, new string[] { closeBracket });
        }
        public static bool IsWrapped(this string text, string[] openBrackets, string[] closeBrackets)
        {
            SymbolFinder finder = new SymbolFinder(openBrackets.Concat(closeBrackets).ToArray());
            char c;
            char next;
            int depth = 0;
            bool startsWithOpenBracket = false;
            bool endsWithOpenBracket = false;
            string openFound;
            string closeFound;
            string found;
            for (int i = 0; i < text.Length; i++)
            {
                openFound = "";
                closeFound = "";
                c = text[i];
                if (i == text.Length - 1)
                {
                    next = '\0';
                }
                else
                {
                    next = text[i + 1];
                }
                found = finder.Find(c, next);
                if (openBrackets.Contains(found))
                {
                    openFound = found;
                }
                else
                {
                    closeFound = found;
                }

                if (openFound.Length > 0)
                {
                    depth++;
                    if (!startsWithOpenBracket)
                    {
                        if (openFound.Length == i + 1)
                        {
                            startsWithOpenBracket = true;
                        }
                        else
                        {
                            return false; /*--(FAIL)--*/
                        }
                    }
                }
                if (closeFound.Length > 0)
                {
                    depth--;
                    if (i == text.Length - 1)
                    {
                        endsWithOpenBracket = true;
                    }
                    else
                    {
                        if (depth == 0)
                        {
                            return false; /*--(FAIL)--*/
                        }
                    }
                }
            }
            return startsWithOpenBracket && endsWithOpenBracket && (depth == 0);

            // is it (...) or (..)..(...)?,  (* = not 0): depth pattern for a wrapped string "1********0"
            //{

            //SymbolFinder openFinder = new SymbolFinder(openBracket);
            //SymbolFinder closeFinder = new SymbolFinder(closeBracket);

            //int depth = 0;
            //char c = '\0';
            //char next = '\0';

            //for (int i = 0; i < text.Length; i++)
            //{
            //    c = text[i];
            //    if (i == text.Length - 1)
            //    {
            //        next = '\0';
            //    }
            //    else
            //    {
            //        next = text[i + 1];
            //    }
            //    if (openFinder.Find(c, next).Length != 0) { depth++; }
            //    if (closeFinder.Find(c, next).Length != 0) { depth--; }
            //    #if DEBUG
            //    Console.WriteLine($"{c} : {(next != '\0' ? next.ToString() : "\\0")},  depth: {depth}");
            //    #endif

            //    if (i == openBracket.Length - 1)
            //    {
            //        //^ is first character
            //        // it must start with an openBracket, if the first character is an open bracket, then the depth will be emediatly 1, not 0.
            //        if (depth != 1) { return false; }
            //    }
            //    else if (i == text.Length - 1)
            //    {
            //        //^ is last character
            //        if (depth != 0) { return false; }
            //    }
            //    else if (i >= openBracket.Length - 1 && i < (text.Length - 1))
            //    {
            //        if (depth == 0) { return false; }
            //        //^ is not first or last (middle character)
            //    }
            //}
            ////^ has passed test

            //return true;
        }
    }

    /// <summary>
    /// This class is capable of finding multi-character symbols in strings. example: "a := x + 7" -> ":=", "+".
    /// This is done using the find method. The standard use is in a for loop, looping through a string such that the currect character and the next character are provided.
    /// Like this:
    /// 
    /// SymbolFinder finder = new SymbolFinder(ARRAY OF SYMBOLS TO FIND);
    /// 
    /// char c;
    /// char? next;
    /// int text_lastIndex = text.Length - 1
    /// string symb;
    /// for(int i = 0; i < text.Length)
    /// {
    ///     c = text[i];
    ///     next = i == text_lastIndex ? null : (char?) text[i + 1];
    ///     
    ///     symb = finder.Find(c, next);
    /// }
    /// </summary>
    class SymbolFinder
    {
        public SymbolFinder(string[] symbols)
        {
            this.symbols = symbols.ToList();
            symbols_size = symbols.Length;
            //first build of possible
            for (int i = 0; i < symbols_size; i++) { possible.Add(i); }
        }
        public SymbolFinder(string op)
            : this(new string[] { op })
        { }
        public SymbolFinder()
            : this(new string[0])
        { }
        public void Reset()
        {
            opIndex = 0;
            //rebuild possible
            possible.Clear();
            for (int i = 0; i < symbols_size; i++) { possible.Add(i); }
        }
        public void Reset(string[] symbols)
        {
            this.symbols = symbols.ToList();
            symbols_size = symbols.Length;
            Reset();
        }

        /// <summary>
        /// Returns the found symbol or an empty string if no symbol was found.
        /// </summary>
        /// <returns></returns>
        public string Find(char c, char? next)
        {
            possiblyFound = -1;
            for (int i = possible.Count - 1; i >= 0; i--)
            {
                string op = symbols[possible[i]];
                if (op.Length <= opIndex
                    || op[opIndex] != c)
                {
                    possible.RemoveAt(i);
                }
                else if (op.Length == opIndex + 1)
                {
                    possiblyFound = possible[i];
                }
            }

            if (!possible.Any())
            {
                //reset
                opIndex = 0;
                //rebuild possible
                for (int i = 0; i < symbols_size; i++) { possible.Add(i); }
                //
                //

                //skip the rest
                return "";
            }

            nextPossible = false;
            foreach (int p in possible)
            {
                string op = symbols[p];

                // Will this operator still be possible with the next character?
                if (op.Length > (opIndex + 1)
                    && op[opIndex + 1] == next)
                {
                    nextPossible = true;
                    break;
                }
            }

            if (!nextPossible)
            {
                //reset
                opIndex = 0;
                possible.Clear();
                //rebuild possible
                for (int i = 0; i < symbols_size; i++) { possible.Add(i); }
                //
                //
                if (possiblyFound == -1)
                {
                    return "";
                }
                else
                {
                    return symbols[possiblyFound];
                }
            }
            else
            {
                opIndex++;
                return "";
            }
        }
        /// <summary>
        /// Returns true if a symbol was found.
        /// </summary>
        /// <param name="c">Current character.</param>
        /// <param name="next">Last character.</param>
        /// <param name="symbol">Returns the found symbol or an empty string if no symbol was found.</param>
        public bool Found(char c, char? next, out string symbol)
        {
            symbol = Find(c, next);
            return symbol.Length != 0;
        }
        /// <summary>
        /// Returns true if a symbol was found.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public bool Found(char c, char? next)
        {
            return Find(c, next).Length != 0;
        }
        #region hidden
        private List<int> possible = new List<int>();
        private int possiblyFound = -1;
        private bool nextPossible = false;
        private int symbols_size;
        private int opIndex = 0;
        private List<string> symbols;
        #endregion
    }


    class Tokenizer
    {
        public string[] Separators { get; }
        public string[] Delims { get; }
        public string[] OpenParens { get; }
        public string[] CloseParens { get; }
        public string[] Quotes { get; }
        public string[] PreBreaks { get; }
        public string[] PostBreaks { get; }
        private SymbolFinder symbolFinder;
        private void ResetSymbolFinder()
        {
            symbolFinder.Reset(Separators.Union(Delims).Union(OpenParens).Union(CloseParens).Union(Quotes).Union(PreBreaks).Union(PostBreaks).ToArray());
        }

        public Tokenizer(string[] separators, string[] delims, string[] openParens, string[] closeParens, string[] quotes, string[] preBreaks, string[] postBreaks)
        {
            Separators = separators.ToArray();
            Delims = delims.ToArray();
            OpenParens = openParens.ToArray();
            CloseParens = openParens.ToArray();
            Quotes = quotes.ToArray();
            PreBreaks = preBreaks.ToArray();
            PostBreaks = postBreaks.ToArray();
            if (OpenParens.Length != CloseParens.Length)
            {
                throw new UnequalParenArrays("The arrays: openParens and closeParens must be the same length so that every openParen has a corresponding closeParen.");
            }
            symbolFinder = new SymbolFinder(Separators.Union(Delims).Union(OpenParens).Union(CloseParens).Union(Quotes).Union(PreBreaks).Union(PostBreaks).ToArray());
        }

        public bool TrimWhitespace { get; set; } = true;
        public bool BreakOnWhiteSpace { get; set; } = false;
    }
}