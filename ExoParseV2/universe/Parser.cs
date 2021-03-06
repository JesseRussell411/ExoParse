﻿using System;
using System.Collections.Generic;
using ParsingTools;
using System.Linq;
using System.Collections.Immutable;
using ExoParseV2.theUniverse;
using ExoParseV2.elements;

using JesseRussell.LinqExtension;

namespace ExoParseV2
{
    #region Item
    internal enum ItemType
    {
        Element,
        Operator,
        PreModifier,
        PostModifier,
        Container
    }
    internal class Item
    {
        public object DefiningObject
        {
            get
            {
                switch (Type)
                {
                    case ItemType.Element:
                        return Element;
                    case ItemType.Operator:
                        return Operator;
                    case ItemType.PreModifier:
                        return PreModifier;
                    case ItemType.PostModifier:
                        return PostModifier;
                    case ItemType.Container:
                        return SubItems;
                    default:
                        return null;
                }
            }
        }
        public bool TriedParsingElement { get; set; } = false;

        public int? GetPriority(SymbolizedIndex si)
        {
            switch (Type)
            {
                case ItemType.Operator:
                    return Operator?.GetPriority(si);
                case ItemType.PreModifier:
                    return PreModifier?.GetPriority(si);
                case ItemType.PostModifier:
                    return PostModifier?.GetPriority(si);
                default:
                    return null;
            }
        }
        public ItemType? Type { get; set; } = null;
        public Item(string text)
            : this(text, null, null, null, null, null)
        {
        }

        public Item(IElement element)
            : this(element?.ToString(), element, null, null, null, null)
        {
            Type = ItemType.Element;
        }

        public Item(Operator @operator)
            : this(@operator?.ToString(), null, @operator, null, null, null)
        {
            Type = ItemType.Operator;
        }

        public Item(PreModifier preModifier)
            : this(preModifier?.ToString(), null, null, preModifier, null, null)
        {
            Type = ItemType.PreModifier;
        }

        public Item(PostModifier postModifier)
            : this(postModifier?.ToString(), null, null, null, postModifier, null)
        {
            Type = ItemType.PostModifier;
        }

        public Item(LinkedList<Item> subItems)
            : this("", null, null, null, null, subItems)
        {
            Type = ItemType.Container;
        }
        public Item(string text, IElement element, Operator @operator, PreModifier preModifier, PostModifier postModifier, LinkedList<Item> subItems)
        {
            Text = text;
            Element = element;
            Operator = @operator;
            PreModifier = preModifier;
            PostModifier = postModifier;
            SubItems = subItems;
        }

        public bool ConfusedSymbol
        {
            get
            {
                int count = 0;
                if (Operator != null) count++;
                if (PreModifier != null) count++;
                if (PostModifier != null) count++;
                return count > 1;
            }
        }

        public bool Unparsed
        {
            get
            {
                return (Element ?? Operator ?? PreModifier ?? (object)PostModifier) == null;
            }
        }

        public String Text { get; }
        public IElement Element { get; set; }
        public Operator Operator { get; set; }
        public PreModifier PreModifier { get; set; }
        public PostModifier PostModifier { get; set; }
        public LinkedList<Item> SubItems { get; set; }

        public override string ToString()
        {
            return $"[{Text}]";
        }
    }
    #endregion
    public class Parser
    {
        public static IEnumerable<string> ems = Enumerable.Empty<string>();
        public Operator DefaultOperator { get; set; }
        public IElement DefaultElement { get; set; } = IElement.Void;
        public SymbolizedIndex SymbolizedIndex { get; set; }
        public Universe Universe { get; set; }
        public IElement Starter { get; set; }
        public Parser(SymbolizedIndex si, Universe universe)
        {
            SymbolizedIndex = si;
            Universe = universe;

            stage1Tokenizer = new Tokenizer(ems, ems, ems, ems, StringProps.OpenBrackets, StringProps.CloseBrackets, ems, ems)
            { BreakOnWhiteSpace = true };

            stage2Tokenizer = new Tokenizer(si.AllSymbols, ems, ems, ems, StringProps.OpenBrackets, StringProps.CloseBrackets, ems, ems);

            stage3Tokenizer = new Tokenizer(ems, ems, ems, ems, StringProps.OpenBrackets, StringProps.CloseBrackets, ems, ems, true);
        }

