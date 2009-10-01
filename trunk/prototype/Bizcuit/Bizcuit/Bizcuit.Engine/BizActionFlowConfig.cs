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
			if (!dict.ContainsKey(actionFlowName))
			{
				dict.Add(actionFlowName, digest);
			}
			else
			{
				throw new Exception("Duplicate action flow name.");
			}
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
			string entryActionName = null;
			foreach (XmlAttribute attr in node.Attributes)
			{
				if (attr.Name == "name")
				{
					digest = config.AddActionFlowDigest(attr.Value);
				}
				else if (attr.Name == "entry")
				{
					entryActionName = attr.Value;
				}
			}
			if (digest != null)
			{
				digest.EntryActionName = entryActionName;
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
			string actionClassName = null;
			foreach (XmlAttribute attr in actionNode.Attributes)
			{
				if (attr.Name == "name")
				{
					actionName = attr.Value;
				}
				else if (attr.Name == "class")
				{
					actionClassName = attr.Value;
				}
			}
			if (actionName == null)
				return;	// TODO: 抛出Engine的ConfigParserException异常。需要定义一套异常。
			IBizActionDigest actionDigest = digest.AddActionDigest(actionName, actionClassName);


			// 获取action.next Tag的信息
			foreach (XmlNode node in actionNode.ChildNodes)
			{
				if (node.Name == "action.next")
				{
					string nextActionName = null;
					string condition = null;
					foreach (XmlAttribute attr in node.Attributes)
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
				else if (node.Name == "return")
				{
					XmlNode returnTypeNode = node.ChildNodes.Item(0);
					string nodeName = returnTypeNode.Name;
					if (nodeName == "content")
					{
						string contentType = null;
						string contentSrc = null;
						string contentValue = null;
						foreach (XmlAttribute attr in returnTypeNode.Attributes)
						{
							if (attr.Name == "type")
							{
								contentType = attr.Value;
							}
							else if (attr.Name == "src")
							{
								contentSrc = attr.Value;
							}
							else if (attr.Name == "value")
							{
								contentValue = attr.Value;
							}
						}

						actionDigest.SetReturnContentType(contentType);
						actionDigest.SetReturnContent(contentValue);
						actionDigest.SetReturnContentSource(contentSrc);





					}
					else if (nodeName == "value")
					{
						// TODO: check value type?
					}

					
				}
			}

		}

		

		private static void ParseGlobalActionNode(XmlNode node)
		{
			Console.WriteLine("ParseGlobalActionNode");

		}

	}
}
