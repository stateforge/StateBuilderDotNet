#region Copyright
//------------------------------------------------------------------------------
// <copyright file="VisitorEventInterface.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.Collections.Generic;
    using ICSharpCode.NRefactory;
    using ICSharpCode.NRefactory.PrettyPrinter;
    using ICSharpCode.NRefactory.Visitors;
    using ICSharpCode.NRefactory.Ast;

    /// <summary>
    /// VisitorEventInterface is passed as a parameter of CompilationUnit.AcceptVisitor. 
    /// Events are created based on the methods of the given interface.
    /// </summary>
    public class VisitorEventInterface : AbstractAstVisitor
    {
        public bool InterfaceFound { get; protected set; }
        StateMachineType model;
        string interfaceName;
        string feeder;

        public VisitorEventInterface(StateMachineType model, string interfaceName, string feeder)
        {
            this.model = model;
            this.interfaceName = interfaceName;
            this.feeder = feeder;
        }

        /// <summary>
        /// Called when a method is visited.
        /// </summary>
        /// <param name="methodDeclaration"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
        {
            INode nodeParent = methodDeclaration.Parent;

            if (nodeParent is TypeDeclaration)
            {
                TypeDeclaration typeDeclaration = (TypeDeclaration)methodDeclaration.Parent;

                EventType evt = new EventType();

                evt.id = methodDeclaration.Name;
                if (methodDeclaration.Parameters.Count > 0)
                {
                    evt.parameter = new ParameterType[methodDeclaration.Parameters.Count];
                    for (int i = 0; i < methodDeclaration.Parameters.Count; i++)
                    {
                        ParameterDeclarationExpression paramDecl = methodDeclaration.Parameters[i];
                        ParameterType param = new ParameterType();
                        param.name = paramDecl.ParameterName;
                        param.type = paramDecl.TypeReference.ToString();
                        evt.parameter[i] = param;
                    }
                }
                model.AddEvent(evt);
                model.AddEventToFeeder(feeder, evt.id);
            }

            return base.VisitMethodDeclaration(methodDeclaration, data);
        }

        /// <summary>
        /// Called when a class or interface is visited.
        /// </summary>
        /// <param name="typeDeclaration"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
        {
            if (typeDeclaration.Name == interfaceName)
            {
                this.InterfaceFound = true;
                return base.VisitTypeDeclaration(typeDeclaration, data);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Called when "using" is visited.
        /// </summary>
        /// <param name="@using"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override object VisitUsing(Using @using, object data)
        {
            var usingArray = model.settings.@using;
            Array.Resize<string>(ref usingArray, usingArray.Length + 1);
            model.settings.@using = usingArray;
            usingArray[usingArray.Length - 1] = @using.Name;
            return base.VisitUsing(@using, data);
        }
    }
}

