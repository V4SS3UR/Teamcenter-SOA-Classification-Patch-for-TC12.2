using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Teamcenter.Schemas.Soa._2011_06.Metamodel;

namespace Teamcenter.Soa.Internal.Client.Parse
{
    public abstract class Node_patch
    {
        private static readonly IDictionary<Type, Type> CDM_TYPES = new Dictionary<Type, Type>()
        {
            {
                typeof (Teamcenter.Soa.Client.Model.ServiceData),
                typeof (Teamcenter.Schemas.Soa._2006_03.Base.ServiceData)
            },
            {
                typeof (Teamcenter.Soa.Client.Model.Preferences),
                typeof (Teamcenter.Schemas.Soa._2006_03.Base.Preferences)
            },
            {
                typeof (Teamcenter.Soa.Client.Model.PartialErrors),
                typeof (Teamcenter.Schemas.Soa._2006_03.Base.PartialErrors)
            },
            {
                typeof (Teamcenter.Schemas.Soa._2006_03.Base.ModelSchema),
                typeof (Teamcenter.Schemas.Soa._2006_03.Base.ModelSchema)
            },
            {
                typeof (TypeSchema),
                typeof (TypeSchema)
            }
        };

        private static readonly IDictionary<Type, Type> POLICY_TYPES = new Dictionary<Type, Type>()
        {
            {
                typeof (Teamcenter.Soa.Common.ObjectPropertyPolicy),
                typeof (Teamcenter.Soa.Common.ObjectPropertyPolicy)
            },
            {
                typeof (Teamcenter.Soa.Common.PolicyType),
                typeof (Teamcenter.Soa.Common.PolicyType)
            },
            {
                typeof (Teamcenter.Soa.Common.PolicyProperty),
                typeof (Teamcenter.Soa.Common.PolicyProperty)
            }
        };

        private static readonly ISet<Type> ELEMENT_TYPES = (ISet<Type>)new HashSet<Type>()
        {
            typeof (Teamcenter.Soa.Client.Model.ServiceData),
            typeof (Teamcenter.Soa.Client.Model.Preferences),
            typeof (Teamcenter.Soa.Client.Model.PartialErrors),
            typeof (Teamcenter.Soa.Common.ObjectPropertyPolicy)
        };

        protected static Node BuildChildNode(Type type, string name, FieldInfo field, Hashtable allNodes)
        {
            if (allNodes.Contains(type))
            {
                if (((Node)allNodes[type]).LocalName == name) //Patch
                {
                    return (Node)allNodes[type];
                }
            }

            if (!type.IsArray)
            {
                return !PrimitiveNode.IsPrimitiveType(type) ?
                            (!typeof(Hashtable).IsAssignableFrom(type) ?
                                (!typeof(Teamcenter.Soa.Client.Model.ModelObject).IsAssignableFrom(type) ?
                                    (!CDM_TYPES.ContainsKey(type) ?
                                        (!POLICY_TYPES.ContainsKey(type) ?
                                            (Node)new StructNode(type, name, field, allNodes) :
                                            (Node)new PolicyNode(type, name, allNodes)) :
                                        (Node)new CDMStructNode(type, CDM_TYPES[type], allNodes)) :
                                    (Node)new ModelObjectNode(type, name)) :
                                (Node)new MapNode(type, name, field, allNodes)) :
                            (Node)PrimitiveNode.Builder(type, name);
            }

            Type elementType = type.GetElementType();
            return !PrimitiveNode.IsPrimitiveType(elementType) ?
                      (!typeof(Teamcenter.Soa.Client.Model.ModelObject).IsAssignableFrom(elementType) ?
                          (Node)new StructArrayNode(type, name, field, allNodes) :
                          (Node)new ModelObjectArrayNode(type, name, field, allNodes)) :
                      (Node)new PrimitiveArrayNode(type, name);
        }
    }
}