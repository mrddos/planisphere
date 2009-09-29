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


		private Dictionary<string, IBizActionFlowDigest> dict = new Dictionary<string, IBizActionFlowDigest>();





		IBizActionFlowDigest IBizActionFlowConfig.AddActionFlowDigest(string actionFlowName)
		{
			IBizActionFlowDigest digest = new BizActionFlowDigest(actionFlowName);
			dict.Add(actionFlowName, digest);
			return digest;
		}

		IBizActionFlowDigest IBizActionFlowConfig.GetActionFlowDigest(string actionFlowName)
		{
			return dict[actionFlowName];
		}


		// TODO: Create a Config file Parser later.
		// Y: BizActionFlowConfig现在内部聚合一个Parser
		public static void Load(string fileName, IBizActionFlowConfig config)
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
						ParseActionFlowNode(node, config);
					}
					else if (node.Name == "global.action")
					{
						ParseGlobalActionNode(node);
					}
				}
			}
		}

		private static void ParseActionFlowNode(XmlNode node, IBizActionFlowConfig config)
		{
			Console.WriteLine("ParseActionFlowNode");
			IBizActionFlowDigest digest = null;
			foreach (XmlAttribute attr in node.Attributes)
			{
				if (attr.Name == "name")
				{
					digest = config.AddActionFlowDigest(attr.Value);
				}
			}
			if (digest != null)
			{
				XmlNodeList children = node.ChildNodes;
				foreach (XmlNode child in children)
				{
					if (child.NodeType == XmlNodeType.Element)
					{
						if (child.Name == "actions")
						{
							ParseActionsNode(child, digest);
						}
					}
				}
			}
		}

		private static void ParseActionsNode(XmlNode actionsNode, IBizActionFlowDigest digest)
		{
			foreach (XmlNode node in actionsNode.ChildNodes)
			{
				if (node.NodeType == XmlNodeType.Element && node.Name == "action")
				{
					ParseActionNode(node, digest);
				}
			}
		}


		private static void ParseActionNode(XmlNode actionNode, IBizActionFlowDigest digest)
		{
			// 主要获取name和class属性值，保存到Config对象中。
			string actionName = null;
			string actionClass = null;
			foreach (XmlAttribute attr in actionNode.Attributes)
			{
				if (attr.Name == "name")
				{
					actionName = attr.Value;
				}
				else if (attr.Name == "class")
				{
					actionClass = attr.Value;
				}
			}
			if (actionName == null)
				return;	// TODO: 抛出Engine的ConfigParserException异常。需要定义一套异常。
			IBizActionDigest actionDigest = digest.AddActionDigest(actionName);


			// 获取action.next Tag的信息
			foreach (XmlNode nextNode in actionNode.ChildNodes)
			{
				if (nextNode.Name == "action.next")
				{
					string nextActionName = null;
					string condition = null;
					foreach (XmlAttribute attr in nextNode.Attributes)
					{
						if (attr.Name == "next")
						{
							nextActionName = attr.Value;
						}
						else if (attr.Name == "condition")
						{
							condition = attr.Value;
						}
					}
					actionDigest.SetNextActionOnCondition(condition, nextActionName);
				}
			}

		}

		

		private static void ParseGlobalActionNode(XmlNode node)
		{
			Console.WriteLine("ParseGlobalActionNode");

		}

	}
}