        public bool TryParseBaseElement(String s, out IElement result, Dictionary<string, IReference> localLabeled = null)
        {

            if (TryParseConstant(s, out Literal c))
            {
                result = c;
                return true;
            }
            else if (TryParseContainer(s, localLabeled, out Container con))
            {
                result = con;
                return true;
            }
            else if (TryParseLabeled(s, out IReference l, localLabeled))
            {
                result = l;
                return true;
            }
            else if (TryParseExecution(s, localLabeled,  out Execution e))
            {
                result = e;
                return true;
            }
            else
            {
                result = null;
                return false;
            }

        }



        #region parse element/ expression
        public IElement ParseElement(string s, Dictionary<string, IReference> localLabeled = null)
        {
            bool startsWithWhitespace = s.Length > 0 && char.IsWhiteSpace(s[0]);
            //bool startsWithWhitespace = s.Length > 0 && s[0].IsWhiteSpace();
            s = s.Trim();
            if (s.Length == 0) { return null; } // give up if line is empty

            return InternalParseElement(s, localLabeled, Starter, startsWithWhitespace);
        }
        private IElement InternalParseElement(string s, Dictionary<string, IReference> localLabeled, IElement starter = null, bool startsWithWhitespace = false)
        {
            #region internal function definitions
            /// AKA: this code gets run later.
            bool TryParseBaseElementFromItem(Item i, out IElement result)
            {
                if (i.TriedParsingElement) { result = null; return false; } // The point of this internal method is to check this property and give up if the item has already been tried(as in tryParseBaseElement).

                i.TriedParsingElement = true; // Don't forget to mark it for later. The whole point of this method is optimization.
                return TryParseBaseElement(i.Text, out result, localLabeled); // If the item hasn't already been tried, then try.
            }
            #endregion

            #region stage -1:
            // Clean(remove) whitespace and brackets.
            string s_cleaned = s.Trim();
            if (s.Length == 0) { return IElement.Void; } // 
            //s_cleaned = s.UnWrap(ParsingProps.OpenBrackets, ParsingProps.CloseBrackets);
            #endregion

            //#region stage 0
            //if (TryParseBaseElement(s_cleaned, out IElement e)){
            //    return e;
            //}
            //#endregion

            #region stage 1
            // Break on whitespace.
            List<Item> items = stage1Tokenizer.Tokenize(s_cleaned).Select(t => new Item(t)).ToList(); // seperate string on whitespace and store in a list of items (special object used just for parsing)
            foreach (Item item in items)
            {
                // elements
                if (item.Element == null && TryParseBaseElementFromItem(item, out IElement e)) // Try to parse each individual item
                {
                    item.Element = e;
                }
            }

            if (items.Count == 1 && items[0].Element != null) { return items[0].Element; } // skip the rest if we have what we need now, a single element successfully parsed.
            // *this replaces stage 0
            #endregion

            #region stage 2
            // Break on operators
            items = items
                .Select(i =>
                    (tokens: i.Unparsed ? stage2Tokenizer.Tokenize(i.Text) : new List<string>(), item: i)) //   Tokenize each item that hasn't been discovered to be element or operator or modifier
                .Select(t => t.tokens.Count == 0 ? t.item.Enumerate() : t.tokens.Select(to => new Item(to))) // Itemize the tokens
                .SelectMany(m => m).ToList(); // flatten the 2d item list

            foreach (Item item in items)
            {
                if (SymbolizedIndex.TryGetItems(item.Text.Trim(), out ImmutableArray<ISymbolized> syms))
                {
                    foreach (ISymbolized symb in syms)
                    {
                        if (symb is Operator op)
                        {
                            item.Operator = op;
                        }
                        else if (symb is PreModifier prm)
                        {
                            item.PreModifier = prm;
                        }
                        else if (symb is PostModifier pom)
                        {
                            item.PostModifier = pom;
                        }
                    }
                }

                // try parsing elements again
                if (item.Element == null && !item.TriedParsingElement && TryParseBaseElementFromItem(item, out IElement e))
                {
                    item.Element = e;
                }
            }

            #endregion


            #region stage 3 + convert to linked list
            // Break on brackets.

            items = items
                .Select(i =>
                    (tokens: i.Unparsed ? stage3Tokenizer.Tokenize(i.Text) : new List<string>(), item: i)) //   Tokenize each item that hasn't been discovered to be element or operator or modifier
                .Select(t => t.tokens.Count == 0 ? t.item.Enumerate() : t.tokens.Select(to => new Item(to))) // Itemize the tokens
                .SelectMany(m => m).ToList(); // flatten the 2d item list






            //!\/!\/!\     Convert to linked list
            LinkedList<Item> items_ll = new LinkedList<Item>();

            // Try parsing the new tokenized elements
            // And convert to linked list
            foreach (Item item in items)
            {
                if (item.Element == null && TryParseBaseElementFromItem(item, out IElement e))
                {
                    item.Element = e;
                }

                //!\/!\/!\      I'm piggy backing off this loop to fill the new linkedlist with items. Hopefully it helps performance
                items_ll.AddLast(item);

            }
            #endregion


            #region stage 2.5
            // Add on starter element (probably finalVariable: ans (previouse answer)) if applicable
            if (!startsWithWhitespace &&
                (items_ll.First != null && items_ll.First.Value.Operator != null && starter != null))
            {
                items_ll.AddFirst(new Item(starter));
            }
            #endregion

            #region stage 4
            // Finalize Element and Operator items.

            // This means that after this stage the items are definitely elements or they are definitely not: operators, modifiers, or ItemContainers. Or vise/versa, etc. etc. for operators and pre/post modifiers.


            LinkedListNode<Item> currentNode = items_ll.First;
            do
            {
                Item prev = currentNode.Previous?.Value;
                Item current = currentNode.Value;
                Item next = currentNode.Next?.Value;

                bool prevIsValid;
                bool nextIsValid;
                if (current.Element != null)
                {
                    current.Type = ItemType.Element;
                    prevIsValid = prev?.PreModifier != null || prev?.Operator != null || prev == null;
                    nextIsValid = next?.PostModifier != null || next?.Operator != null || next == null;

                    if (!prevIsValid)
                    {
                        if (DefaultOperator != null) { items_ll.AddBefore(currentNode, new LinkedListNode<Item>(new Item(DefaultOperator))); }
                    }
                    if (!nextIsValid)
                    {
                        if (DefaultOperator != null) { items_ll.AddAfter(currentNode, new LinkedListNode<Item>(new Item(DefaultOperator))); }
                    }
                }
            } while ((currentNode = currentNode.Next) != null);



            int? op_lp = null;
            int? prem_lp = null;
            int? pom_lp = null;
            currentNode = items_ll.First;
            do
            {
                Item prev = currentNode.Previous?.Value;
                Item current = currentNode.Value;
                Item next = currentNode.Next?.Value;

                bool canBeOp =
                    (prev?.Element != null || prev?.PostModifier != null) &&
                    (next?.Element != null || next?.PreModifier != null);

                bool canBePrem =
                    (prev?.Operator != null || prev?.PreModifier != null || prev == null) &&
                    (next?.Element != null || next?.PreModifier != null);

                bool canBePom =
                    (prev?.Element != null || prev?.PostModifier != null) &&
                    (next?.Operator != null || next?.PostModifier != null || next == null);

                if (canBeOp && current.Operator != null)
                {
                    current.Type = ItemType.Operator;

                    int p = SymbolizedIndex.GetPriority(current.Operator);
                    op_lp = op_lp == null ? p : Math.Min(p, (int)op_lp);
                }
                else if (canBePrem && current.PreModifier != null)
                {
                    current.Type = ItemType.PreModifier;

                    int p = SymbolizedIndex.GetPriority(current.PreModifier);
                    prem_lp = prem_lp == null ? p : Math.Min(p, (int)prem_lp);
                }
                else if (canBePom && current.PostModifier != null)
                {
                    current.Type = ItemType.PostModifier;

                    int p = SymbolizedIndex.GetPriority(current.PostModifier);
                    pom_lp = pom_lp == null ? p : Math.Min(p, (int)pom_lp);
                }

                if (current.Type == null)
                {
                    throw new ParsingException($"Unkown item: {current.Text}");
                }
            } while ((currentNode = currentNode.Next) != null);
            #endregion

            #region stage 5
            // This section is called recursively so it has to be contained in its own function, see function: Stage5(LinkedList<Item> items, int? op_lp, int? prem_lp, int? pom_lp).
            Stage5(items_ll, op_lp, prem_lp, pom_lp);
            #endregion



            #region stage 6
            // This section is called recursively so it has to be contained in its own function, see function: Stage6(LinkedList<Item> items)
            Stage6(items_ll);
            #endregion


            return items_ll.First?.Value?.Element;
        }





