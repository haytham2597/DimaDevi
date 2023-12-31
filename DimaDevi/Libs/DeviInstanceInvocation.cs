﻿using DimaDevi.Modules;
using System.Collections.Generic;

namespace DimaDevi.Libs
{
    internal class DeviInstanceInvocation
    {
        private static DeviInstanceInvocation instance;
        public IList<IDeviComponent> Components = new List<IDeviComponent>();
        private DeviInstanceInvocation()
        {
            DeviDefaultSet.GetInstance().AddThis(this);
        }
        /// <summary>
        /// Reset global configuration
        /// </summary>
        public void Reset()
        {
            DeviDefaultSet.GetInstance().SetThis(this);
        }
        public static DeviInstanceInvocation GetInstance()
        {
            return instance ?? (instance = new DeviInstanceInvocation());
        }
    }
}
