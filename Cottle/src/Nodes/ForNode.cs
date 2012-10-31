﻿using System.Collections.Generic;
using System.IO;

using Cottle.Expressions;
using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    ForNode : INode
    {
        #region Attributes

        private INode           body;

        private INode           empty;

        private IExpression     from;

        private NameExpression   key;

        private NameExpression   value;

        #endregion

        #region Constructors

        public  ForNode (IExpression from, NameExpression key, NameExpression value, INode body, INode empty)
        {
            this.body = body;
            this.empty = empty;
            this.from = from;
            this.key = key;
            this.value = value;
        }

        #endregion

        #region Methods

        public bool Apply (Scope scope, TextWriter output, out Value result)
        {
            FieldMap    fields = this.from.Evaluate (scope, output).Fields;

            if (fields.Count > 0)
            {
                foreach (KeyValuePair<Value, Value> pair in fields)
                {
                    scope.Enter ();

                    if (this.key != null)
                        this.key.Set (scope, pair.Key, ScopeMode.Local);

                    if (this.value != null)
                        this.value.Set (scope, pair.Value, ScopeMode.Local);

                    if (this.body.Apply (scope, output, out result))
                    {
                        scope.Leave ();

                        return true;
                    }

                    scope.Leave ();
                }
            }
            else if (this.empty != null)
            {
                scope.Enter ();

                if (this.empty.Apply (scope, output, out result))
                {
                    scope.Leave ();

                    return true;
                }

                scope.Leave ();
            }

            result = UndefinedValue.Instance;
            
            return false;
        }

        public void Print (ISetting setting, TextWriter output)
        {
            output.Write (setting.BlockBegin);
            output.Write ("for ");

            if (this.key != null)
            {
                output.Write (this.key);
                output.Write (", ");
            }

            output.Write (this.value);
            output.Write (" in ");
            output.Write (this.from);
            output.Write (":");

            this.body.Print (setting, output);

            if (this.empty != null)
            {
                output.Write (setting.BlockContinue);
                output.Write ("empty:");

                this.empty.Print (setting, output);
            }

            output.Write (setting.BlockEnd);
        }

        #endregion
    }
}