        // group operators/ modifiers/ and elements apropriately
        private LinkedList<Item> Stage5(LinkedList<Item> items, int? op_lp, int? prem_lp, int? pom_lp)
        {
            if (items.First == null) { return null; }
            LinkedList<Item> subNodes;
            LinkedListNode<Item> subNode;
            int? rec_op_lp = null;
            int? rec_prem_lp = null;
            int? rec_pom_lp = null;


            #region local methods
            void getRecLp(Item item)
            {
                int p;
                switch (item.Type)
                {
                    case ItemType.Operator:
                        p = (int)item.GetPriority(SymbolizedIndex);
                        rec_op_lp = rec_op_lp == null ? p : Math.Min(p, (int)rec_op_lp);
                        break;
                    case ItemType.PreModifier:
                        p = (int)item.GetPriority(SymbolizedIndex);
                        rec_prem_lp = rec_prem_lp == null ? p : Math.Min(p, (int)rec_prem_lp);
                        break;
                    case ItemType.PostModifier:
                        p = (int)item.GetPriority(SymbolizedIndex);
                        rec_pom_lp = rec_pom_lp == null ? p : Math.Min(p, (int)rec_pom_lp);
                        break;
                }
            }
            void getLP(Item item)
            {
                int p;
                switch (item.Type)
                {
                    case ItemType.Operator:
                        p = (int)item.GetPriority(SymbolizedIndex);
                        op_lp = op_lp == null ? p : Math.Min(p, (int)op_lp);
                        break;
                    case ItemType.PreModifier:
                        p = (int)item.GetPriority(SymbolizedIndex);
                        prem_lp = prem_lp == null ? p : Math.Min(p, (int)prem_lp);
                        break;
                    case ItemType.PostModifier:
                        p = (int)item.GetPriority(SymbolizedIndex);
                        pom_lp = pom_lp == null ? p : Math.Min(p, (int)pom_lp);
                        break;
                }
            }
            #endregion


            // Group pre-modifiers
            LinkedListNode<Item> currentNode = items.First;
            #region Group by pre-modifiers
            if (prem_lp != null)
            {
                op_lp = null;
                //prem_lp = null;
                pom_lp = null;

                do
                {
                    Item current = currentNode.Value;


                    if (current.DefiningObject is PreModifier && current.GetPriority(SymbolizedIndex) == prem_lp)
                    {
                        subNode = currentNode;
                        subNodes = new LinkedList<Item>();

                        rec_op_lp = null;
                        rec_prem_lp = null;
                        rec_pom_lp = null;
                        while ((subNode = currentNode.Next) != null && ((!(subNode.Value.DefiningObject is PostModifier) && !(subNode.Value.DefiningObject is Operator)) || subNode.Value.GetPriority(SymbolizedIndex) >= prem_lp))
                        {
                            getRecLp(subNode.Value);

                            subNodes.AddLast(subNode.Value);
                            items.Remove(subNode);
                        }

                        if (subNodes.First != null)
                        {
                            Item i = subNodes.First.Next == null ? subNodes.First.Value : new Item(Stage5(subNodes, rec_op_lp, rec_prem_lp, rec_pom_lp));
                            items.AddAfter(currentNode, i);
                        }
                    }
                    getLP(currentNode.Value);
                } while ((currentNode = currentNode.Next) != null);
            }
            #endregion


            #region Group by post-Modifiers
            if (pom_lp != null)
            {
                op_lp = null;
                prem_lp = null;
                //pom_lp = null;
                currentNode = items.Last;
                do
                {
                    Item current = currentNode.Value;

                    if (current.DefiningObject is PostModifier && current.GetPriority(SymbolizedIndex) == pom_lp)
                    {
                        subNode = currentNode;
                        subNodes = new LinkedList<Item>();

                        rec_op_lp = null;
                        rec_prem_lp = null;
                        rec_pom_lp = null;
                        while ((subNode = currentNode.Previous) != null && ((!(subNode.Value.DefiningObject is PreModifier) && !(subNode.Value.DefiningObject is Operator)) || subNode.Value.GetPriority(SymbolizedIndex) >= pom_lp))
                        {
                            getRecLp(subNode.Value);

                            items.Remove(subNode);
                            subNodes.AddFirst(subNode.Value);
                        }
                        if (subNodes.First != null)
                        {
                            Item i = subNodes.First.Next == null ? subNodes.First.Value : new Item(Stage5(subNodes, rec_op_lp, rec_prem_lp, rec_pom_lp));
                            items.AddBefore(currentNode, i);
                        }
                    }
                    getLP(currentNode.Value);
                } while ((currentNode = currentNode.Previous) != null);
            }
            #endregion


            #region Group by operators
            //throw new NotImplementedException();
            if (op_lp != null)
            {
                //op_lp = null;
                prem_lp = null;
                pom_lp = null;
                subNodes = new LinkedList<Item>();

                currentNode = items.First;
                LinkedListNode<Item> currentNode_Next = null;
                // This section works in two stages, collectLeft and !collectLeft
                // collectLeft:
                // 1+8*7
                // | |
                // collect this stuff here
                // !collectLeft:
                // 1+8*7
                //    | - operator found
                bool collectLeft = true;
                do
                {
                    Item current = currentNode.Value;
                    if (current.DefiningObject is Operator && current.GetPriority(SymbolizedIndex) == op_lp)
                    {
                        // The item is an operator that we are trying to look for, so lets group the items to the left are right accordingly
                        Item i;
                        if (collectLeft && subNodes.First != null)
                        {
                            i = subNodes.First.Next == null /*if true, the list only contains one item*/ ? subNodes.First.Value : new Item(Stage5(subNodes, rec_op_lp, rec_prem_lp, rec_pom_lp));
                            items.AddBefore(currentNode, i);
                        }
                        collectLeft = false;

                        subNode = currentNode;
                        subNodes = new LinkedList<Item>();

                        rec_op_lp = null;
                        rec_prem_lp = null;
                        rec_pom_lp = null;
                        while ((subNode = currentNode.Next) != null && (!(subNode.Value.DefiningObject is Operator) || subNode.Value.GetPriority(SymbolizedIndex) > op_lp))
                        {
                            getRecLp(subNode?.Value);

                            subNodes.AddLast(subNode.Value);
                            items.Remove(subNode);
                        }

                        if (subNodes.First != null)
                        {
                            i = subNodes.First.Next == null ? subNodes.First.Value : new Item(Stage5(subNodes, rec_op_lp, rec_prem_lp, rec_pom_lp));
                            items.AddAfter(currentNode, i);
                        }
                    }

                    currentNode_Next = currentNode.Next;
                    if (collectLeft)
                    {
                        getRecLp(current);
                        subNodes.AddLast(current);
                        items.Remove(currentNode);
                    }

                } while ((currentNode = currentNode_Next) != null);
            }
            #endregion
            return items;
        }

