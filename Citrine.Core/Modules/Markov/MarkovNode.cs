using System;
using System.Collections.Generic;

namespace Citrine.Core.Modules.Markov
{
	public class MarkovNode
	{
		public string Value { get; set; } = "";

		public List<MarkovNode> Children { get; set; } = new List<MarkovNode>();

		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public MarkovNode() { }

		public MarkovNode(string value) => Value = value;

		public static MarkovNode Start => new MarkovNode("START OF NODE");

		public static MarkovNode End { get; } = new MarkovNode("END OF NODE")
		{
			CreatedAt = default,
			Children = new List<MarkovNode>(),
		};

		public static bool operator ==(MarkovNode n1, MarkovNode n2) => n1?.Value == n2?.Value;
		public static bool operator !=(MarkovNode n1, MarkovNode n2) => n1?.Value != n2?.Value;

		public override bool Equals(object obj)
		{
			return obj is MarkovNode node &&
				   Value == node.Value &&
				   EqualityComparer<List<MarkovNode>>.Default.Equals(Children, node.Children) &&
				   CreatedAt == node.CreatedAt;
		}

		public override int GetHashCode()
		{
			int hashCode = -1319415750;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
			hashCode = hashCode * -1521134295 + EqualityComparer<List<MarkovNode>>.Default.GetHashCode(Children);
			hashCode = hashCode * -1521134295 + CreatedAt.GetHashCode();
			return hashCode;
		}
	}
}
