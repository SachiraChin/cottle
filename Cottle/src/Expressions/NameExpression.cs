﻿using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Expressions
{
	class NameExpression : IExpression
	{
		#region Attributes

		private readonly string	name;

		#endregion

		#region Constructors

		public	NameExpression (string name)
		{
			this.name = name;
		}

		#endregion

		#region Methods

		public Value Evaluate (IScope scope, TextWriter output)
		{
			Value	value;

			if (scope.Get (this.name, out value))
				return value;

			return VoidValue.Instance;
		}

		public bool Set (IScope scope, Value value, ScopeMode mode)
		{
			return scope.Set (this.name, value, mode);
		}

		public override string ToString ()
		{
			return this.name;
		}

		#endregion
	}
}
