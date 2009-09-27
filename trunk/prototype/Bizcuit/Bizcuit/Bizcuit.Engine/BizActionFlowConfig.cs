using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;
using System.IO;
using System.Xml;

namespace Bizcuit.Engine
{
	public class BizActionFlowConfig : IBizActionFlowConfig
	{

		private Dictionary<string, 

		// TODO: Create a Config file Parser later.
		// Y: BizActionFlowConfig现在内部聚合一个Parser
		public static IBizActionFlowConfig Load(string fileName)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(fileName);

			XmlElement element = xmlDoc.DocumentElement;
			XmlNodeList nodes = element.ChildNodes;

			foreach (XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Element)
				{
					if (node.Name == "actionflow")	// TODO: 提取字符串到一个Common的模块中
					{
						ParseActionFlowNode(node);
					}
					else if (node.Name == "global.action")
					{
						ParseGlobalActionNode(node);
					}
				}
			}
			return null;
		}

		private static void ParseActionFlowNode(XmlNode node)
		{
			Console.WriteLine("ParseActionFlowNode");
			foreach (XmlAttribute attr in node.Attributes)
			{
				Console.WriteLine(attr.Name + ":" + attr.Value);
			}

			XmlNodeList children = node.ChildNodes;
			foreach (XmlNode child in children)
			{
				if (child.NodeType == XmlNodeType.Element)
				{
					if (child.Name == "actions")
					{
						ParseActionsNode(child);
					}
				}
			}

		}

		private static void ParseActionsNode(XmlNode actionsNode)
		{
			foreach (XmlNode node in actionsNode.ChildNodes)
			{
				if (node.NodeType == XmlNodeType.Element && node.Name == "action")
				{
					ParseActionNode(node);
				}
			}
		}


		private static void ParseActionNode(XmlNode actionNode)
		{
			// 主要获取name和class属性值，保存到Config对象中。
			foreach (XmlAttribute attr in actionNode.Attributes)
			{
				if (attr.Name == "name")
				{
					Console.Write(attr.Value + ": ");
				}
				else if (attr.Name == "class")
				{
					Console.WriteLine(attr.Value);
				}
			}
			// 获取action.next Tag的信息
		}

		

		private static void ParseGlobalActionNode(XmlNode node)
		{
			Console.WriteLine("ParseGlobalActionNode");

		}

	}
}