        // Convert subItems to elements / finishing stages
        private IElement Stage6(LinkedList<Item> items)
        {
            if (items.First == null) { return null; }
            LinkedListNode<Item> currentNode;
            #region Stage 6A
            currentNode = items.First;
            do
            {
                if (currentNode.Value.Type == ItemType.Container)
                {
                    currentNode.Value = new Item(Stage6(currentNode.Value.SubItems));
                }
            } while ((currentNode = currentNode.Next) != null);
            #endregion


            #region Stage 6B

            #region post-modifiers
            currentNode = items.Last;
            do
            {
                if (currentNode.Value.DefiningObject is PostModifier)
                {
                    PostModifier mod = currentNode.Value.PostModifier;
                    IElement element;
                    if (currentNode.Previous.Value.DefiningObject is IElement)
                    {
                        element = currentNode.Previous.Value.Element;
                    }
                    else
                    {
                        throw new ParsingException($"Element expected before post modifier: {currentNode.Value.PostModifier}");
                    }
                    items.Remove(currentNode.Previous);
                    currentNode.Value = new Item(new Modification(mod, element));
                }
            } while ((currentNode = currentNode.Previous) != null);
            #endregion

            #region pre-modifiers
            currentNode = items.First;
            do
            {
                if (currentNode.Value.DefiningObject is PreModifier)
                {
                    PreModifier mod = currentNode.Value.PreModifier;
                    IElement element;
                    if (currentNode.Next?.Value?.DefiningObject is IElement)
                    {
                        element = currentNode.Next.Value.Element;
                        items.Remove(currentNode.Next);
                        currentNode.Value = new Item(new Modification(mod, element));
                    }
                    else
                    {
                        throw new ParsingException($"Element expected after pre modifier: {currentNode.Value.PreModifier}");
                    }
                }
            } while ((currentNode = currentNode.Next) != null);
            #endregion


            #region left-to-right operators
            currentNode = items.First;
            do
            {
                if (currentNode.Value.DefiningObject is LeftToRightOperator)
                {
                    Operator op = currentNode.Value.Operator;
                    IElement A = currentNode.Previous.Value.Element;
                    IElement B = currentNode.Next.Value.Element;
                    items.Remove(currentNode?.Previous);
                    items.Remove(currentNode?.Next);

                    Operation opr = new Operation(op, A, B);
                    currentNode.Value = new Item(opr);


                    while (currentNode.Next != null)
                    {
                        currentNode = currentNode.Next;
                        items.Remove(currentNode.Previous);
                        A = opr;
                        B = currentNode.Next?.Value?.Element;
                        if (currentNode.Next != null) { items.Remove(currentNode.Next); }
                        opr = new Operation(currentNode.Value.Operator, A, B);
                    }
                    currentNode.Value = new Item(opr);
                }
            } while ((currentNode = currentNode.Next) != null);
            #endregion

            #region right-to-left operators
            currentNode = items.Last;
            do
            {
                if (currentNode.Value.DefiningObject is RightToLeftOperator)
                {
                    Operator op = currentNode.Value.Operator;
                    IElement B = currentNode.Next.Value.Element;
                    IElement A = currentNode.Previous.Value.Element;
                    items.Remove(currentNode.Next);
                    items.Remove(currentNode.Previous);

                    Operation opr = new Operation(op, A, B);
                    currentNode.Value = new Item(opr);


                    while (currentNode.Previous != null)
                    {
                        currentNode = currentNode.Previous;
                        B = opr;
                        A = currentNode.Previous.Value.Element;
                        items.Remove(currentNode.Previous);
                        opr = new Operation(currentNode.Value.Operator, A, B);
                    }
                    currentNode.Value = new Item(opr);
                }
            } while ((currentNode = currentNode.Previous) != null);
            #endregion
            #endregion

            return items.First?.Value?.Element;
        }

