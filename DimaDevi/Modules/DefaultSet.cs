using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DimaDevi.Libs;

namespace DimaDevi.Modules
{
    //Singleton
    /// <summary>
    /// A module that after disposed class set all props/fields to default value
    /// Why this module? I can create new Instance... Well this is for NOT Create new instance of class or for Clear() method for example
    /// </summary>
    internal class DefaultSet : IDisposable
    {
        private static DefaultSet instance;

        /// <summary>
        /// [Class Object] -> [Props/Fields -> Value]
        /// </summary>
        private readonly Dictionary<object, Dictionary<MemberInfo, object>> ClassMemberValue = new Dictionary<object, Dictionary<MemberInfo, object>>();
        private DefaultSet()
        {

        }

        /// <summary>
        /// Add this Object and remember defaults values
        /// </summary>
        /// <param name="obj"></param>
        public void AddThis(object obj)
        {
            var members = obj.GetMembers();
            ClassMemberValue.Add(obj, new Dictionary<MemberInfo, object>());
            var dict = new Dictionary<MemberInfo, object>();
            using (var enumer = members.GetEnumerator())
            {
                while (enumer.MoveNext())
                {
                    if (enumer.Current == null)
                        continue;
                    object value = null;
                    if (enumer.Current.MemberType == MemberTypes.Field)
                        value = (enumer.Current as FieldInfo).GetValue(obj);
                    if (enumer.Current.MemberType == MemberTypes.Property)
                        value = (enumer.Current as PropertyInfo).GetValue(obj);
                    dict.Add(enumer.Current, value);
                }
            }
            ClassMemberValue[obj] = dict;
        }

        /// <summary>
        /// Set by default all values of this object
        /// </summary>
        /// <param name="obj"></param>
        public void SetThis(object obj)
        {
            if (!ClassMemberValue.ContainsKey(obj))
                return;
            var dict = ClassMemberValue[obj];
            var members = obj.GetMembers().ToList();
            if (members.Count != dict.Count) //Impossible, should be equal
                return;
            for (int i = 0; i < members.Count; i++) //Is supposed that all by order have same props/fields
            {
                var elem = dict.ElementAt(i);
                if (members[i] != elem.Key) //WTF
                    continue;

                //TODO: If is Collection,List,Observable, Dictionary, etc. Clear the list
                
                members[i].SetMembers(obj, elem.Value);
            }
        }
        public static DefaultSet GetInstance()
        {
            return instance ?? (instance = new DefaultSet());
        }

        public void Dispose()
        {
            ClassMemberValue.Clear();
        }
    }
}
