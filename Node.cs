// Assembly: TcSoaClient, Version=12000.2.0.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using Teamcenter.Schemas.Soa._2011_06.Metamodel;
using Teamcenter.Soa.Internal.Client.Model;
using Teamcenter.Soa.Internal.Common;

namespace Teamcenter.Soa.Internal.Client.Parse
{
    public abstract class Node
    {
        protected static Node BuildChildNode(Type type, string name, FieldInfo field, Hashtable allNodes)
        {
            if (allNodes.Contains((object)type))
                return (Node)allNodes[(object)type];
            if (!type.IsArray)
                return !PrimitiveNode.IsPrimitiveType(type) ?
                            (!typeof(Hashtable).IsAssignableFrom(type) ?
                                (!typeof(Teamcenter.Soa.Client.Model.ModelObject).IsAssignableFrom(type) ?
                                    (!Node.CDM_TYPES.ContainsKey(type) ? (!Node.POLICY_TYPES.ContainsKey(type) ?
                                        (Node)new StructNode(type, name, field, allNodes) :
                                        (Node)new PolicyNode(type, name, allNodes)) :
                                    (Node)new CDMStructNode(type, Node.CDM_TYPES[type], allNodes)) :
                                (Node)new ModelObjectNode(type, name)) :
                                (Node)new MapNode(type, name, field, allNodes)) :
                            (Node)PrimitiveNode.Builder(type, name);

            Type elementType = type.GetElementType();
            return !PrimitiveNode.IsPrimitiveType(elementType) ?
                        (!typeof(Teamcenter.Soa.Client.Model.ModelObject).IsAssignableFrom(elementType) ?
                            (Node)new StructArrayNode(type, name, field, allNodes) :
                            (Node)new ModelObjectArrayNode(type, name, field, allNodes)) :
                        (Node)new PrimitiveArrayNode(type, name);
        }
    }
}