        private Tokenizer stage1Tokenizer;
        private Tokenizer stage2Tokenizer;
        private Tokenizer stage3Tokenizer;
        #endregion




        public bool TryParseContainer(String s, Dictionary<string, IReference> localLabeled, out Container result)
        {
            if (s.IsWrapped(StringProps.OpenBrackets, StringProps.CloseBrackets, out string opening, out string closing))
            {
                result = new Container(InternalParseElement(s.UnWrap(StringProps.OpenBrackets, StringProps.CloseBrackets), localLabeled), opening, closing);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public bool TryParseConstant(String s, out Literal result)
        {
            return Literal.TryParse(s, out result);
        }

        public bool TryParseLabeled(String s, out IReference result, Dictionary<string, IReference> localLabeled)
        {
            //if (localLabeled != null && localLabeled.TryGetValue(s, out result))
            //{
            //    return true;
            //}

            if (localLabeled == null)
            {
                if (Universe.References.TryGetValue(s, out result))
                {
                    return true;
                }
                else
                {
                    if (this.IsLabel(s))
                    {
                        result = new Variable(s);
                        Universe.References.Add(result.Name, result);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (Universe.References.TryGetValue(s, out result) && result is Constant)
                {
                    return true;
                }
                else if (localLabeled.TryGetValue(s, out result))
                {
                    return true;
                }
                else
                {
                    if (this.IsLabel(s))
                    {
                        result = new Variable(s);
                        localLabeled.Add(result.Name, result);
                        //Universe.NamedItems.Add(result.Name, result);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

        }

        public bool TryParseExecution(string s, Dictionary<string, IReference> localLabeled, out Execution result)
        {
            //(string name, List<String> args) partParsed;
            if (IsFunction(s, out var partParsed, out string opening, out string closing))
            {
                if (TryParseExecution(partParsed, localLabeled, out result, opening, closing))
                {
                    return true;
                }
            }
            result = null;
            return false;
        }

        public bool TryParseExecution((String name, List<String> args) partParsed, Dictionary<string, IReference> localLabeled, out Execution result, string openingBracket = null, string closingBracket = null)
        {
            (string name, int argCount) id = (partParsed.name, partParsed.args.Count);
            Function f;
            if (Universe.Functions.TryGetValue(id, out f))
            {
                result = new Execution(f, partParsed.args.Select(a => InternalParseElement(a, localLabeled)).ToArray(), openingBracket, closingBracket);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        #region is...

        public bool IsLabel(String text)
        {
            //structure of variable: "{a-b,A-B,_, 0-9,.}"

            // variable labels can have: uppercase/lowercase letters, underscore(_), period(.), and numbers but they have to contain at least one non-number character.
            // variable labels can not contain whitespace

            //also it must have at least one character: text.Length >= 1
            #region this bit does stuff to, even though it's before declarations
            if (!(text.Length >= 1))
            {
                return false; //--(FAIL)--
            }
            //else if (text.Trim().Length != text.Length)
            //{
            //    return false; //--(FAIL)--

            //    // Why: the text starts or ends with some sort of whitespace which is not allowd
            //}
            #endregion

            bool contains_non_NumberOrSpace = false;

            {
                //bool first = true;
                foreach (char c in text)
                {
                    bool isNumber = char.IsNumber(c);
                    bool isSpace = char.IsWhiteSpace(c);
                    if (char.IsLetter(c) ||
                        isNumber         ||
                        c == '_'         ||
                        c == '.')
                    {
                        //test passed, so far --(PASS?)--
                    }
                    else
                    {
                        //test failed --(FAIL)--
                        return false;
                    }

                    if (!(isNumber | isSpace)) { contains_non_NumberOrSpace = true; }

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
        #region IsFunction
        public bool IsFunction(string s, out (string name, List<string> args) partParsed)
        {
            return IsFunction(s, out partParsed, out string trashA, out string trashB);
        }
        public bool IsFunction(string s, out (string name, List<string> args) partParsed, out string openingBracket, out string closingBracket)
        {
            List<string> sl;
            sl = IsFunction_BracketTokenizer.Tokenize(s);
            if (sl.Count == 2)
            {
                if (IsLabel(sl[0]) && sl[1].IsWrapped(StringProps.OpenBrackets, StringProps.CloseBrackets, out openingBracket, out closingBracket))
                {
                    partParsed = (sl[0], IsFunction_DelimTokenizer.Tokenize(sl[1].UnWrap(openingBracket, closingBracket)));
                    return true; //--(PASS)--
                }
            }

            partParsed = (null, null);
            openingBracket = null;
            closingBracket = null;
            return false; //--(FAIl)--

        }
        public static Tokenizer IsFunction_BracketTokenizer = new Tokenizer(ems, ems, ems, ems, StringProps.OpenBrackets, StringProps.CloseBrackets, ems, ems, true);
        public static Tokenizer IsFunction_DelimTokenizer = new Tokenizer(ems, StringProps.Delims, ems, ems, StringProps.OpenBrackets, StringProps.CloseBrackets, ems, ems, false);
        #endregion
        #endregion
    }
}
