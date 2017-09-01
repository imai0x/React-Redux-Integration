﻿using OnlineStore.Core.CrossCuttingConcerns.Logging.Log4Net;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using OnlineStore.Core.CrossCuttingConcerns.Logging;

namespace OnlineStore.Core.CrossCuttingConcerns.Aspects.Postsharp.LogAspects
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method, TargetExternalMemberAttributes = MulticastAttributes.Instance)]
    public class LogAspect : OnMethodBoundaryAspect
    {
        private Type _logerType;
        private LoggerService _loggerService;

        public LogAspect(Type loggerType)
        {
            _logerType = loggerType;
        }

        public override void RuntimeInitialize(MethodBase method)
        {
            if (_logerType.BaseType != typeof(LoggerService))
            {
                throw new Exception("Wrong Loogger Type");
            }

            _loggerService = (LoggerService)Activator.CreateInstance(_logerType);

            base.RuntimeInitialize(method);
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            if (!_loggerService.IsInfoEnabled)
            {
                return;
            }

            try
            {
                var logParameters = args.Method.GetParameters().Select((t, i) =>
                    new LogParameter
                    {
                        Name = t.Name,
                        Type = t.ParameterType.Name,
                        Value = args.Arguments.GetArgument(i)
                    }).ToList();

                var logDetail = new LogDetail
                {
                    FullName = args.Method.DeclaringType == null ? null : args.Method.DeclaringType.Name,
                    MethodName = args.Method.Name,
                    Parameters = logParameters
                };

                _loggerService.Info(logDetail);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
